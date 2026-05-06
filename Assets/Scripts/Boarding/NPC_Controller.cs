using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(Animator))]
[RequireComponent(typeof(PathFollower))]
public class NPC : MonoBehaviour
{
    Animator animator;

    [SerializeField] PathFollower pathFollower;

    private TaskCompletionSource<bool> _pathCompleteTcs;
    private CancellationTokenSource _cts;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        pathFollower.OnPathComplete += PathCompleteAction;

        _cts = new CancellationTokenSource();
    }

    private void OnDisable()
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
        animator.Play("Idle");
        _pathCompleteTcs?.TrySetResult(true);
    }

    // ✅ Public — any script can await this
    public Task WaitForPathComplete(CancellationToken token)
    {
        _pathCompleteTcs = new TaskCompletionSource<bool>();
        token.Register(() => _pathCompleteTcs.TrySetCanceled());
        return _pathCompleteTcs.Task;
    }
}
