using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField] PathFollower pathFollower;
    public PathFollower Path => pathFollower;


    private TaskCompletionSource<bool> _pathCompleteTcs;
    private CancellationTokenSource _cts;

    [SerializeField] BoardCharType boardCharType;
    public BoardCharType Type => boardCharType;

    private void OnEnable()
    {
        pathFollower.OnPathComplete += PathCompleteAction;

        _cts = new CancellationTokenSource();
    }

    public void SetPathDuration(float duration) { pathFollower.SetTargetDuration(duration); }

    private void OnDisable()
    {
        pathFollower.OnPathComplete -= PathCompleteAction;

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        _pathCompleteTcs?.TrySetCanceled();
        _pathCompleteTcs = null;
    }

    private void OnDestroy()
    {
        pathFollower.OnPathComplete -= PathCompleteAction;

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        _pathCompleteTcs?.TrySetCanceled();
        _pathCompleteTcs = null;
    }

    private void PathCompleteAction()
    {
        _pathCompleteTcs?.TrySetResult(true);
    }

    // ✅ Public — any script can await this
    public Task WaitForPathComplete(CancellationToken token)
    {
        _pathCompleteTcs = new TaskCompletionSource<bool>();
        token.Register(() => _pathCompleteTcs?.TrySetCanceled());
        return _pathCompleteTcs.Task;
    }
}
