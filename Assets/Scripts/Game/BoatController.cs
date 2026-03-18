using TMPro;
using UnityEditor;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BoatController : MonoBehaviour,IHelem,IGear
{
    [SerializeField] HelmController helmController;
    [SerializeField] Gear mGear;

    [Header("Settings:")]
    [Range(0,1f)]
    [SerializeField] float rotationMultiplier = 0.3f;

    [Range(0, 10f)]
    [SerializeField] float mSpeed = 5.0f;


    [SerializeField] bool isControl;

    Quaternion startRotation;

    private const float MPS_TO_KNOTS = 1.94384f;

    [Space]
    [SerializeField] float speedInKnots;
    [SerializeField] TMP_Text mSpeedKnots;
    private Vector3 mLastPosition;

    private float mCurrentSpeed;
    [Range(0,10f)]
    [SerializeField] float mAcceleration = 2f;

    [Range(0, 10f)]
    [SerializeField] float mDeceleration = 3f;   

    private void OnEnable()
    {
        helmController.callback = this;
        mGear.callback = this;


        startRotation = transform.localRotation;
    }

    void GearAction()
    {
        isControl = false;

        mGear.Change(!mGear.Stat);
    }

    public void GearChange()
    {
        isControl = true;
    }

    private void Update()
    {
        if (Keyboard.current.leftArrowKey.isPressed)
            helmController.Direct(HelmDirection.left);

        if (Keyboard.current.rightArrowKey.isPressed)
            helmController.Direct(HelmDirection.right);

        if (Keyboard.current.spaceKey.isPressed)
            GearAction();

        if (Keyboard.current.leftArrowKey.wasReleasedThisFrame || Keyboard.current.rightArrowKey.wasReleasedThisFrame)
        {
          //  Debug.Log("Released helm !!!");
            helmController.StopRotation();
        }

        #region SPEED_HANDLING
        // Accelerate or decelerate based on gear
        float targetSpeed = mGear.Stat ? mSpeed : 0f;

        float rate = mGear.Stat ? mAcceleration : mDeceleration;
        mCurrentSpeed = Mathf.MoveTowards(mCurrentSpeed, targetSpeed, rate * Time.deltaTime);

        // Move using smoothed speed
        transform.position += transform.forward * mCurrentSpeed * Time.deltaTime;

        // Knots from actual displacement
        float actualSpeed = Vector3.Distance(transform.position, mLastPosition) / Time.deltaTime;
        speedInKnots = actualSpeed * MPS_TO_KNOTS;
        mSpeedKnots.text = $"{speedInKnots:F2} Knots";

        mLastPosition = transform.position;
        #endregion
    }

    private float currentRotation = 0f;
    private float lastHelmZ = 0f;
    private bool isFirstUpdate = true;

    public void Rotate(float zValue)
    {
        if (isFirstUpdate)
        {
            lastHelmZ = zValue;
            isFirstUpdate = false;
            return;
        }

        float delta = Mathf.DeltaAngle(lastHelmZ, zValue);
        lastHelmZ = zValue;

        currentRotation += delta * rotationMultiplier;

        transform.localRotation = startRotation * Quaternion.Euler(0, currentRotation, 0);

        Debug.Log($"Delta: {delta} | Accumulated: {currentRotation}");
    }
}
