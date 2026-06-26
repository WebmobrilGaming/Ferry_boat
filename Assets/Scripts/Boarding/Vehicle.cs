using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class Vehicle : MonoBehaviour
{
    [SerializeField] PathFollower pathFollower;
    public PathFollower Path => pathFollower;


    private TaskCompletionSource<bool> _pathCompleteTcs;
    private CancellationTokenSource _cts;

    [SerializeField] BoardCharType boardCharType;
    public BoardCharType Type => boardCharType;

    [Space]

    public float wheelRotationMultiplier = 300f;

    public List<Transform> tyres = new List<Transform>();

    private Vector3 lastPosition;
    private float lastYRotation;

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

    private void Start()
    {
        lastPosition = this.transform.position;
        lastYRotation = transform.eulerAngles.y;
    }

    private void Update()
    {  
        float speed = Vector3.Distance(this.transform.position, lastPosition) / Time.deltaTime;
        lastPosition = this.transform.position;

        float rotation = -speed * wheelRotationMultiplier * Time.deltaTime;

        foreach (Transform t in tyres)
            t.Rotate(rotation, 0f, 0f, Space.Self);

        //float delta = Mathf.DeltaAngle(lastYRotation, transform.eulerAngles.y);
        //lastYRotation = transform.eulerAngles.y;


        //float steerAngle = Mathf.Clamp(delta * 10f, -30f, 30f);

        //tyres[0].localRotation = Quaternion.Euler(rotation, steerAngle, 0);
        //tyres[2].localRotation = Quaternion.Euler(rotation, steerAngle, 0);
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
