using System;
using DebugUtils;
using FerryBoat;
using UnityEngine;

public class Destination : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    private Collider dockCollider;

    public static event Action OnFerryMissedDockEvent;
    public static event Action OnEnterDockEvent;

    private enum DockState { Outside, Entered, FullyDocked }
    private DockState currentState = DockState.Outside;

    private bool hasMissed = false; // prevents spam


    private void Awake()
    {
        dockCollider = GetComponent<Collider>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsTargetLayer(other)) return;
        if (currentState != DockState.Outside) return;

        var boatController = other.GetComponentInParent<BoatController>();
        if (boatController == null) return;

        currentState = DockState.Entered;
        hasMissed = false;

        boatController.IsEnterDock = true;
        OnEnterDockEvent?.Invoke();

        DevDebug.Log("Ferry entered dock area.", DebugColor.Yellow);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsTargetLayer(other)) return;
        if (currentState == DockState.Outside || currentState == DockState.FullyDocked) return;

        var boatController = other.GetComponentInParent<BoatController>();
        if (boatController == null) return;

        Bounds dockBounds = dockCollider.bounds;
        Bounds boatBounds = other.bounds;

        bool fullyInside = dockBounds.Contains(boatBounds.min) && dockBounds.Contains(boatBounds.max);

        bool isStopped = boatController.Speed == 0 && !boatController.IsEningeActive;

        // ✅ SUCCESS CASE
        if (fullyInside && isStopped)
        {
            currentState = DockState.FullyDocked;
            boatController.IsInDock = true;

            DevDebug.Log("Boat fully inside dock!", DebugColor.Green);
            Act.ReachedDestination?.Invoke();
            return;
        }

        // ✅ MISS CASE (only once per stop)
        if (!fullyInside && isStopped && !hasMissed)
        {
            hasMissed = true;

            DevDebug.Log("Ferry missed the dock!", DebugColor.Red);
            OnFerryMissedDockEvent?.Invoke();
        }

        // ✅ RESET miss when boat starts moving again (allows retry)
        if (!isStopped)
        {
            hasMissed = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsTargetLayer(other)) return;

        var boatController = other.GetComponentInParent<BoatController>();
        if (boatController != null)
        {
            boatController.IsInDock = false;
            boatController.IsEnterDock = false;
        }

        currentState = DockState.Outside;
        hasMissed = false;

        DevDebug.Log("Ferry left dock area.", DebugColor.Yellow);
    }

    private bool IsTargetLayer(Collider other)
    {
        return (targetLayer & (1 << other.gameObject.layer)) != 0;
    }
}