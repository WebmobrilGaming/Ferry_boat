using DG.Tweening;
using FerryBoat;
using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] Material mIndicator;

    [Header("Settings:")]
    [Range(0,1f)]
    [SerializeField] float rotationMultiplier = 0.3f;

    [Range(0, 100f)]
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

    [SerializeField] Slider trottleSlider;

    [Header("Health Settings:")]
    [SerializeField] float mHealth;
    [SerializeField] float mdamage;
    [SerializeField] Slider mHealthSlider;

    [SerializeField] TMP_Text mHealthText;

    bool isHit = false;
    bool isEngine = false;


    private void OnEnable()
    {
        helmController.callback = this;
        mGear.callback = this;

        mFuelSlider.value = mFuel.currentFuel;
        startRotation = transform.localRotation;

        isControl = false;
        isHit = false;

        Act.SpeedChange += SpeedChange;
        Act.HitAction += HitAction;

        mHealthSlider.maxValue = mHealth;
        mHealthSlider.value = mHealth;

        mIndicator.DisableKeyword("_EMISSION");
    }

    private void OnDisable()
    {
        Act.SpeedChange -= SpeedChange;
        Act.HitAction -= HitAction;

        mIndicator.DisableKeyword("_EMISSION");
    }

    private void HitAction()
    {
        Debug.LogError("Hit !! ");

        trottleSlider.DOValue(0, 0.65f);
        mIndicator.DisableKeyword("_EMISSION");
        mGear.Change(false);

        isHit = true;

        mHealth -= mdamage;
        mHealthSlider.DOValue(mHealth, 1.0f);

        mHealthText.transform.DOShakePosition(0.5f, strength: 20f, vibrato: 10)
            .OnStart(()=> mHealthText.color = Color.red)
            .SetUpdate(true)
            .OnComplete(()=> mHealthText.color = Color.white); 

        if (mHealth <=0)
        {
            Act.BoatDestroyedAction?.Invoke();
            return;
        }

        StartCoroutine(Recover());
    }

    IEnumerator Recover()
    {
        yield return new WaitForSeconds(4.0f);

        trottleSlider.DOValue(mBoatSpeed, 0.65f);
        mIndicator.EnableKeyword("_EMISSION");

        mGear.Change(true);
        isHit = false;
    }
     

    void GearAction()
    {
        Debug.Log("GEAR CHANGE !!!");

        isControl = false;
        mGear.Change(!mGear.Stat);

        Act.EnableScore(mGear.Stat);
    }

    public void GearChange()
    {
        isControl = true;

        float speed = mGear.Stat ? mSpeed : -1;
        
        trottleSlider.maxValue = speed <= 0 ? 0 : speed;

        if (mGear.Stat)
        {
            trottleSlider.DOValue(mBoatSpeed, 0.65f);
            mIndicator.EnableKeyword("_EMISSION");
        }
        else
            mIndicator.DisableKeyword("_EMISSION");


        Act.SpeedInit?.Invoke(speed);
    }

    private void Update()
    {
        if (mFuel.FuelPercent < 0.1f)
            return;

        if (Keyboard.current.leftArrowKey.isPressed && mGear.Stat)
            helmController.Direct(HelmDirection.left);

        if (Keyboard.current.rightArrowKey.isPressed && mGear.Stat)
            helmController.Direct(HelmDirection.right);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
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

        mCurrentSpeed = isHit ? 0 : mCurrentSpeed;

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

        if (!mGear.Stat)
            return;

        trottleSlider.DOValue(val,0.65f);  
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
            EngineStat(false);
        }
    }

    void EngineStat(bool enable)
    {
        isEngine = enable;

        if (!enable)
        {
            trottleSlider.DOValue(0, 0.65f);
            mIndicator.DisableKeyword("_EMISSION");
            mGear.Change(false);

            return;
        }

        trottleSlider.DOValue(mBoatSpeed, 0.65f);
        mIndicator.EnableKeyword("_EMISSION");

        mGear.Change(true);
        isHit = false;
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
    }
}
