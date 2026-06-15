using DG.Tweening;
using Ferry.Config;
using Ferry.Motion;
using FerryBoat.Actions;
using FerryBoat.Store;
using GF;
using Newtonsoft.Json;
using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BoatController : MonoBehaviour, IHelem, IGear
{
    [SerializeField] HelmController helmController;
    [SerializeField] Gear mGear;
    [SerializeField] Material mIndicator;
    [Header("Settings:")]
    [Range(0, 1f)]
    [SerializeField] float rotationMultiplier = 0.3f;
    [Range(0, 100f)]
    [SerializeField] float mSpeed = 5.0f;
    [SerializeField] float mBoatSpeed;
    [SerializeField] bool isControl;
    public Quaternion startRotation;
    private const float MPS_TO_KNOTS = 2.23694f;

    [SerializeField] TMP_Text mWind;


    [Header("Speed Settings:")]
    [Space]
    [SerializeField] public float speedInKnots;
    public float Speed => speedInKnots;
    [SerializeField] TMP_Text mSpeedKnots;
    private Vector3 mLastPosition;
    private float mCurrentSpeed;
    private float mPreviousSpeed;

    [Range(0, 100f)]
    [SerializeField] float mAcceleration = 2f;
    [Range(0, 100f)]
    [SerializeField] float mDeceleration = 3f;
    [SerializeField] float mReverseSpeed = 25f;

    [Header("Fuel Settings:")]
    [SerializeField] TMP_Text mFuelText;
    [SerializeField] Slider mFuelSlider;
    //[SerializeField] Fuel mFuel;
    [SerializeField] Slider trottleSlider;

    [Header("Health Settings:")]
    [SerializeField] float mHealth;
    [SerializeField] float mdamage;
    [SerializeField] Slider mHealthSlider;
    [SerializeField] TMP_Text mHealthText;

    [Header("Score Settings")]
    [SerializeField] public Score_System score_System;


    bool isHit = false;
    public ShipConfig ferryConfig;

    public float currentRotation = 0f;
    private float lastHelmZ = 0f;
    private bool isFirstUpdate = true;
    public bool isEngineStarted = false;
    public bool IsEningeActive => isEngineStarted;
    public bool IsInDock { get; set; }
    public bool IsEnterDock { get; set; }

    [SerializeField] public DifficultyLevel difficultyLevel;

    [SerializeField] float thresholdSpeed;
    bool mThresholdApplied;
    public static event Action OnBoatStartEvent;
    private bool isInitialized = false;
    private WaveMotion waveMotion;
    [Header("Steering")]
    [SerializeField] private float steeringSensitivity = 120f;
    [SerializeField] public bool isTurning;
    [SerializeField] private Button backButton;
    [SerializeField] private float windDriftStrength;
    [SerializeField] private bool Is_KeyboardEnabled;
    [SerializeField] private float maxTurnSpeed = 20f;
    private float wheelInput;
    private void Awake()
    {
        LoadConfig();
        IsEnterDock = false;
    }

    private void LoadConfig()
    {
        waveMotion = GetComponent<WaveMotion>();
        ferryConfig = ShipConfigController.Instance.GetShipConfig(ShipType.Ferry);
        rotationMultiplier = ferryConfig.velocitiesLevels[(int)difficultyLevel].angularSpeed;
        mSpeed = ferryConfig.velocitiesLevels[(int)difficultyLevel].shipSpeed;
        mAcceleration = ferryConfig.velocitiesLevels[(int)difficultyLevel].acceleration;
        mDeceleration = ferryConfig.velocitiesLevels[(int)difficultyLevel].deceleration;
        mHealth = ferryConfig.health;
        mdamage = ferryConfig.damage;
        // mFuel.accelerationSurcharge = ferryConfig.fuelConfig.accelerationSurge;
        // mFuel.baseConsumption = ferryConfig.fuelConfig.baseConsumption;
        // mFuel.maxFuel = ferryConfig.fuelConfig.fuelCapacity;
    }

    private void OnEnable()
    {
        helmController.callback = this;
        mGear.callback = this;
        isTurning = false;
        var currentDifficulty = PlayerPrefs.GetString(GamePrefs.difficulty_Level,DifficultyLevel.easy.ToString());
        Debug.LogWarning(currentDifficulty);
       // mFuelSlider.value = mFuel.currentFuel;
        startRotation = transform.localRotation;

        isControl = false;
        isHit = false;
        isEngineStarted = false;
        backButton.gameObject.SetActive(true);

        Act.SpeedChange += SpeedChange;
        Act.HitAction += HitAction;

        mHealthSlider.maxValue = mHealth;
        mHealthSlider.value = mHealth;

        mIndicator.DisableKeyword("_EMISSION");
        UserInterFace.StopEngineEvent += StopEngine;
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

        mWind.text = difficultyLevel switch
        {
            DifficultyLevel.easy => "Stable",
            DifficultyLevel.medium => "Challenging",
            DifficultyLevel.hard => "Extreme"
        };

        thresholdSpeed = difficultyLevel switch
        {
            DifficultyLevel.easy => 5.0f,
            DifficultyLevel.medium => 4.0f,
            DifficultyLevel.hard => 3.0f,
            _ => 5.0f
        };

        mLastPosition = transform.position;
        waveMotion.SetLevel(difficultyLevel);
    }

    private void StopEngine()
    {
        isEngineStarted = false;
        //mFuel.currentFuel = 0;
    }

    private void OnDisable()
    {
        Act.SpeedChange -= SpeedChange;
        Act.HitAction -= HitAction;
        UserInterFace.StopEngineEvent -= StopEngine;
        mIndicator.DisableKeyword("_EMISSION");
    }

    private void Update()
    {


        // if (mFuel.FuelPercent < 0.1f)
        //     return;
        if (Is_KeyboardEnabled)
        {
            if (Keyboard.current.leftArrowKey.isPressed && mGear.Stat)
                helmController.Direct(HelmDirection.left);

            if (Keyboard.current.rightArrowKey.isPressed && mGear.Stat)
                helmController.Direct(HelmDirection.right);
            if (Keyboard.current.leftArrowKey.wasReleasedThisFrame ||
                Keyboard.current.rightArrowKey.wasReleasedThisFrame)
                helmController.StopRotation();
        }

        if (Timer.gameStarted)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                if (!isInitialized)
                {
                    isInitialized = true;
                    OnBoatStartEvent?.Invoke();
                }
                GearAction();
                isEngineStarted = !isEngineStarted;
            }
        }
        bool isReversing = (Keyboard.current.sKey.isPressed ||
                            Keyboard.current.downArrowKey.isPressed) && mGear.Stat;

        // Removed early return here — speed handling must run even when gear/engine
        // is off so the boat decelerates to a stop instead of freezing mid-motion.

        #region SPEED_HANDLING
        // canMove requires the engine to be running, gear engaged, fuel available, and no hit
        bool canMove = mGear.Stat && isEngineStarted && !isHit ; //&& !mFuel.IsEmpty;

        float targetSpeed = 0f;
        if (canMove)
            targetSpeed = isReversing ? -mReverseSpeed : Mathf.Min(mBoatSpeed, ferryConfig.velocitiesLevels[(int)difficultyLevel].shipSpeed);

        float rate = canMove ? mAcceleration : mDeceleration;

        mCurrentSpeed = Mathf.MoveTowards(mCurrentSpeed, targetSpeed, rate * Time.deltaTime);

        var forward = transform.forward * mCurrentSpeed * Time.deltaTime;
        var currentTransform = transform.position;
        var target = new Vector3(
            currentTransform.x + forward.x,
            currentTransform.y,
            currentTransform.z + forward.z);
        transform.position = target;
        if (Mathf.Abs(mCurrentSpeed) > 0.1f)
        {
            if (waveMotion.currentWindSpeed != 0)
            {
                float drift =
               windDriftStrength * (waveMotion.currentWindSpeed / 35f);

                currentRotation += drift * Time.deltaTime;

                transform.localRotation =
                    startRotation *
                    Quaternion.Euler(0, currentRotation, 0);
            }
           
        }


        float actualSpeed = Vector3.Distance(transform.position, mLastPosition) / Time.deltaTime;
        //speedInKnots = actualSpeed * MPS_TO_KNOTS * 0.095f;
        speedInKnots = actualSpeed * MPS_TO_KNOTS * 0.13f;
        speedInKnots =Mathf.Clamp(speedInKnots,0,10);
        mSpeedKnots.text = $"{(mCurrentSpeed < 0 ? "-" : "")}{speedInKnots:F2} mph";

        mLastPosition = transform.position;
        ThresholdCheck();
        #endregion

        #region FUEL_HANDLING
        // if (mGear.Stat && !mFuel.IsEmpty)
        // {
        //     float speedDelta = mCurrentSpeed - mPreviousSpeed;
        //     bool isAccelerating = speedDelta > 0.01f;

        //     float consumption = mFuel.idleConsumption
        //         + mFuel.baseConsumption * Mathf.Abs(mCurrentSpeed)
        //         + (isAccelerating ? mFuel.accelerationSurcharge * speedDelta : 0f);

        //     mFuel.currentFuel -= consumption * Time.deltaTime;
        //     mFuel.currentFuel = Mathf.Max(mFuel.currentFuel, 0f);
        // }

        // mPreviousSpeed = mCurrentSpeed;
        // UpdateFuelUI();
        #endregion
    }



    void ThresholdCheck()
    {
        if (!DockPassCheker.passedStartingDock)
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

            //float newHealth = mHealth - mdamage;
            Utils.ShowInGamePopup($"You are crossing the speed limit, It will deduct your points..! Keep it under {thresholdSpeed}/mph");

            score_System.Set(-20, Data.time);
        }
            
        
        
    }

    public void SpeedChange(float val)
    {
        mBoatSpeed = Mathf.Clamp(val, 0f, ferryConfig.velocitiesLevels[(int)difficultyLevel].shipSpeed);
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

        float speed = mGear.Stat ? Mathf.Min(mSpeed, ferryConfig.velocitiesLevels[(int)difficultyLevel].shipSpeed) : -1f;
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
        Debug.LogWarning("Hit !! ");
        backButton.gameObject.SetActive(false);
        Act.BoatDestroyedAction?.Invoke();
        mHealth = 0;
    }

    // void UpdateFuelUI()
    // {
    //     mFuelSlider.value = mFuel.FuelPercent;

    //     if (mFuel.FuelPercent < 0.2f)
    //     {
    //         mFuelText.text = "Low Fuel";

    //         Sequence sequence = DOTween.Sequence();
    //         sequence.Join(mFuelText.DOColor(Color.red, 0.2f))
    //                 .Append(mFuelText.DOColor(Color.white, 0.2f))
    //                 .SetLoops(-1);
    //     }

    //     if (mFuel.FuelPercent < 0.1f)
    //         EngineStat(false);
    // }

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

    // public void Refuel(float amount)
    // {
    //     mFuel.currentFuel = Mathf.Min(mFuel.currentFuel + amount, mFuel.maxFuel);
    // }

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
    public void SetSteering(float wheelValue)
    {
        float turnRate = wheelValue * maxTurnSpeed;
        currentRotation += turnRate * Time.deltaTime;
        transform.localRotation =  startRotation * Quaternion.Euler(0,currentRotation,0);
    }
    public void PerformUTurn()
    {
        if (isTurning)
            return;

        isTurning = true;

        float previousSpeed = mCurrentSpeed;

        DOTween.To(
     () => mCurrentSpeed,
     x => mCurrentSpeed = x,
     0,
     1f
 )
 .OnComplete(() =>
 {
     transform.DORotate(
         transform.eulerAngles + new Vector3(0, 180, 0),
         3f
     )
     .OnComplete(() =>
     {
        currentRotation = transform.localEulerAngles.y;
         DOTween.To(
             () => mCurrentSpeed,
             x => mCurrentSpeed = x,
             previousSpeed,
             1f
         );

         isTurning = false;
     });
 });
    }
}
