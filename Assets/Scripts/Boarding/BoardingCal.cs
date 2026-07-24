using System;
using FerryBoat.Actions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public interface IBoardCal
{
    public void UpdateBoarding(BoardCharType boardCharType, bool onBoard);
}

public class BoardingCal : MonoBehaviour
{
    [SerializeField] Button mPlusBtn;
    [SerializeField] Button mMinusBtn;

    int count;
    [SerializeField] TMP_Text mP_Text;

    [Space]
    [SerializeField] BoardCharType boardCharType;

    public IBoardCal callback;

    [SerializeField] TextAnimator textAnimator;
    [SerializeField] bool maximumBoardTimeReached;
 
    private void OnEnable()
    {
        count = 0;
        maximumBoardTimeReached = false;
        mMinusBtn.interactable = false;
        UpdateText(count);
        mPlusBtn?.onClick.AddListener(() =>
        {
            textAnimator.ResetAction();

            mPlusBtn.interactable = false;

            count++;
            UpdateText(count);

            textAnimator.Animate($" + {boardCharType.ToString()}", Color.green, () =>
            {
                if(!maximumBoardTimeReached)
                {
                    mPlusBtn.interactable = true;
                }
               
            });
            callback?.UpdateBoarding(boardCharType, true);
            if (count > 0)
            {
                mMinusBtn.interactable = true;
            }
        });

        mMinusBtn?.onClick.AddListener(() =>
        {
            textAnimator.ResetAction();

            mMinusBtn.interactable = false;
            count--;
            if(maximumBoardTimeReached == true)
            {
                mPlusBtn.interactable = true;
                maximumBoardTimeReached = false;
            }
           

            if(count <= 0)
                count = 0;

            UpdateText(count);

            textAnimator.Animate($" - {boardCharType.ToString()}", Color.red, () =>
            {
                if (count != 0)
                {
                    mMinusBtn.interactable = true;
                }
                
            });

            callback?.UpdateBoarding(boardCharType, false);
            
        });
    }


    void UpdateText(int count)
    {
        mP_Text.text = boardCharType switch
        {
            BoardCharType.passenger => $"<mspace=0.6em>{"Passenger",-12} : {count,8}</mspace>",
            BoardCharType.car => $"<mspace=0.6em>{"Cars",-12} : {count,8}</mspace>",
            BoardCharType.truck => $"<mspace=0.6em>{"Trucks",-12} : {count,8}</mspace>",
        };
    }

    private void OnDisable()
    {
        mPlusBtn?.onClick.RemoveAllListeners();
        mMinusBtn?.onClick.RemoveAllListeners();
    }
}
