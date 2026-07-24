using TMPro;
using UnityEngine;

using DG.Tweening;
using System;

[RequireComponent(typeof(TMP_Text))]
public class TextAnimator : MonoBehaviour
{
    TMP_Text mtext;

   [SerializeField] Color initC;
   [SerializeField]Vector3 initPos;

    private void Awake()
    {
        mtext = GetComponent<TMP_Text>();

        initC = mtext.color;
    }

    public void ResetAction()
    {
        mtext.transform.position = initPos;
        mtext.color = initC;
    }

    public void Animate(string  text ,Color color,Action onComplete)
    {
        Color c = mtext.color;

        mtext.text = text;
        mtext.color = color;

        Sequence sequence = DOTween.Sequence();
      
        Vector3 pos = mtext.transform.position;


        mtext.transform.position = pos;
        mtext.color = c;

        sequence.Append(mtext.DOColor(new Color(c.r, c.g, c.b, 1f), 1.0f))
                .Join(mtext.transform.DOMove(new Vector3(pos.x, pos.y + 20f, pos.z), 1.0f))
                .Append(mtext.DOColor(new Color(c.r, c.g, c.b, 0f), 0.5f))
                .OnComplete(() =>
                {
                    mtext.transform.position = pos;
                    mtext.color = c;

                    onComplete?.Invoke();
                });
    }
}
