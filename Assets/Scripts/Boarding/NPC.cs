using System;
using System.Threading;
using System.Threading.Tasks;
using DebugUtils;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(Animator))]
[RequireComponent(typeof(PathFollower))]
public class NPC : MonoBehaviour
{
    Animator animator;

    [SerializeField] PathFollower pathFollower;
    public PathFollower Path => pathFollower;


    private TaskCompletionSource<bool> _pathCompleteTcs;
    private CancellationTokenSource _cts;

    private Vector3 previousPosition;

    [Space]
    [SerializeField] BoardType boardType;

    [Space]
    [SerializeField] PathData onBoardingPath;
    [SerializeField] PathData offBoardingPath;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        pathFollower.OnPathComplete += PathCompleteAction;

        pathFollower.OnReachWaypoint += ReachWayPointAction;

        pathFollower.OnAnimationSpeedChanged += AnimationSpeedChange;

        switch(boardType)
        {
            case BoardType.onBoard:
                animator.Play("walking");
                break;
        }

        if(boardType == BoardType.offBoard)

         animator.Play("walking");
        else
         animator.Play("running");

        _cts = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        pathFollower.OnPathComplete -= PathCompleteAction;
        pathFollower.OnReachWaypoint -= ReachWayPointAction;

        pathFollower.OnAnimationSpeedChanged -= AnimationSpeedChange;

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        _pathCompleteTcs?.TrySetCanceled();
        _pathCompleteTcs = null;
    }

    private void Update()
    {
        float speed = Vector3.Distance(transform.position, previousPosition) / Time.deltaTime;

        previousPosition = transform.position;
       // DevDebug.Log($"Speed: {speed}", DebugColor.Silver);

        animator.speed = Mathf.Clamp(speed / 2f, 1.5f, 1f);
    }

    public void SetBoard(BoardType type) { boardType = type; }

    private void ReachWayPointAction(int wayPoint)
    {
        DevDebug.Log($"ReachedPoint : {wayPoint}", DebugColor.Teal);

        switch(boardType)
        {
            case BoardType.onBoard:

                if (wayPoint == 1)
                {
                    animator.CrossFadeInFixedTime("walking", 0.8f);
                }


                if (wayPoint == 2)
                {
                    animator.CrossFadeInFixedTime("Crouched", 0.8f);
                }

                break;

            case BoardType.offBoard:

                if(wayPoint == 1)
                {
                    animator.CrossFadeInFixedTime("Crouched", 0.8f);
                }

                if (wayPoint == 2)
                {
                    animator.CrossFadeInFixedTime("walking", 0.8f);
                }

                if (wayPoint == 3)
                {
                    animator.CrossFadeInFixedTime("walking", 0.8f);
                }

                break;
        }
        
       

        //animator.Play("walking");
    }

    private void AnimationSpeedChange(float val){   /*animator.SetFloat("Speed", val);*/ }

    public void SetPathDuration(float duration) { pathFollower.SetTargetDuration(duration); }


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
        animator.Play("Idle");
        _pathCompleteTcs?.TrySetResult(true);
    }

    // ✅ Public — any script can await this
    public Task WaitForPathComplete(CancellationToken token)
    {
        _pathCompleteTcs = new TaskCompletionSource<bool>();
        //token.Register(() => _pathCompleteTcs.TrySetCanceled());
        var tcs = _pathCompleteTcs;
        token.Register(()=>
        {
            tcs?.TrySetCanceled();
        });
        return tcs.Task;
    }
}
