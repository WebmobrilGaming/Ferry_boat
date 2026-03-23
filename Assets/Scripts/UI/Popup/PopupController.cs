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
    private Image overlay;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        CreateOverlay();

        okBtn.onClick.AddListener(OnOkClicked);
        yesBtn.onClick.AddListener(OnYesClicked);
        noBtn.onClick.AddListener(OnNoClicked);
    }

    private void CreateOverlay()
    {
       
        GameObject overlayGO = new GameObject("Overlay");

        overlayGO.transform.SetParent(popup.transform.parent, false);

       
        overlayGO.transform.SetSiblingIndex(popup.transform.GetSiblingIndex());

       
        overlay = overlayGO.AddComponent<Image>();
        overlay.color = new Color(0, 0, 0, 0);

        
        RectTransform rt = overlayGO.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

      
        overlayGO.SetActive(false);
    }

    private void OnOkClicked()
    {
        HidePopup(() => completeAction?.Invoke());
    }

    private void OnYesClicked()
    {
        HidePopup(() => yesAction?.Invoke());
    }

    private void OnNoClicked()
    {
        HidePopup(() => noAction?.Invoke());
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
        ShowOk("This username does not exist.\nPlease try again.");
    }

    public void ShowFillAllFields()
    {
        ShowOk("Please fill in all\nthe required fields.");
    }

    public void ShowUsernameAlreadyExists()
    {
        ShowOk("This username already exists.\nPlease choose another.");
    }

    private void SetupPopup(string bodyText)
    {
        body.text = bodyText;

       
        overlay.gameObject.SetActive(true);
        overlay.color = new Color(0.05f, 0.05f, 0.05f, 0.05f);
        overlay.DOFade(0.85f, 0.2f);  

       
        popup.SetActive(true);
        popup.transform.localScale = Vector3.zero;
        popup.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }

    private void HidePopup(Action onComplete)
    {
       
        popup.transform.DOScale(0, 0.2f);

       
        overlay.DOFade(0, 0.2f).OnComplete(() =>
        {
            popup.SetActive(false);
            overlay.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    private void OnDestroy()
    {
        okBtn.onClick.RemoveAllListeners();
        yesBtn.onClick.RemoveAllListeners();
        noBtn.onClick.RemoveAllListeners();
    }
}