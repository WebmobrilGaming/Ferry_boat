using System;
using System.Collections;
using PolyAndCode.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecyclableScrollSnap : ScrollRect
{
    [HideInInspector]
    public IRecyclableScrollRectDataSource DataSource;
    [Range(0f, 1f)]
    public float SwipeDragThreshold = 0.5f; // 0.5 = require half a cell drag

    private bool _willChangePage = false;
    private int _pendingPageIndex = -1;
    public bool IsGrid;
    public bool IsInfinite;
    public RectTransform PrototypeCell;
    public bool SelfInitialize = true;

    public enum DirectionType { Vertical, Horizontal }
    public DirectionType Direction;

    [SerializeField] private int _segments;
    public int Segments
    {
        set { _segments = Math.Max(value, 2); }
        get { return _segments; }
    }

    public int CurrentPageIndex { get; set; } = 0;

    private RecyclingSystem _recyclingSystem;
    private Vector2 _prevAnchoredPos;
    private int previousIndex = 0;

    // --- 🚀 New swipe direction enum & event ---
    public enum SwipeDirection { None, Left, Right, Up, Down }
    private Vector2 _dragStartPos;
    private Vector2 _dragEndPos;
    private SwipeDirection _lastSwipe = SwipeDirection.None;
    private Vector2 lastContentPosition;
    protected override void Start()
    {
        vertical = true;
        horizontal = false;
        lastContentPosition = content.anchoredPosition;
        if (!Application.isPlaying) return;
        if (SelfInitialize) Initialize();
    }

    private void Initialize()
    {
        //Destroy old cells
        var childrens = content.GetComponentsInChildren<ICell>();
        foreach (var cell in childrens)
        {
            Destroy(cell.GetGameObject());
        }

        // Construct recycling system
        if (Direction == DirectionType.Vertical)
        {
            _recyclingSystem = new VerticalRecyclingSystem(PrototypeCell, viewport, content, DataSource, IsGrid, Segments);
        }
        else if (Direction == DirectionType.Horizontal)
        {
            _recyclingSystem = new HorizontalRecyclingSystem(PrototypeCell, viewport, content, DataSource, IsGrid, Segments);
        }

        _recyclingSystem.IsInfinite = IsInfinite;
        vertical = Direction == DirectionType.Vertical;
        horizontal = Direction == DirectionType.Horizontal;

        _prevAnchoredPos = content.anchoredPosition;
        onValueChanged.RemoveListener(OnValueChangedListener);

        // Add recycling listener after pool init
        StartCoroutine(_recyclingSystem.InitCoroutine(() =>
            onValueChanged.AddListener(OnValueChangedListener)
        ));
    }

    public void Initialize(IRecyclableScrollRectDataSource dataSource)
    {
        DataSource = dataSource;
        Initialize();
    }

    public void OnValueChangedListener(Vector2 normalizedPos)
    {
        Vector2 dir = content.anchoredPosition - _prevAnchoredPos;
        m_ContentStartPosition += _recyclingSystem.OnValueChangedListener(dir);
        _prevAnchoredPos = content.anchoredPosition;
    }

    public void ReloadData()
    {
        ReloadData(DataSource);
    }

    public void ReloadData(IRecyclableScrollRectDataSource dataSource)
    {
        if (_recyclingSystem != null)
        {
            StopMovement();
            onValueChanged.RemoveListener(OnValueChangedListener);
            _recyclingSystem.DataSource = dataSource;
            StartCoroutine(_recyclingSystem.InitCoroutine(() =>
                onValueChanged.AddListener(OnValueChangedListener)
            ));
            _prevAnchoredPos = content.anchoredPosition;
        }
    }

    // ===========================
    // 🚀 SNAPPING + SWIPE
    // ===========================

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        _dragStartPos = eventData.position;   // record drag start
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        _dragEndPos = eventData.position;     // record drag end

        Vector2 delta = _dragEndPos - _dragStartPos;
        _lastSwipe = GetSwipeDirection(delta);   // record drag end

        // Decide swipe direction
        SnapToNearestCell();
    }

    private SwipeDirection GetSwipeDirection(Vector2 delta)
    {
        if (delta.magnitude < 20f) return SwipeDirection.None; // deadzone

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            return delta.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
        else
            return delta.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
    }

    private void SnapToNearestCell()
    {
        if (content.childCount == 0) return;

        RectTransform cell = content.GetChild(0) as RectTransform;
        float cellWidth = cell.sizeDelta.x;
        float cellHeight = cell.sizeDelta.y;

        Vector2 targetPos = content.anchoredPosition;
        int index = 0;

        if (vertical)
        {
            float offset = -content.anchoredPosition.y;
            index = Mathf.RoundToInt(offset / cellHeight);
            index = Mathf.Clamp(index, 0, DataSource.GetItemCount() - 1);
            targetPos.y = -index * cellHeight;
        }
        else if (horizontal)
        {
            float offset = -content.anchoredPosition.x;
            index = Mathf.RoundToInt(offset / cellWidth);
            index = Mathf.Clamp(index, 0, DataSource.GetItemCount() - 1);
            targetPos.x = -index * cellWidth;
        }
        StopMovement();
        onValueChanged.RemoveListener(OnValueChangedListener);
        StartCoroutine(SnapCoroutine(content.anchoredPosition, targetPos, 0.25f));
    }

    private IEnumerator SnapCoroutine(Vector2 startPos, Vector2 endPos, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            content.anchoredPosition = Vector2.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        content.anchoredPosition = endPos;
        if (lastContentPosition != endPos)
        {

            if (horizontal)
            {
                if (_lastSwipe == SwipeDirection.Left)
                {
                    CurrentPageIndex++;
                }
                else if (_lastSwipe == SwipeDirection.Right)
                {
                    CurrentPageIndex--;
                }
            }
            else
            {
                // if (_lastSwipe == SwipeDirection.Left)
                // {
                //     CurrentPageIndex++;
                // }
                // else if(_lastSwipe==SwipeDirection.Right)
                // {
                //     CurrentPageIndex--;
                // }
            }
            CurrentPageIndex = Math.Clamp(CurrentPageIndex, 0, DataSource.GetItemCount());
            // Resume recycling
            DataSource.PageChanged(CurrentPageIndex);
            lastContentPosition = endPos;
        }
        onValueChanged.AddListener(OnValueChangedListener);
    }
}
