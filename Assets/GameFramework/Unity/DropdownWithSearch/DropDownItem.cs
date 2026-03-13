using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GF
{
    [RequireComponent(typeof(Button))]
    public class DropDownItem : MonoBehaviour,ICell
    {
        public Image bg;
        public TMP_Text label;
        public string Lable => label.text;
        private Button btn;
        private int index;
        void Awake()
        {
            btn = GetComponent<Button>();
        }
        public void Initialize((string, Sprite) value, ScrollRect scrollRect, int index,Action<int> SelectAction)
        {
            this.index=index;
            btn.AddListener(scrollRect, () => SelectAction?.Invoke(this.index));
            this.label.text = value.Item1;
            this.bg.sprite = value.Item2;
        }
        public void Initialize((string, Sprite) value, Action selectAction)
        {
            btn.AddListener(null, selectAction);
            this.label.text = value.Item1;
            this.bg.sprite = value.Item2;
        }
        public void SetItem((string, Sprite) value)
        {
            this.label.text = value.Item1;
            this.bg.sprite = value.Item2;
        }
        void OnDestroy()
        {
            btn.RemoveListener();
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}
