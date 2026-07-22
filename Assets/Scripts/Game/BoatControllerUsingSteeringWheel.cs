using UnityEngine;
using UnityEngine.InputSystem;

public class BoatControllerUsingSteeringWheel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BoatController boatController;

    [Header("Wheel Settings")]
    [SerializeField] private float steeringSensitivity = 120f;
    [SerializeField] private float deadZone;
    [SerializeField] private float steeeringSmothness = 2f;
    [SerializeField] private float maxTurnSpeed = 35f;
    private float currentSteering;
    public HelmController helmController;

    private float wheelInput;

    private void Awake()
    {
        if (boatController == null)
            boatController = GetComponent<BoatController>();
            deadZone = 0.001f;
            //helmController = GetComponent<HelmController>();
    }

    private void Update()
    {
        // No wheel connected
        if (Gamepad.current == null)
            return;
        if (!boatController.IsEningeActive)
            return;
            
        // Read steering wheel axis
        wheelInput = Gamepad.current.leftStick.x.ReadValue();
        Debug.LogWarning(wheelInput);
        if(Mathf.Abs(wheelInput)<deadZone)wheelInput = 0f;
        helmController.SetWheelAngle(wheelInput);
        boatController.SetSteering(wheelInput);
        // if (wheelInput < 0)
        // {
        //     helmController.Direct(HelmDirection.left);
        // }
        // if (wheelInput > 0)
        // {
        //     helmController.Direct(HelmDirection.right);
        // }
        // if (wheelInput == 0)
        // {
        //     helmController.StopRotation();
        // }
        
        // // Deadzone
        // if (Mathf.Abs(wheelInput) < deadZone)
        //     wheelInput = 0f;

        
       

      
    }
}