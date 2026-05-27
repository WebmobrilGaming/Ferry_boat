using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
namespace GF
{
    public class ToggleBtn : MonoBehaviour
    {
        public Image dotImg;
        private RectTransform dotRect;
        private bool isOff=false;
        public bool IsOn => isOff;
        private Button button;
        public float startX;
        public float endX;
        private Color defaultColor;
        private Action<bool> action;
        [SerializeField] private string key;

        void Awake()
        {
            defaultColor = dotImg.color;
            button = GetComponent<Button>();
            dotRect = dotImg.GetComponent<RectTransform>();
        }
        void OnEnable()
        {
            
            DotPositionOnEnable();
            button.onClick.AddListener(OnToggle);

        }
        public void AddListener(Action<bool> action)
        {
            this.action = action;
        }
        private void OnToggle()
        {
            isOff = !isOff;
            if (!isOff)
            {
                dotRect.DOAnchorPosX(endX, 0.3f).SetEase(Ease.OutBack);
                SaveDotPosition(endX);
                dotImg.color = Color.green;
                this.action?.Invoke(true);
            }
            else
            {
                dotRect.DOAnchorPosX(startX, 0.3f).SetEase(Ease.OutBack);
                SaveDotPosition(startX);
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
        public void SaveDotPosition(float value)
        {
            RectTransform rect = dotRect;
            PlayerPrefs.SetFloat(key, value);
            PlayerPrefs.Save();
        }

        public void DotPositionOnEnable()
        {
            float x = dotRect.anchoredPosition.x;
            float y = dotRect.anchoredPosition.y;
            float savedX = PlayerPrefs.GetFloat(key,endX);
            dotRect.anchoredPosition = new Vector2(PlayerPrefs.GetFloat(key,endX),y);
            isOff = savedX == startX;
            if(dotRect.anchoredPosition.x == endX)
            {
                dotImg.color = Color.green;
            }
        }
        
    }
}
