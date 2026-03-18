using DG.Tweening;
using FerryBoat;
using System;
using System.ComponentModel;
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
    [SerializeField] float mBoatSpeed;


    [SerializeField] bool isControl;

    Quaternion startRotation;

    private const float MPS_TO_KNOTS = 1.94384f;

    [Header("Speed Settings:")]
    [Space]
    [SerializeField] float speedInKnots;
    [SerializeField] TMP_Text mSpeedKnots;
    private Vector3 mLastPosition;

    private float mCurrentSpeed;
    private float mPreviousSpeed;

    [Range(0,10f)]
    [SerializeField] float mAcceleration = 2f;

    [Range(0, 10f)]
    [SerializeField] float mDeceleration = 3f;

    [Header("Fuel Settings:")]

    [SerializeField] TMP_Text mFuelText;
    [SerializeField] Slider mFuelSlider;
    [SerializeField] Fuel mFuel;


    private void OnEnable()
    {
        helmController.callback = this;
        mGear.callback = this;

        mFuelSlider.value = mFuel.currentFuel;
        startRotation = transform.localRotation;

        Act.SpeedChange += SpeedChange;
    }

    private void OnDisable()
    {
        Act.SpeedChange -= SpeedChange;
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
        if (mFuel.FuelPercent < 0.1f)
            return;

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
        // Fuel gate
        bool canMove = mGear.Stat && !mFuel.IsEmpty;

        // Accelerate or decelerate based on gear + fuel
        float targetSpeed = canMove ? mBoatSpeed : 0f;
        float rate = canMove ? mAcceleration : mDeceleration;
        mCurrentSpeed = Mathf.MoveTowards(mCurrentSpeed, targetSpeed, rate * Time.deltaTime);

        // Move using smoothed speed
        transform.position += transform.forward * mCurrentSpeed * Time.deltaTime;

        // Knots from actual displacement
        float actualSpeed = Vector3.Distance(transform.position, mLastPosition) / Time.deltaTime;
        speedInKnots = actualSpeed * MPS_TO_KNOTS;
        mSpeedKnots.text = $"{speedInKnots:F2} Knots";

        mLastPosition = transform.position;
        #endregion

        #region FUEL_HANDLING
        if (mGear.Stat && !mFuel.IsEmpty)
        {
            float speedDelta = mCurrentSpeed - mPreviousSpeed;
            bool isAccelerating = speedDelta > 0.01f;

            float consumption = mFuel.idleConsumption
                + mFuel.baseConsumption * mCurrentSpeed
                + (isAccelerating ? mFuel.accelerationSurcharge * speedDelta : 0f);

            mFuel.currentFuel -= consumption * Time.deltaTime;
            mFuel.currentFuel = Mathf.Max(mFuel.currentFuel, 0f);
        }

        mPreviousSpeed = mCurrentSpeed;

        UpdateFuelUI();
        #endregion
    }

    private void SpeedChange(float val)
    {
        mBoatSpeed = val;
    }

    void UpdateFuelUI()
    {
        // Example — wire to your own UI elements
        //  mFuelText.text = $"{mFuel.currentFuel:F1} L";
        mFuelSlider.value = mFuel.FuelPercent;

        // Low fuel warning
        if (mFuel.FuelPercent < 0.2f)
        {
            mFuelText.text = "Low Fuel";

            
            Sequence sequence =  DOTween.Sequence();

            sequence.Join(mFuelText.DOColor(Color.red, 0.2f))
                    .Append(mFuelText.DOColor(Color.white, 0.2f))
                    .SetLoops(-1);
        }

        if(mFuel.FuelPercent < 0.1f)
        {
            mGear.Change(false);
        }
    }

    public void Refuel(float amount)
    {
        mFuel.currentFuel = Mathf.Min(mFuel.currentFuel + amount, mFuel.maxFuel);
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
