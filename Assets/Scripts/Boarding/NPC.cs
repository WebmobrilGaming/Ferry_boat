using System;
using System.Threading;
using System.Threading.Tasks;
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

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        pathFollower.OnPathComplete += PathCompleteAction;

        pathFollower.OnAnimationSpeedChanged += AnimationSpeedChange;

        animator.Play("walking");

        _cts = new CancellationTokenSource();
    }

    private void OnDisable()
    {
        pathFollower.OnPathComplete -= PathCompleteAction;
        pathFollower.OnAnimationSpeedChanged -= AnimationSpeedChange;

        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        _pathCompleteTcs?.TrySetCanceled();
        _pathCompleteTcs = null;
    }

    private void AnimationSpeedChange(float val){ /*animator.SetFloat("Speed", val);*/ }

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
