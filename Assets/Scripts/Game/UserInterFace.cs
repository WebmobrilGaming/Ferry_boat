using DG.Tweening;
using Ferry.Config;
using FerryBoat;
using GF;
using Newtonsoft.Json;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterFace : MonoBehaviour
{
    [SerializeField] InputAction scrollAction;

    [Space]
    [SerializeField] float mScrollRange;

    float rangeMin = 0f;
    float rangeMax = 10f;
    [SerializeField] float sensitivity = 1f;
    public CanvasGroup Info;
    public TMP_Text InfoTxt;
    private Coroutine dockMissedRoutine;
    public Button backBtn;
    public TMP_Text windTxt;
    public TMP_Text levelTxt;
    private void Awake()
    {
        rangeMax = -1;
        scrollAction.performed += OnScroll;
        backBtn.AddListener(null, GoHome);
    }

    private void GoHome()
    {
        SceneManager.LoadScene(0);
    }
    private DifficultyLevel GetDifficultyLevel()
    {
        if (PlayerPrefs.HasKey("Settings"))
        {
            string json = PlayerPrefs.GetString("Settings");
            var loaded = JsonConvert.DeserializeObject<SettingsData>(json);
            return loaded.level;
        }
        return DifficultyLevel.easy;
    }
    private void OnEnable()
    {
        scrollAction.Enable();
        TimerAndScore.TimeOutEvent += TimeOut;
        var ferryConfig = ShipConfigController.Instance.GetShipConfig(ShipType.Ferry);
        int level = (int)GetDifficultyLevel();
        windTxt.text = $"{ferryConfig.velocitiesLevels[level].windSpeed} / mph";
        levelTxt.text = $"{(DifficultyLevel)level}";
        Destination.OnFerryMissedDockEvent += OnFerryMissedDock;
        Destination.OnEnterDockEvent += OnEnterDock;
        BoatController.OnSpeedThresholdCrossedEvent += OnSpeedCrossed;
        Act.SpeedInit += SetRange;
        Act.ReachedDestination += LevelFinish;
        Act.BoatDestroyedAction += BoatDestroyed;
    }

    private void TimeOut()
    {
        InfoTxt.text = "Opps...! Time out";
        if (dockMissedRoutine != null)
        {
            StopCoroutine(dockMissedRoutine);
            dockMissedRoutine = null;
        }
        dockMissedRoutine = StartCoroutine(ShowMissedDock());
    }

    private void OnSpeedCrossed(float speed)
    {
        InfoTxt.text = $"You are crossing speed limit, It will deduct your points..! Keep it under {speed}/mph";
        if (dockMissedRoutine != null)
        {
            StopCoroutine(dockMissedRoutine);
            dockMissedRoutine = null;
        }
        dockMissedRoutine = StartCoroutine(ShowMissedDock());
    }

    private void OnEnterDock()
    {
        InfoTxt.text = "You entered in the dock area";
        if (dockMissedRoutine != null)
        {
            StopCoroutine(dockMissedRoutine);
            dockMissedRoutine = null;
        }
        dockMissedRoutine = StartCoroutine(ShowMissedDock());
    }

    private void OnFerryMissedDock()
    {
        InfoTxt.text = "You missed the dock";
        if (dockMissedRoutine != null)
        {
            StopCoroutine(dockMissedRoutine);
            dockMissedRoutine = null;
        }
        dockMissedRoutine = StartCoroutine(ShowMissedDock());
    }
    private IEnumerator ShowMissedDock()
    {
        Info.DOFade(1, 1f);
        yield return new WaitForSeconds(5f);
        Info.DOFade(0, 1f).SetEase(Ease.InBounce);
    }
    private void OnDisable()
    {
        scrollAction.Disable();
        TimerAndScore.TimeOutEvent -= TimeOut;
        Act.SpeedInit -= SetRange;
        Destination.OnEnterDockEvent -= OnEnterDock;
        Destination.OnFerryMissedDockEvent -= OnFerryMissedDock;
        BoatController.OnSpeedThresholdCrossedEvent -= OnSpeedCrossed;
        Act.ReachedDestination -= LevelFinish;
    }

    private void BoatDestroyed()
    {
        Time.timeScale = 0;
        GamePopUp.Instance.FinalPopUp("Boat destroyed !!");

        DOVirtual.DelayedCall(2.0f, () => { Time.timeScale = 1; });
    }

    public void ExitAction()
    {
        Time.timeScale = 1;

    }

    private void OnDestroy()
    {
        scrollAction.performed -= OnScroll;
    }

    private void LevelFinish()
    {
        Time.timeScale = 0;
        GamePopUp.Instance.FinalPopUp("Destination Reached");
    }

    public void SetRange(float range)
    {
        rangeMin = 0;
        rangeMax = range;
    }

    private void OnScroll(InputAction.CallbackContext ctx)
    {
        if (rangeMax <= 0)
            return;

        Vector2 scroll = ctx.ReadValue<Vector2>();

        float mScrollVal = -1 * scroll.y;

        mScrollRange = Mathf.Clamp(mScrollRange + mScrollVal * sensitivity, rangeMin, rangeMax);

        Act.SpeedChange?.Invoke(mScrollRange);
    }

}
