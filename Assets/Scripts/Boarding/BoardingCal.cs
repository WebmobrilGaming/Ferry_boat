using TMPro;
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
 
    private void OnEnable()
    {
        count = 0;
        UpdateText(count);

        mPlusBtn?.onClick.AddListener(() =>
        {
            mPlusBtn.interactable = false;

            count++;
            UpdateText(count);

            textAnimator.Animate($" + {boardCharType.ToString()}", Color.green, () =>
            {
                mPlusBtn.interactable = true;
            });
            callback?.UpdateBoarding(boardCharType, true);
        });

        mMinusBtn?.onClick.AddListener(() =>
        {
            mMinusBtn.interactable = false;
            count--;

            if(count <= 0)
                count = 0;

            UpdateText(count);

            textAnimator.Animate($" - {boardCharType.ToString()}", Color.red, () =>
            {
                mMinusBtn.interactable = true;
            });
            callback?.UpdateBoarding(boardCharType, false);
        });
    }

    void UpdateText(int count)
    {
        mP_Text.text = boardCharType switch
        {
            BoardCharType.passenger => $"Passenger:{count}",
            BoardCharType.car => $"Cars:{count}",
            BoardCharType.truck => $"Trucks:{count}"
        };
    }

    private void OnDisable()
    {
        mPlusBtn?.onClick.RemoveAllListeners();
        mMinusBtn?.onClick.RemoveAllListeners();
    }
}
