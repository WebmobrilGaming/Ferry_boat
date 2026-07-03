using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Cysharp.Threading.Tasks;

public class GamePopUp : MonoBehaviour
{
    static GamePopUp instance;

    public static GamePopUp Instance {  get { return instance; } }

    [SerializeField] GameObject panelFinal;
    [SerializeField] TMP_Text mFinalText;

    [SerializeField] GameObject panelStat;
    [SerializeField] TMP_Text mStatText;

    [SerializeField] GameObject panelYesNo;
    [SerializeField] TMP_Text mYesNoText;
    [SerializeField] Button yesBtn;
    [SerializeField] Button noBtn;
     

    bool isPanel = false;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void OnEnable()
    {
        yesBtn.interactable = true;
        noBtn.interactable = true;
    }
    public void FinalPopUp(string message)
    {
        panelFinal.SetActive(true);
        mFinalText.text = message;
    }

    public void PopStat(string message,float delay,Action onComplete = null)
    {
        Pop(delay, message);
    }

    async UniTask Pop(float delay ,string message, Action onComplete = null)
    {
        await UniTask.WaitUntil(() => !isPanel);

        isPanel = true;

        panelStat.SetActive(true);
        mStatText.text = message;

        await UniTask.Delay((int)delay*1000);
        panelStat.SetActive(false);

        isPanel = false;

        onComplete?.Invoke();
    }


    public void PopControl(string message, Action yesAct, Action noAct)
    {
        panelYesNo.SetActive(true);

        mYesNoText.text = message;

        yesBtn?.onClick.RemoveAllListeners();
        noBtn?.onClick.RemoveAllListeners();

        yesBtn?.onClick.AddListener(() =>
        {
            yesBtn.interactable = false;
            Time.timeScale=1;
            yesAct?.Invoke();
        });

        noBtn?.onClick.AddListener(() =>
        {
            
            panelYesNo.gameObject.SetActive(false);
            Time.timeScale = 1;
            noAct?.Invoke();
        });
    }

    public void ClosePanel()
    {
        panelFinal.SetActive(false);
    }
}
