using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace GF
{
    public static class ButtonClickEffect
    {
        private static float scaleDown = 0.95f;
        private static float scaleUp = 1;
        private static float duration = 0.1f;
        private static GameObject lastObj = null;
        public static void AddListener(this Button ob, ScrollRect scrollView = null, Action onClick = null)
        {
            bool draging = false;
            EventTrigger trigger = ob.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                ob.gameObject.AddComponent<EventTrigger>();
                trigger = ob.GetComponent<EventTrigger>();
            }
            if (trigger.triggers.Find(x => x.eventID == EventTriggerType.PointerDown) == null)
            {
                var onPointerDown = new EventTrigger.Entry();
                onPointerDown.eventID = EventTriggerType.PointerDown;
                onPointerDown.callback.AddListener((e) =>
                {
                    //scale down gameobject.
                    draging = false;
                    if (lastObj)
                        lastObj.transform.DOKill();
                    ob.gameObject.transform.DOKill();
                    ob.transform.DOScale(new Vector3(scaleDown, scaleDown, scaleDown), duration);
                });
                trigger.triggers.Add(onPointerDown);

                var onPointerup = new EventTrigger.Entry();
                onPointerup.eventID = EventTriggerType.PointerUp;
                onPointerup.callback.AddListener((e) =>
                {
                    //scale up gameobject.
                    if (lastObj)
                        lastObj.transform.DOKill();
                    ob.transform.DOKill();
                    ob.transform.DOScale(new Vector3(scaleUp, scaleUp, scaleUp), duration);
                    if (!draging && ob && ob.interactable)
                    {
                        var ped = e as PointerEventData;
                        var results = new System.Collections.Generic.List<RaycastResult>();
                        EventSystem.current.RaycastAll(ped, results);

                        // check if button is still under pointer
                        bool isOverButton = results.Exists(r => r.gameObject == ob.gameObject);

                        if (isOverButton || e.selectedObject == ob.gameObject)
                            onClick?.Invoke();
                    }
                });
                trigger.triggers.Add(onPointerup);

                var onBeginDrag = new EventTrigger.Entry();
                onBeginDrag.eventID = EventTriggerType.BeginDrag;
                onBeginDrag.callback.AddListener((e) =>
                {
                    //scale up gameobject.
                    draging = true;
                    if (lastObj)
                        lastObj.transform.DOKill();
                    ob.transform.DOKill();
                    ob.transform.DOScale(new Vector3(scaleUp, scaleUp, scaleUp), duration);
                    if (scrollView != null)
                        scrollView.OnBeginDrag((PointerEventData)e);
                });
                trigger.triggers.Add(onBeginDrag);

                var onDrag = new EventTrigger.Entry();
                onDrag.eventID = EventTriggerType.Drag;
                onDrag.callback.AddListener((e) =>
                {
                    draging = true;
                    if (scrollView != null)
                        scrollView.OnDrag((PointerEventData)e);
                });
                trigger.triggers.Add(onDrag);

                var onEndDrag = new EventTrigger.Entry();
                onEndDrag.eventID = EventTriggerType.EndDrag;
                onEndDrag.callback.AddListener((e) =>
                {
                    draging = false;
                    if (scrollView != null)
                        scrollView.OnEndDrag((PointerEventData)e);
                });
                trigger.triggers.Add(onEndDrag);

                var onInitialPotentialDrag = new EventTrigger.Entry();
                onInitialPotentialDrag.eventID = EventTriggerType.InitializePotentialDrag;
                onInitialPotentialDrag.callback.AddListener((e) =>
                {
                    if (scrollView != null)
                        scrollView.OnInitializePotentialDrag((PointerEventData)e);
                });
                trigger.triggers.Add(onInitialPotentialDrag);

                var onScroll = new EventTrigger.Entry();
                onScroll.eventID = EventTriggerType.Scroll;
                onScroll.callback.AddListener((e) =>
                {
                    draging = true;
                    if (scrollView != null)
                        scrollView.OnScroll((PointerEventData)e);
                });
                trigger.triggers.Add(onScroll);
            }
            lastObj = ob.gameObject;
        }
        public static void RemoveListener(this Button ob)
        {
            EventTrigger trigger = ob.GetComponent<EventTrigger>();
            if (trigger == null) return;

            // Clear stored last object
            if (lastObj == ob.gameObject)
                lastObj = null;

            trigger.triggers.Clear();
            GameObject.Destroy(trigger);

            // Reset scale just in case animation stopped midway
            ob.transform.DOKill();
            ob.transform.localScale = Vector3.one;
        }

    }
}
