using System.Collections.Generic;
using System.Linq;
using PolyAndCode.UI;
using TMPro;
using UnityEngine;

namespace GF
{
    public class DropdownController : MonoBehaviour, IRecyclableScrollRectDataSource
    {
        public RecyclableScrollRect recyclableScrollRect;
        public TMP_InputField searchInput;
        private List<(string, Sprite)> dropdownList = new List<(string, Sprite)>();
        private List<(string, Sprite)> searchDropDownList = new List<(string, Sprite)>();
        public DropDownItem dropDownItem;
        public int LastSeenIndex { get; private set; }
        private DropDownSearchTrie searchTrie;
        void Awake()
        {
            searchTrie = new DropDownSearchTrie();
            searchInput.onValueChanged.AddListener(OnType);
        }

        private void OnType(string search)
        {
            if (string.IsNullOrEmpty(search) || string.IsNullOrWhiteSpace(search))
            {
                searchDropDownList = this.dropdownList;
            }
            else
            {
                var searchedList = searchTrie.Search(search).ToHashSet();
                searchDropDownList = dropdownList.Where(e => searchedList.Contains(e.Item1)).ToList();
            }
            Invoke(nameof(InitializeScrollView), 0.2f);
        }

        public void Initialize(List<(string, Sprite)> dropdownList)
        {
            gameObject.SetActive(true);
            this.dropdownList = dropdownList;
            searchDropDownList = this.dropdownList;
            foreach (var item in dropdownList)
                searchTrie.Insert(item.Item1);
            searchInput.text = null;
            Invoke(nameof(InitializeScrollView), 0.2f);
        }
        private void InitializeScrollView()
        {
            recyclableScrollRect.Initialize(this);
        }
        public int GetItemCount()
        {
            return searchDropDownList.Count;
        }

        public int GetStartingIndex()
        {
            if (LastSeenIndex >= searchDropDownList.Count || LastSeenIndex <= 0)
            {
                LastSeenIndex = 0;
            }
            return LastSeenIndex;
        }

        public void PageChanged(int index)
        {

        }

        public void SetCell(ICell cell, int index)
        {
            var dropDownItem = cell as DropDownItem;
            dropDownItem.Initialize(searchDropDownList[index], recyclableScrollRect, index, OnSelectItem);
        }

        private void OnSelectItem(int index)
        {
            LastSeenIndex = GetGlobalIndex(index);
            searchTrie.Clear();
            dropDownItem.SetItem(searchDropDownList[index]);
            gameObject.SetActive(false);
        }
        private int GetGlobalIndex(int localIndex)
        {
            var item=searchDropDownList[localIndex];
            return dropdownList.IndexOf(item);
        }
        void OnDestroy()
        {
            searchInput.onValueChanged.RemoveListener(OnType);
        }
    }
}
