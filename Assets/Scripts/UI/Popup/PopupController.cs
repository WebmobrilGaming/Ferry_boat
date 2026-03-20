using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupController : MonoBehaviour
{
    public static PopupController Instance
    {
        get { return _instance; }
    }
    private static PopupController _instance;

    [Header("Popup Root")]
    public GameObject popup;

    [Header("UI References")]
    public TMP_Text body;
    public Button okBtn;
    public Button yesBtn;
    public Button noBtn;

    private Action completeAction;
    private Action yesAction;
    private Action noAction;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        okBtn.onClick.AddListener(OnOkClicked);
        yesBtn.onClick.AddListener(OnYesClicked);
        noBtn.onClick.AddListener(OnNoClicked);
    }

    private void OnOkClicked()
    {
        popup.SetActive(false);
        completeAction?.Invoke();
    }

    private void OnYesClicked()
    {
        popup.SetActive(false);
        yesAction?.Invoke();
    }

    private void OnNoClicked()
    {
        popup.SetActive(false);
        noAction?.Invoke();
    }

    public void ShowOk(string bodyText, Action onOk = null)
    {
        SetupPopup(bodyText);
        okBtn.gameObject.SetActive(true);
        yesBtn.gameObject.SetActive(false);
        noBtn.gameObject.SetActive(false);
        completeAction = onOk;
    }

    public void ShowYesNo(string bodyText, Action onYes, Action onNo = null)
    {
        SetupPopup(bodyText);
        okBtn.gameObject.SetActive(false);
        yesBtn.gameObject.SetActive(true);
        noBtn.gameObject.SetActive(true);
        yesAction = onYes;
        noAction = onNo;
    }

    public void ShowExitConfirmation()
    {
        ShowYesNo(
            "Do you want to exit the game?",
            onYes: () =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        );
    }

    public void ShowInvalidUsername()
    {
        ShowOk("This username does not exist. Please try again.");
    }

    public void ShowFillAllFields()
    {
        ShowOk("Please fill in all the required fields.");
    }

    public void ShowUsernameAlreadyExists()
    {
        ShowOk("This username already exists. Please choose another.");
    }

    private void SetupPopup(string bodyText)
    {
        popup.SetActive(true);
        popup.transform.localScale = Vector3.zero;
        popup.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        body.text = bodyText;
    }

    private void OnDestroy()
    {
        okBtn.onClick.RemoveAllListeners();
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();
    }
}