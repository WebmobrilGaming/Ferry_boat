using DG.Tweening;
using Ferry.Config;
using Ferry.Loading;
using FerryBoat;
using GF;
using Newtonsoft.Json;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    private Coroutine popupRoutine;
    public Button backBtn;
    public TMP_Text windTxt;
    public TMP_Text levelTxt;
    private InGamePopupEvent popupEvent = null;

    public static event Action StopEngineEvent;
    private void Awake()
    {
        rangeMax = -1;
        scrollAction.performed += OnScroll;
        backBtn.AddListener(null, GoHome);
    }

    private void GoHome()
    {
        LoadingScreen.Instance.LoadSceneAsync(SceneEnum.Home, SceneEnum.Game);
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
        EventManager.Instance.AddListener<InGamePopupEvent>(OnPopupEventArrive);
        var ferryConfig = ShipConfigController.Instance.GetShipConfig(ShipType.Ferry);
        int level = (int)GetDifficultyLevel();
        windTxt.text = $"{ferryConfig.velocitiesLevels[level].windSpeed} / mph";
        levelTxt.text = $"{(DifficultyLevel)level}";
        Act.SpeedInit += SetRange;
        Act.ReachedDestination += LevelFinish;
        Act.BoatDestroyedAction += BoatDestroyed;
    }

    private void OnPopupEventArrive(InGamePopupEvent e)
    {
        this.popupEvent = e;
        InfoTxt.text = e.info;
        if (popupRoutine != null)
        {
            StopCoroutine(popupRoutine);
            popupRoutine = null;
        }
        popupRoutine = StartCoroutine(ShowPopupRoutine());
    }
    private IEnumerator ShowPopupRoutine()
    {
        Info.DOFade(1, 1f);
        yield return new WaitForSeconds(5f);
        Info.DOFade(0, 1f).SetEase(Ease.InBounce).OnComplete(() =>
        {
            popupEvent.SetIsDone();
            popupEvent=null;
        });
    }
    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<InGamePopupEvent>(OnPopupEventArrive);
        scrollAction.Disable();
        Act.SpeedInit -= SetRange;
        Act.ReachedDestination -= LevelFinish;
    }

    private void BoatDestroyed()
    {
        GamePopUp.Instance.FinalPopUp("Boat destroyed !!");
        AudioManager.Instance.PlaySFX(AudioState.crash);

        StopEngineEvent?.Invoke();
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
        StopEngineEvent?.Invoke();
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
