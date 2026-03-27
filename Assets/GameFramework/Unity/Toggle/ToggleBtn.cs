using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
namespace GF
{
    public class ToggleBtn : MonoBehaviour
    {
        public Image dotImg;
        private RectTransform dotRect;
        private bool isOn = false;
        public bool IsOn => isOn;
        private Button button;
        public float startX;
        public float endX;
        private Color defaultColor;
        private Action<bool> action;

        void Awake()
        {
            defaultColor = dotImg.color;
            button = GetComponent<Button>();
            dotRect = dotImg.GetComponent<RectTransform>();
        }
        void OnEnable()
        {
            button.onClick.AddListener(OnToggle);
        }
        public void AddListener(Action<bool> action)
        {
            this.action = action;
        }
        private void OnToggle()
        {
            isOn = !isOn;
            if (isOn)
            {
                dotRect.DOAnchorPosX(endX, 0.3f).SetEase(Ease.OutBack);
                dotImg.color = Color.green;
                this.action?.Invoke(true);
            }
            else
            {
                dotRect.DOAnchorPosX(startX, 0.3f).SetEase(Ease.OutBack);
                dotImg.color = defaultColor;
                this.action?.Invoke(false);
            }
        }
        public void RemoveListener()
        {
            this.action = null;
        }
        void OnDisable()
        {
            button.onClick.RemoveListener(OnToggle);
        }
    }
}
