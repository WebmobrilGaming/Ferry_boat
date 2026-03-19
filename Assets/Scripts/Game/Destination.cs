using DebugUtils;
using FerryBoat;
using UnityEngine;

public class Destination : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;

    private void OnTriggerEnter(Collider other)
    {
        if ((targetLayer & (1 << other.gameObject.layer)) != 0)
        {
            DevDebug.Log("Reached target ", DebugColor.Green);

            Act.ReachedDestination?.Invoke();
        }
    }
}
