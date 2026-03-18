using UnityEngine;

[System.Serializable]
public class Fuel
{
    [Header("Fuel Settings")]
    public float maxFuel = 100f;
    public float currentFuel = 100f;

    [Header("Consumption Rates (units/sec)")]
    public float idleConsumption = 0.05f;    // engine on, not moving
    public float baseConsumption = 0.2f;     // per m/s of speed
    public float accelerationSurcharge = 0.5f; // extra burn while accelerating

    public bool IsEmpty => currentFuel <= 0f;
    public float FuelPercent => currentFuel / maxFuel;
}
