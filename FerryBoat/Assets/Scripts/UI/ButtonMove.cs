using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ButtonMove : MonoBehaviour
{
    [SerializeField] private RectTransform[] mButtons;
    void OnEnable()
    {
        ButtonMoveF();
    }
    void Start()
    {
       
    }


    public void ButtonMoveF()
    {
        Sequence seq = DOTween.Sequence();
        for(int i = mButtons.Length - 1; i>=0; i--)
        {
            seq.Append(mButtons[i].DOAnchorPosX(mButtons[i].anchoredPosition.x + 700f,0.2f).SetEase(Ease.OutQuad));
            
        }  
        
    }
    
}
