using System;
using System.Collections;
using DebugUtils;
using FerryBoat;
using UnityEngine;

public class Destination : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float missedDockDelay = 3f; // seconds before firing missed event

    private Collider dockCollider;

    public static event Action OnFerryMissedDockEvent;
    public static event Action OnEnterDockEvent;

    private enum DockState { Outside, Entered, FullyDocked, Missed }
    private DockState currentState = DockState.Outside;

    private Coroutine missedDockCoroutine;

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
        boatController.IsEnterDock = true;
        OnEnterDockEvent?.Invoke();
        DevDebug.Log("Ferry entered dock area.", DebugColor.Yellow);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsTargetLayer(other)) return;
        if (currentState != DockState.Entered) return;

        var boatController = other.GetComponentInParent<BoatController>();
        if (boatController == null) return;

        Bounds dockBounds = dockCollider.bounds;
        Bounds boatBounds = other.bounds;
        bool fullyInside = dockBounds.Contains(boatBounds.min) && dockBounds.Contains(boatBounds.max);
        bool hasStopped = boatController.Speed == 0 && !boatController.IsEningeActive;

        if (fullyInside && hasStopped)
        {
            // Docked successfully — cancel any pending missed timer
            CancelMissedTimer();
            currentState = DockState.FullyDocked;
            boatController.IsInDock = true;
            DevDebug.Log("Boat fully inside dock!", DebugColor.Green);
            Act.ReachedDestination?.Invoke();
        }
        else if (!fullyInside && hasStopped)
        {
            // Ferry stopped outside — start missed timer if not already running
            if (missedDockCoroutine == null)
            {
                missedDockCoroutine = StartCoroutine(MissedDockAfterDelay());
                DevDebug.Log($"Ferry may have missed dock. Waiting {missedDockDelay}s...", DebugColor.Yellow);
            }
        }
        else
        {
            // Ferry still moving — cancel missed timer, it might still correct
            CancelMissedTimer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsTargetLayer(other)) return;

        CancelMissedTimer();

        var boatController = other.GetComponentInParent<BoatController>();
        if (boatController != null)
        {
            boatController.IsInDock = false;
            boatController.IsEnterDock = false;
        }

        currentState = DockState.Outside;
        DevDebug.Log("Ferry left dock area.", DebugColor.Yellow);
    }

    private IEnumerator MissedDockAfterDelay()
    {
        yield return new WaitForSeconds(missedDockDelay);

        // Re-check state hasn't resolved in the meantime
        if (currentState == DockState.Entered)
        {
            currentState = DockState.Missed;
            missedDockCoroutine = null;
            DevDebug.Log("Ferry missed the dock!", DebugColor.Red);
            OnFerryMissedDockEvent?.Invoke();
        }
    }

    private void CancelMissedTimer()
    {
        if (missedDockCoroutine != null)
        {
            StopCoroutine(missedDockCoroutine);
            missedDockCoroutine = null;
            DevDebug.Log("Missed dock timer cancelled.", DebugColor.Yellow);
        }
    }

    private bool IsTargetLayer(Collider other)
    {
        return (targetLayer & (1 << other.gameObject.layer)) != 0;
    }
}