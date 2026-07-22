using DebugUtils;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PopUp : MonoBehaviour
{
    private static PopUp _instance;

    [SerializeField] CanvasGroup  canvasGroup;
    [SerializeField] TMP_Text mMessage;

    Sequence mSequence;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        canvasGroup = this.GetComponent<CanvasGroup>();
    }


    public static void Show(string message)
    {
        _instance.mSequence = DOTween.Sequence();

        _instance.mSequence.AppendCallback(()=> 
                            {                       
                               _instance.canvasGroup.DOFade(1.0f, 0.4f); 
                            })
                           .AppendCallback(() => { _instance.mMessage.text = message; })
                           .AppendInterval(2)
                           .AppendCallback(() => 
                           {
                               DevDebug.Log("Fade off",DebugColor.Grey);
                               _instance.canvasGroup.DOFade(0.0f, 0.4f);
                           });
    }
}
