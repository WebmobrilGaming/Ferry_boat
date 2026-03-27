using System;
using UnityEngine;
namespace Ferry.Config
{
    [CreateAssetMenu(fileName = "ShipConfig", menuName = "Config/ShipConfig")]
    public class ShipConfig : ScriptableObject
    {
        public ShipType shipType;
        public Velocity[] velocitiesLevels;
        public float limit;
        public float health;
        public float damage;
        public FuelConfig fuelConfig;
    }
    [Serializable]
    public class Velocity
    {
        public float acceleration;
        public float deceleration;
        public float angularSpeed;
        public float shipSpeed;
        public float windSpeed;
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
