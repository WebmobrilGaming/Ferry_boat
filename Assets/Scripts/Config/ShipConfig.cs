using System;
using UnityEngine;
namespace Ferry.Config
{
    [CreateAssetMenu(fileName ="ShipConfig", menuName ="Config/ShipConfig")]
    public class ShipConfig : ScriptableObject
    {
        public ShipType shipType;
        public float acceleration;
        public float deceleration;
        public float rotationMultiplier;
        public float shipSpeed;
        public float health;
        public float damage;
        public FuelConfig fuelConfig;
    }
    [Serializable]
    public class FuelConfig
    {
        public float fuelCapacity;
        public float idleConsumption;
        public float baseConsumption;
        public float accelerationSurge;
    }
    public enum ShipType
    {
        CruiseShip,
        Ferry,
        Yacht,
        RiverBoat,
        ContainerShip,
        TankerShip,
        BulkCarrierShip,
        CargoShip
    }
}
