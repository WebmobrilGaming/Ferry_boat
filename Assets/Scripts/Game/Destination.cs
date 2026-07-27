using System;
using DebugUtils;
using FerryBoat.Actions;
using FerryBoat.Store;
using GF;
using UnityEngine;

public class Destination : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    private Collider dockCollider;

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
        Act.ShowWarn?.Invoke("You entered in the dock area");

        if(boatController.speedInKnots > 1)
        {
            Act.ShowWarn?.Invoke($"You are crossed the Docking speed limit - > 1 mph..");
            boatController.score_System.Set(-20,Data.time);
        }
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

        bool isStopped = boatController.Speed == 0 || !boatController.IsEningeActive;

        Vector3 boatForward = other.transform.forward;
        Vector3 dockForward = transform.forward;

        float dot = Vector3.Dot(boatForward.normalized, dockForward.normalized);

        DevDebug.Log($"angle difference at docking : {dot} ",DebugColor.Orange);

        fullyInside = dot > 0.9f;

       // fullyInside = true;

        if (fullyInside && isStopped)
        {
            currentState = DockState.FullyDocked;
            boatController.IsInDock = true;

            DevDebug.Log("Boat fully inside dock!", DebugColor.Green);

            Act.EnableUser?.Invoke(false);
            Act.ReachedDestination?.Invoke();

            Act.OffBoardAction?.Invoke();

            return;
        }

        if (!fullyInside && isStopped && !hasMissed)
        {
            hasMissed = true;

            DevDebug.Log("Ferry missed the dock!", DebugColor.Red);
            Act.ShowWarn?.Invoke("You missed the dock");

            Score_System.Instance.Set(-20,Data.time);
        }

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