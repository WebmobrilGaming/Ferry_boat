using DG.Tweening;
using Ferry.Config;
using FerryBoat;
using Newtonsoft.Json;
using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BoatController : MonoBehaviour, IHelem, IGear
{
    [SerializeField] HelmController helmController;
    [SerializeField] Gear mGear;
    [SerializeField] Material mIndicator;

    [Header("Boat Visual Bobbing")]
    [SerializeField] Transform boatVisual;   // assign boat model/mesh child here
    [SerializeField] float easyWaveHeight = 0.03f;
    [SerializeField] float mediumWaveHeight = 0.06f;
    [SerializeField] float hardWaveHeight = 0.10f;

    [SerializeField] float easyWaveSpeed = 0.8f;
    [SerializeField] float mediumWaveSpeed = 1.2f;
    [SerializeField] float hardWaveSpeed = 1.8f;

    [SerializeField] float rollAmount = 1.5f;
    [SerializeField] float pitchAmount = 1.0f;

    [Header("Settings:")]
    [Range(0, 1f)]
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
    public float Speed => speedInKnots;
    [SerializeField] TMP_Text mSpeedKnots;
    private Vector3 mLastPosition;
    private float mCurrentSpeed;
    private float mPreviousSpeed;

    [Range(0, 100f)]
    [SerializeField] float mAcceleration = 2f;
    [Range(0, 100f)]
    [SerializeField] float mDeceleration = 3f;
    [SerializeField] float mReverseSpeed = 2f;

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
    public ShipConfig ferryConfig;

    private float currentRotation = 0f;
    private float lastHelmZ = 0f;
    private bool isFirstUpdate = true;
    private bool isEngineStarted = false;
    public bool IsEningeActive => isEngineStarted;
    public bool IsInDock { get; set; }
    public bool IsEnterDock { get; set; }

    [SerializeField] DifficultyLevel difficultyLevel;

    [SerializeField] float thresholdSpeed;
    bool mThresholdApplied;

    // wave bobbing cache
    private Vector3 visualStartLocalPos;
    private Quaternion visualStartLocalRot;
    private float waveAmplitude;
    private float waveSpeed;

    private void Awake()
    {
        ShipConfigController.Instance.BuildMap();
        LoadConfig();
        IsEnterDock = false;
    }

    private void LoadConfig()
    {
        ferryConfig = ShipConfigController.Instance.GetShipConfig(ShipType.Ferry);
        rotationMultiplier = ferryConfig.rotationMultiplier;
        mSpeed = ferryConfig.shipSpeed;
        mAcceleration = ferryConfig.acceleration;
        mDeceleration = ferryConfig.deceleration;
        mHealth = ferryConfig.health;
        mdamage = ferryConfig.damage;
        mFuel.accelerationSurcharge = ferryConfig.fuelConfig.accelerationSurge;
        mFuel.baseConsumption = ferryConfig.fuelConfig.baseConsumption;
        mFuel.maxFuel = ferryConfig.fuelConfig.fuelCapacity;
    }

    private void OnEnable()
    {
        helmController.callback = this;
        mGear.callback = this;

        mFuelSlider.value = mFuel.currentFuel;
        startRotation = transform.localRotation;

        isControl = false;
        isHit = false;
        isEngineStarted = false;

        Act.SpeedChange += SpeedChange;
        Act.HitAction += HitAction;

        mHealthSlider.maxValue = mHealth;
        mHealthSlider.value = mHealth;

        mIndicator.DisableKeyword("_EMISSION");

        if (PlayerPrefs.HasKey("Settings"))
        {
            string json = PlayerPrefs.GetString("Settings");
            var loaded = JsonConvert.DeserializeObject<SettingsData>(json);
            difficultyLevel = loaded.level;
        }
        else
        {
            difficultyLevel = DifficultyLevel.easy;
        }

        thresholdSpeed = difficultyLevel switch
        {
            DifficultyLevel.easy => 5.0f,
            DifficultyLevel.medium => 4.0f,
            DifficultyLevel.hard => 3.0f,
            _ => 5.0f
        };

        SetWaveValues();

        if (boatVisual != null)
        {
            visualStartLocalPos = boatVisual.localPosition;
            visualStartLocalRot = boatVisual.localRotation;
        }

        mLastPosition = transform.position;
    }

    private void SetWaveValues()
    {
        switch (difficultyLevel)
        {
            case DifficultyLevel.easy:
                waveAmplitude = easyWaveHeight;
                waveSpeed = easyWaveSpeed;
                break;

            case DifficultyLevel.medium:
                waveAmplitude = mediumWaveHeight;
                waveSpeed = mediumWaveSpeed;
                break;

            case DifficultyLevel.hard:
                waveAmplitude = hardWaveHeight;
                waveSpeed = hardWaveSpeed;
                break;

            default:
                waveAmplitude = easyWaveHeight;
                waveSpeed = easyWaveSpeed;
                break;
        }
    }

    private void OnDisable()
    {
        Act.SpeedChange -= SpeedChange;
        Act.HitAction -= HitAction;

        mIndicator.DisableKeyword("_EMISSION");
    }

    private void Update()
    {
        ApplyWaveMotion();
        if (mFuel.FuelPercent < 0.1f)
            return;

        if (Keyboard.current.leftArrowKey.isPressed && mGear.Stat)
            helmController.Direct(HelmDirection.left);

        if (Keyboard.current.rightArrowKey.isPressed && mGear.Stat)
            helmController.Direct(HelmDirection.right);

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            GearAction();
            isEngineStarted = !isEngineStarted;
        }

        if (Keyboard.current.leftArrowKey.wasReleasedThisFrame ||
            Keyboard.current.rightArrowKey.wasReleasedThisFrame)
            helmController.StopRotation();

        bool isReversing = (Keyboard.current.sKey.isPressed ||
                            Keyboard.current.downArrowKey.isPressed) && mGear.Stat;

        if (!mGear.Stat)
            return;

        #region SPEED_HANDLING
        bool canMove = mGear.Stat && !mFuel.IsEmpty && !isHit;

        float targetSpeed = 0f;
        if (canMove)
            targetSpeed = isReversing ? -mReverseSpeed : mBoatSpeed;

        float rate = canMove ? mAcceleration : mDeceleration;

        mCurrentSpeed = Mathf.MoveTowards(mCurrentSpeed, targetSpeed, rate * Time.deltaTime);

        var forward = transform.forward * mCurrentSpeed * Time.deltaTime;
        var currentTransform = transform.position;
        var target = new Vector3(
            currentTransform.x + forward.x,
            currentTransform.y,
            currentTransform.z + forward.z);
        transform.position = target;

        float actualSpeed = Vector3.Distance(transform.position, mLastPosition) / Time.deltaTime;
        speedInKnots = actualSpeed * MPS_TO_KNOTS * 0.095f;
        mSpeedKnots.text = $"{(mCurrentSpeed < 0 ? "-" : "")}{speedInKnots:F2} Speed";

        mLastPosition = transform.position;
        ThresholdCheck();
        #endregion

        #region FUEL_HANDLING
        if (mGear.Stat && !mFuel.IsEmpty)
        {
            float speedDelta = mCurrentSpeed - mPreviousSpeed;
            bool isAccelerating = speedDelta > 0.01f;

            float consumption = mFuel.idleConsumption
                + mFuel.baseConsumption * Mathf.Abs(mCurrentSpeed)
                + (isAccelerating ? mFuel.accelerationSurcharge * speedDelta : 0f);

            mFuel.currentFuel -= consumption * Time.deltaTime;
            mFuel.currentFuel = Mathf.Max(mFuel.currentFuel, 0f);
        }

        mPreviousSpeed = mCurrentSpeed;
        UpdateFuelUI();
        #endregion

        ApplyWaveMotion();
    }

    private void ApplyWaveMotion()
    {
        if (boatVisual == null)
            return;

        float t = Time.time * waveSpeed;

        // vertical bobbing
        float bob = Mathf.Sin(t) * waveAmplitude;

        // small roll/pitch so it feels like wave motion
        float roll = Mathf.Sin(t * 1.3f) * rollAmount;
        float pitch = Mathf.Cos(t * 1.1f) * pitchAmount;

        boatVisual.localPosition = visualStartLocalPos + new Vector3(0f, bob, 0f);
        boatVisual.localRotation = visualStartLocalRot * Quaternion.Euler(pitch, 0f, roll);
    }

    void ThresholdCheck()
    {
        int threshold = (int)thresholdSpeed;
        int currentSpeed = (int)speedInKnots;

        if (threshold > currentSpeed)
        {
            mThresholdApplied = false;
            return;
        }

        if (trottleSlider.value <= 0)
            return;

        if (mThresholdApplied)
            return;

        mThresholdApplied = true;

        float newHealth = mHealth - mdamage;

        GamePopUp.Instance.PopStat("Deducting Coins", 3.0f);
    }

    private void SpeedChange(float val)
    {
        mBoatSpeed = val;

        if (!mGear.Stat)
            return;

        trottleSlider.DOValue(val, 0.65f);
    }

    void GearAction()
    {
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
        {
            mIndicator.DisableKeyword("_EMISSION");
        }

        Act.SpeedInit?.Invoke(speed);
    }

    private void HitAction()
    {
        Debug.LogError("Hit !! ");
        Act.BoatDestroyedAction?.Invoke();
        mHealth = 0;
    }

    void UpdateFuelUI()
    {
        mFuelSlider.value = mFuel.FuelPercent;

        if (mFuel.FuelPercent < 0.2f)
        {
            mFuelText.text = "Low Fuel";

            Sequence sequence = DOTween.Sequence();
            sequence.Join(mFuelText.DOColor(Color.red, 0.2f))
                    .Append(mFuelText.DOColor(Color.white, 0.2f))
                    .SetLoops(-1);
        }

        if (mFuel.FuelPercent < 0.1f)
            EngineStat(false);
    }

    void EngineStat(bool enable)
    {
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