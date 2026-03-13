//MIT License
//Copyright (c) 2020 Mohammed Iqubal Hussain
//Website : Polyandcode.com 


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace PolyAndCode.UI
{
    /// <summary>
    /// Absract Class for creating a Recycling system.
    /// </summary>
    public abstract class RecyclingSystem
    {
        public IRecyclableScrollRectDataSource DataSource;
        public bool IsInfinite { get; set; }
        protected RectTransform Viewport, Content;
        protected RectTransform PrototypeCell;
        protected bool IsGrid;
        protected int currentItemCount=0;
        protected float MinPoolCoverage = 1.5f; // The recyclable pool must cover (viewPort * _poolCoverage) area.
        protected int MinPoolSize = 10; // Cell pool must have a min size
        protected float RecyclingThreshold = .2f; //Threshold for recycling above and below viewport
        public RecyclingSystem(IRecyclableScrollRectDataSource dataSource)
        {
            DataSource = dataSource;
            currentItemCount=DataSource.GetStartingIndex();
        }
        public abstract IEnumerator InitCoroutine(System.Action onInitialized = null);
        public abstract Vector2 OnValueChangedListener(Vector2 direction); 
        private int Increment(int currentIndex, int MinIndex, int MaxIndex)
        {
            currentIndex %= MaxIndex;
            return currentIndex;
        }
        private int Decrement(int currentIndex, int MinIndex, int MaxIndex)
        {
            currentIndex = currentIndex % (MaxIndex + 1);
            if (currentIndex < MinIndex)
            {
                currentIndex += (MaxIndex + 1);
            }
            return currentIndex;
        }
        protected int ClampIndex(int index)
        {
            if (index > DataSource.GetItemCount() - 1)
            {
                index = Increment(index, 0, DataSource.GetItemCount());
            }
            else if (index < 0)
            {
                index = Decrement(index, 0, DataSource.GetItemCount() - 1);
            }
            return index;
        }
    }
}