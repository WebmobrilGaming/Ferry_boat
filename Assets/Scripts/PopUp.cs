using Cysharp.Threading.Tasks;
using DebugUtils;
using DG.Tweening;
using FerryBoat.Actions;
using TMPro;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    private static PopUp _instance;

    [SerializeField] CanvasGroup  canvasGroup;

    [SerializeField] GameObject mPanel;
    [SerializeField] TMP_Text mMessage;

    Sequence mSequence;

    public static PopUp Instance {  get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            return;
        }

        _instance = this;
    }

    private void OnEnable()
    {
        Act.ShowWarn += Show;
    }

    private void OnDisable()
    {
        Act.ShowWarn -= Show;
    }


    public void Show(string message)
    {

        if (mPanel == null)
        {
            //Debug.LogError($"mPanel is NULL on {gameObject.name}");
            return;
        }

        if (mMessage == null)
        {
            Debug.LogError($"mMessage is NULL on {gameObject.name}");
            return;
        }

        Debug.Log($"Show() called on {gameObject.name}");


        mPanel.SetActive(true);
        mMessage.text = message;

        ClosePop(2f);
    }

    async UniTask ClosePop(float delay)
    {
        await UniTask.Delay((int)delay * 1000);
        mPanel.SetActive(false);
    }
}
