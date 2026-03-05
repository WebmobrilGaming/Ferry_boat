using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ButtonMove : MonoBehaviour
{
    [SerializeField] private RectTransform[] mButtons;

    private Vector2[] startPos;

    void Awake()
    {
        startPos = new Vector2[mButtons.Length];

        for(int i = 0; i < mButtons.Length; i++)
        {
            startPos[i] = mButtons[i].anchoredPosition;
        }
    }

    void OnEnable()
    {
        ResetButtons();
        ButtonMoveF();
    }

    void ResetButtons()
    {
        for(int i = 0; i < mButtons.Length; i++)
        {
            mButtons[i].anchoredPosition = startPos[i];
        }
    }

    public void ButtonMoveF()
    {
        Sequence seq = DOTween.Sequence();

        for(int i = mButtons.Length - 1; i >= 0; i--)
        {
            seq.Append(
                mButtons[i].DOAnchorPosX(
                    startPos[i].x + 700f,
                    0.2f
                ).SetEase(Ease.OutQuad)
            );
        }
    }
}