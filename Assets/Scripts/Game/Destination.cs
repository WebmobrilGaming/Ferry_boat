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


    bool isScored = false;

    private void Awake()
    {
        dockCollider = GetComponent<Collider>();

    }

    private void OnEnable()
    {
        isScored = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsTargetLayer(other)) return;
        if (currentState != DockState.Outside) return;

        var boatController = other.GetComponentInParent<BoatController>();
        if (boatController == null) return;

        currentState = DockState.Entered;
        hasMissed = false;


        if(boatController.speedInKnots > 1)
        {
           // Act.ShowWarn?.Invoke($"You are crossed the Docking speed limit - > 1 mph..");
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

        if(!isScored)
        {
            Score_System.Instance.Set(10, Data.time);
            isScored = true;
        }

        Bounds dockBounds = dockCollider.bounds;
        Bounds boatBounds = other.bounds;

        bool fullyInside = dockBounds.Contains(boatBounds.min) && dockBounds.Contains(boatBounds.max);

        bool isStopped = boatController.Speed == 0 || !boatController.IsEningeActive;

        Vector3 boatForward = other.transform.forward;
        Vector3 dockForward = transform.forward;

        float dot = Vector3.Dot(boatForward.normalized, dockForward.normalized);

        DevDebug.Log($"angle difference at docking : {dot} ",DebugColor.Orange);

        fullyInside = IsBoatFullyInsideDock(boatController);

        //fullyInside = true;

        if (fullyInside)
        {
            boatController.IsEnterDock = true;
            Act.ShowWarn?.Invoke("You entered in the dock area.Please use space key to start offBoarding");
        }

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

            //Score_System.Instance.Set(-20,Data.time);
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


    private bool IsBoatFullyInsideDock(BoatController boatController)
    {
        BoxCollider dockBox = dockCollider as BoxCollider;

        if (dockBox == null)
        {
            Debug.LogError("Dock must have a BoxCollider.");
            return false;
        }

        Vector3 halfSize = dockBox.size * 0.5f;
        Vector3 min = dockBox.center - halfSize;
        Vector3 max = dockBox.center + halfSize;

        Collider[] boatColliders =
            boatController.GetComponentsInChildren<Collider>();

        bool foundCollider = false;

        foreach (Collider boatCol in boatColliders)
        {
            if (!boatCol.enabled || boatCol.isTrigger)
                continue;

            foundCollider = true;

            // Check all 8 corners of each boat collider.
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    for (int z = 0; z < 2; z++)
                    {
                        Vector3 worldPoint;

                        if (boatCol is BoxCollider box)
                        {
                            Vector3 localPoint =
                                box.center + Vector3.Scale(
                                    box.size * 0.5f,
                                    new Vector3(
                                        x == 0 ? -1 : 1,
                                        y == 0 ? -1 : 1,
                                        z == 0 ? -1 : 1
                                    )
                                );

                            worldPoint =
                                box.transform.TransformPoint(localPoint);
                        }
                        else
                        {
                            // Conservative fallback for other collider types.
                            Bounds bounds = boatCol.bounds;

                            worldPoint = new Vector3(
                                x == 0 ? bounds.min.x : bounds.max.x,
                                y == 0 ? bounds.min.y : bounds.max.y,
                                z == 0 ? bounds.min.z : bounds.max.z
                            );
                        }

                        Vector3 localPointInDock =
                            dockBox.transform.InverseTransformPoint(worldPoint);

                        if (localPointInDock.x < min.x ||
                            localPointInDock.x > max.x ||
                            localPointInDock.y < min.y ||
                            localPointInDock.y > max.y ||
                            localPointInDock.z < min.z ||
                            localPointInDock.z > max.z)
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return foundCollider;
    }


    private bool IsTargetLayer(Collider other)
    {
        return (targetLayer & (1 << other.gameObject.layer)) != 0;
    }
}