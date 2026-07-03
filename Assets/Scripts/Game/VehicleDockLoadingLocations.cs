using System.Collections.Generic;
using UnityEngine;

public class VehicleDockLoadingLocations : MonoBehaviour
{
    [Header("Vehicle Loading Target Location")]
    [SerializeField] public List<Transform> vehicle_Loc;
    
    public static VehicleDockLoadingLocations instance;

    void Awake()
    {
        instance = this;
    } 
}
