using DebugUtils;
using DG.Tweening;
using FerryBoat;
using System;
using System.Collections;
using UnityEngine;

public interface IGear
{
    public void GearChange();
}

public class Gear : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Animator _animator;

    [SerializeField] float mRange;

    [SerializeField] bool mStat;
    public bool Stat => mStat;

    public IGear callback;

    private void OnEnable()
    {
        mStat = false;

        Act.SpeedChange += SpeedChange;
        Act.SpeedInit += RangeSet;
    }

    private void RangeSet(float val)
    {
        mRange = val;
    }

    private void OnDisable()
    {
        Act.SpeedChange -= SpeedChange;
        Act.SpeedInit -= RangeSet;
    }

    public void Change(bool stat)
   {
        Sequence seq = DOTween.Sequence();

        mStat = stat;
        callback.GearChange();

        //StartCoroutine(WaitForAnimation(stat, () =>
        //{
        //    mStat = stat;
        //    callback.GearChange();

        //}));
    }

    private void SpeedChange(float val)
    {
        float t = Mathf.InverseLerp(0, mRange, val);

        DevDebug.Log($"Animation normalised speed: {t}", DebugColor.Brown);
        _animator.Play("GearON", 0, t);
        _animator.speed = 0f;
    }


    IEnumerator WaitForAnimation(bool stat ,Action onComplete)
    {
        string animation = stat ? "GearON" : "GearOFF";
        _animator.Play(animation);

        // Wait one frame for animator to update
        yield return null;

        // Wait until animation is finished
        yield return new WaitUntil(() =>
            _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f &&
            !_animator.IsInTransition(0));

        Debug.Log("Animation Finished");
        onComplete?.Invoke();
        // Do next action here
    }
}
