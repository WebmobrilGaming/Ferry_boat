using System;
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
        private OkPopupEvent okPopupEvent=null;
        void OnEnable()
        {
            yesBtn.AddListener(null, OnYesClick);
            noBtn.AddListener(null, OnNoClick);
            okBtn.AddListener(null,OnOkClick);
            EventManager.Instance.AddListener<YesNoPopupEvent>(OnYesNoPopup);
            EventManager.Instance.AddListener<OkPopupEvent>(OnOkPopup);
        }

        private void OnOkClick()
        {
            okAction?.Invoke();
            okAction=null;
            okPopupEvent.SetIsDone();
            okPopupEvent=null;
            popupPanel.SetActive(false);
            okPanel.SetActive(false);
        }

        private void OnOkPopup(OkPopupEvent e)
        {
            okPopupEvent=e;
            okAction=e.okAction;
            oktitleTxt.text=e.title;
            okInfoTxt.text=e.info;
            popupPanel.SetActive(true);
            okPanel.SetActive(true);
        }

        private void OnNoClick()
        {
            noAction?.Invoke();
            noAction = null;
            CloseYesNoPanel();
        }

        private void OnYesClick()
        {
            yesAction?.Invoke();
            yesAction = null;
            CloseYesNoPanel();
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
