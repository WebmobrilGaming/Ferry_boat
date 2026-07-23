using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField] PathFollower pathFollower;
    [SerializeField] private float rotationSpeed = 90f;
    private Coroutine rotateCoroutine;
    public PathFollower Path => pathFollower;


    private TaskCompletionSource<bool> _pathCompleteTcs;
    private CancellationTokenSource _cts;

    [SerializeField] BoardCharType boardCharType;
    public BoardCharType Type => boardCharType;

    [Space]

    public float wheelRotationMultiplier = 300f;

    public List<Transform> tyres = new List<Transform>();

    [Header("TurnWheels:")]
    [SerializeField] Transform f1Tyre;
    [SerializeField] Transform f2Tyre;

    [Space]
    [SerializeField] private float steerSensitivity = 250f;
    [SerializeField] private float maxSteerAngle = 180f;
    [SerializeField] private float steerSmooth = 10f;


    [SerializeField]

    float currentSteerAngle;

    private Vector3 lastPosition;
    private float lastYRotation;

    [SerializeField] BoardType boardType;

    public BoardType BoardType => boardType;


    [Header("Paths:")]
    [SerializeField] PathData onBoardPath;
    [SerializeField] PathData offBoardPath;

    Transform parkingArea;

    private void OnEnable()
    {
        pathFollower.OnPathComplete += PathCompleteAction;
        pathFollower.OnReachWaypoint+=ReachWayPointAction;

        _cts = new CancellationTokenSource();
    }

    public void BookParkingArea(Transform park) { parkingArea = park; }

    private void ReachWayPointAction(int current)
    {
        //Debug.LogWarning($"Reached Waypoint {current}");
        //if(current == 2)
        //{
        //    pathFollower._orientToPath = false;

        //    Quaternion targetRotation = CompareTag("Truck")
        //        ? Quaternion.Euler(0, 1.60655582f, 0)
        //        : Quaternion.Euler(-3.153f, -5.198f, 0.755f);

        //    StartCoroutine(SmoothRotate(targetRotation));
        //}
    }

    private IEnumerator SmoothRotate(Quaternion targetRotation)
    {
        // Stop any previous rotation
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        rotateCoroutine = null;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                 Time.deltaTime);

            yield return null;
        }

        transform.rotation = targetRotation;
    }

    public void SetPathDuration(float duration) { pathFollower.SetTargetDuration(duration); }

    private void OnDisable()
    {
        pathFollower.OnPathComplete -= PathCompleteAction;
        pathFollower.OnReachWaypoint -= ReachWayPointAction;

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
    {  if(Time.deltaTime == 0f) return;

        float speed = Vector3.Distance(this.transform.position, lastPosition) / Time.deltaTime;
        lastPosition = this.transform.position;

        float rotation = -speed * wheelRotationMultiplier * Time.deltaTime;
        foreach (Transform t in tyres)
        {
            t.Rotate(rotation, 0f, 0f, Space.Self);
        }

        float delta = Mathf.DeltaAngle(lastYRotation, transform.eulerAngles.y);

        float targetSteerAngle = Mathf.Clamp( delta * steerSensitivity,
                                     -maxSteerAngle, maxSteerAngle);

        currentSteerAngle = Mathf.Lerp(
                                        currentSteerAngle,
                                        targetSteerAngle,
                                        steerSmooth * Time.deltaTime
                                      );

        currentSteerAngle = (float)System.Math.Round(currentSteerAngle, 3);

        f2Tyre.localRotation = Quaternion.Euler(0, currentSteerAngle, 0);
        f1Tyre.localRotation = Quaternion.Euler(0, currentSteerAngle, 0);

        lastYRotation = transform.eulerAngles.y;

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

    public void SetBoard(BoardType type)
    {
        boardType = type;

        pathFollower._path = type switch
        {
            BoardType.onBoard => onBoardPath,
            BoardType.offBoard => offBoardPath,
            _=> onBoardPath
        };
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
