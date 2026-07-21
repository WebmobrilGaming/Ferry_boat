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
                               _instance.canvasGroup.blocksRaycasts = true;
                               _instance.canvasGroup.interactable = true;
                                                   
                               _instance.canvasGroup.DOFade(1.0f, 0.4f); 
                            })
                           .AppendCallback(() => { _instance.mMessage.text = message; })
                           .AppendInterval(2000)
                           .AppendCallback(() => 
                           {
                               _instance.canvasGroup.blocksRaycasts = false;
                               _instance.canvasGroup.interactable = false;

                               _instance.canvasGroup.DOFade(0.0f, 0.4f);
                           });
    }
}
