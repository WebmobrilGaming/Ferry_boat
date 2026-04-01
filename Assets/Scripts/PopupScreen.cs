using System;
using DG.Tweening;
using GF;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Ferry.Popup
{
    public class PopupScreen : MonoBehaviour
    {
        public GameObject popupPanel;
        [Header("YesNo popup")]
        public GameObject yesNoPanel;
        public Button yesBtn;
        public Button noBtn;
        private Action yesAction;
        private Action noAction;
        public TMP_Text yesnoTitleTxt;
        public TMP_Text yesnoInfoTxt;
        private YesNoPopupEvent yesNoPopupEvent = null;
        [Header("Ok popup")]
        public GameObject okPanel;
        public Button okBtn;
        private Action okAction;
        public TMP_Text oktitleTxt;
        public TMP_Text okInfoTxt;
        private OkPopupEvent okPopupEvent = null;
        void OnEnable()
        {
            yesNoPanel.transform.localScale = Vector3.zero;
            okPanel.transform.localScale = Vector3.zero;
            yesBtn.AddListener(null, OnYesClick);
            noBtn.AddListener(null, OnNoClick);
            okBtn.AddListener(null, OnOkClick);
            EventManager.Instance.AddListener<YesNoPopupEvent>(OnYesNoPopup);
            EventManager.Instance.AddListener<OkPopupEvent>(OnOkPopup);
        }

        private void OnOkClick()
        {
            okPanel.transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                okAction?.Invoke();
                okAction = null;
                okPopupEvent.SetIsDone();
                okPopupEvent = null;
                popupPanel.SetActive(false);
                okPanel.SetActive(false);
            });
        }

        private void OnOkPopup(OkPopupEvent e)
        {
            okPopupEvent = e;
            okAction = e.okAction;
            oktitleTxt.text = e.title;
            okInfoTxt.text = e.info;
            popupPanel.SetActive(true);
            okPanel.SetActive(true);
            okPanel.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        }

        private void OnNoClick()
        {
            yesNoPanel.transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                noAction?.Invoke();
                noAction = null;
                CloseYesNoPanel();
            });
        }

        private void OnYesClick()
        {
            yesNoPanel.transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                yesAction?.Invoke();
                yesAction = null;
                CloseYesNoPanel();
            });
        }

        private void OnYesNoPopup(YesNoPopupEvent e)
        {
            yesNoPopupEvent = e;
            yesAction = e.yesAction;
            noAction = e.noAction;
            yesnoTitleTxt.text = e.title;
            yesnoInfoTxt.text = e.info;
            popupPanel.SetActive(true);
            yesNoPanel.SetActive(true);
            yesNoPanel.transform.DOScale(1, 1).SetEase(Ease.OutBack);
        }
        private void CloseYesNoPanel()
        {
            yesNoPopupEvent.SetIsDone();
            yesNoPopupEvent = null;
            popupPanel.SetActive(false);
            yesNoPanel.SetActive(false);
        }

        void OnDisable()
        {
            yesBtn.RemoveListener();
            noBtn.RemoveListener();
            okBtn.RemoveListener();
            EventManager.Instance.RemoveListener<YesNoPopupEvent>(OnYesNoPopup);
            EventManager.Instance.RemoveListener<OkPopupEvent>(OnOkPopup);
        }
    }
}
