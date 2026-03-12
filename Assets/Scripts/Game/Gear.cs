using DG.Tweening;
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

    [SerializeField] bool mStat;
    public bool Stat => mStat;

    public IGear callback;

    private void OnEnable()
    {
        mStat = false;
    }

    public void Change(bool stat)
   {
        Sequence seq = DOTween.Sequence();

        if (!stat)
        {
            StartCoroutine(WaitForAnimation(stat, () =>
            {
                mStat = stat;
                callback.GearChange();
            }));

            return;
        }

        StartCoroutine(WaitForAnimation(stat, () =>
        {
            mStat = stat;
            callback.GearChange();

        }));
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
