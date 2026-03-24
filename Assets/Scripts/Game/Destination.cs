using DebugUtils;
using FerryBoat;
using UnityEngine;

public class Destination : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    private Collider dockCollider;
    private bool isReached = false;
    private void Awake()
    {
        dockCollider = GetComponent<Collider>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (isReached) return;

        if ((targetLayer & (1 << other.gameObject.layer)) != 0)
        {
            Bounds dockBounds = dockCollider.bounds;
            Bounds boatBounds = other.bounds;

            if (dockBounds.Contains(boatBounds.min) && dockBounds.Contains(boatBounds.max))
            {
                var boatController = other.GetComponentInParent<BoatController>();
                if (boatController != null && boatController.Speed == 0 && !boatController.IsEningeActive)
                {
                    isReached = true;
                    DevDebug.Log("Boat fully inside dock!", DebugColor.Green);
                    Act.ReachedDestination?.Invoke();
                    boatController.IsInDock=true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((targetLayer & (1 << other.gameObject.layer)) != 0)
        {
            isReached = false;
            var boatController = other.GetComponentInParent<BoatController>();
            if (boatController != null && boatController.Speed == 0 && !boatController.IsEningeActive)
            {
                boatController.IsInDock=false;
            }
        }
    }
}
