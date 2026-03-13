using SSR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SSR
{
    public class ScrollSelector : MonoBehaviour
    {
        private int _selectorPageIndex;
        private ScrollSnapRect _scrollSnapInstance;
        private Button button;
        private GameObject _selector;
        private void Awake()
        {
            _selector = transform.GetChild(0).gameObject;
            button = transform.GetChild(1).GetComponent<Button>();
            button.onClick.AddListener(() => SelectPage());
            SetSelector(false);
        }
        public void SetScrollSnapInstance(ScrollSnapRect scroll_snap)
        {
            _scrollSnapInstance = scroll_snap;
        }
        public void SetPageIndex(int index)
        {
            _selectorPageIndex = index;
        }
        public void SetSelector(bool status)
        {
            _selector.SetActive(status);
        }
        private void SelectPage()
        {
            Debug.Log("Page-" + _selectorPageIndex);
            _scrollSnapInstance.LerpToPage(_selectorPageIndex);
        }
    }
}