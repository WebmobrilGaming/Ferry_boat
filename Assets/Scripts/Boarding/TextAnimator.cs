using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

[RequireComponent(typeof(TMP_Text))]
public class TextAnimator : MonoBehaviour
{
    private TMP_Text mtext;
    private RectTransform rectTransform;

    [SerializeField] private Color initC;
    [SerializeField] private Vector2 initPos;

    private void Awake()
    {
        mtext = GetComponent<TMP_Text>();
        rectTransform = GetComponent<RectTransform>();

        initC = mtext.color;
        initPos = rectTransform.anchoredPosition;
    }

    public void ResetAction()
    {
        rectTransform.anchoredPosition = initPos;
        mtext.color = initC;
    }

    public void Animate(string text, Color color, Action onComplete)
    {
        mtext.text = text;

        // Start from initial state
        rectTransform.anchoredPosition = initPos;
        mtext.color = new Color(color.r, color.g, color.b, 0f);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(
                    mtext.DOColor(new Color(color.r, color.g, color.b, 1f), 1f))
                .Join(
                    rectTransform.DOAnchorPos(initPos + Vector2.up * 20f, 1f))
                .Append(
                    mtext.DOColor(new Color(color.r, color.g, color.b, 0f), 0.5f))
                .OnComplete(() =>
                {
                    rectTransform.anchoredPosition = initPos;
                    mtext.color = initC;
                    onComplete?.Invoke();
                });
    }
}