using FerryBoat.Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Thrustmaster : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoatController boatController;
    [Header("Throttle")]
    [SerializeField] private float smoothTrottle;
    [SerializeField]private Gear mGear;
    [SerializeField]private float previousThrottle;

    private InputDevice device;

    private AxisControl throttle1;
    private AxisControl throttle2;

    private float smoothThrottle;
    private void Start()
    {
        // Find Airbus device
        foreach (var d in InputSystem.devices)
        {
            if (d.displayName.Contains("TCA"))
            {
                device = d;
                Debug.Log($"Found device: {d.displayName}");
                break;
            }
        }

        if (device == null)
        {
            Debug.LogWarning("No TCA Airbus device found!");
            enabled = false;
            return;
        }

        // Assign axes
        foreach (var control in device.allControls)
        {
            if (control is AxisControl axis)
            {
                if (control.name == "x")
                    throttle1 = axis;

                if (control.name == "y")
                    throttle2 = axis;
            }
        }

        if (throttle1 == null || throttle2 == null)
        {
            Debug.LogError("Throttle axes not found!");
            enabled = false;
        }
    }
    void Update()
    {
        if (throttle1 == null || throttle2 == null)
            return;

        float t1 = 1f - throttle1.ReadValue();

        t1 = t1 * 0.5f;

        float combined = t1;
        
        if(Mathf.Abs(combined - previousThrottle) > 0.001f)
        {
            mGear.Thrustmaster_GearPositionChange(combined);
            previousThrottle = combined;
        }
       
        if (combined < 0.05f)
            combined = 0f;

        float maxSpeed =
            boatController.ferryConfig
            .velocitiesLevels[(int)boatController.difficultyLevel]
            .shipSpeed;

        float targetSpeed = combined * maxSpeed;

        smoothTrottle = Mathf.Lerp(
            smoothTrottle,
            targetSpeed,
            Time.deltaTime * 3f);

        boatController.SpeedChange(smoothTrottle);
        
    }
}
