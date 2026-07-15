using DG.Tweening;
using Ferry.Config;
using Ferry.Loading;
using Ferry.Popup;
using FerryBoat.Actions;
using FerryBoat.Store;
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
    bool crashSoundPlayed;
    float rangeMin = 0f;
    float rangeMax = 10f;
    [SerializeField] float sensitivity = 1f;
    public CanvasGroup Info;
    public TMP_Text InfoTxt;
    private Coroutine popupRoutine;
    public Button backBtn;
    public TMP_Text windTxt;
    public TMP_Text levelTxt;
    public string currentDifficulty;
    private InGamePopupEvent popupEvent = null;
    [SerializeField] private bool is_MouseControl;

    public static event Action StopEngineEvent;
    private void Awake()
    {
        rangeMax = -1;
        Debug.LogWarning("Mouse scroll controller here");
        if (is_MouseControl)
        {
            scrollAction.performed += OnScroll; //mouse input
        }
       
        
    }

    public void GoHome()
    {
        Time.timeScale = 0;
        GamePopUp.Instance.PopControl("Are you sure you wanna quit?", () =>
        {
            
            Score_System.Instance.Set(Score_System.Instance.Score,Data.time, () =>
            {
                LoadingScreen.Instance.LoadSceneAsync(SceneEnum.Home, SceneEnum.Game);
            }); 
        }, 
        () =>
        {

        });
       
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
        //StartCoroutine(BackButtonEnabler());
        EventManager.Instance.AddListener<InGamePopupEvent>(OnPopupEventArrive);
        var ferryConfig = ShipConfigController.Instance.GetShipConfig(ShipType.Ferry);
        int level = (int)GetDifficultyLevel();
        windTxt.text = $"{ferryConfig.velocitiesLevels[level].windSpeed} / mph";
        levelTxt.text = $"{(DifficultyLevel)level}";
        Act.SpeedInit += SetRange;
        Act.ReachedDestination += LevelFinish;
        Act.BoatDestroyedAction += BoatDestroyed;
        Act.EndPointReached+= EndPointReached;
        backBtn.onClick.AddListener(GoHome);
        currentDifficulty = PlayerPrefs.GetString(GamePrefs.difficulty_Level,DifficultyLevel.easy.ToString());
        crashSoundPlayed = false;
    }

    IEnumerator BackButtonEnabler()
    {
        backBtn.enabled = false;
        yield return new WaitForSecondsRealtime(5f);
        backBtn.enabled = true;
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
        Act.BoatDestroyedAction-=BoatDestroyed;
        Act.EndPointReached-=EndPointReached;
        backBtn.onClick.RemoveListener(GoHome);
    }
    private void BoatDestroyed()
    {
        GamePopUp.Instance.FinalPopUp("Boat destroyed !!");
        int SfxOn = PlayerPrefs.GetInt(GamePrefs.isSFXOn,1);
       
        if (SfxOn == 1 && crashSoundPlayed == false)
        {
            crashSoundPlayed = true;
            AudioManager.Instance.PlaySFX(AudioState.crash);
        }
        

        StopEngineEvent?.Invoke();
    }
    private void EndPointReached()
    {
        GamePopUp.Instance.FinalPopUp("EndPoint Reached");
        StopEngineEvent?.Invoke();
    }

    public void ExitAction()
    {
        Time.timeScale = 1;

    }

    private void OnDestroy()
    {
        if (is_MouseControl)
        {
            scrollAction.performed -= OnScroll;
        }
         
    }

    private void LevelFinish()
    {
        StopEngineEvent?.Invoke();
        GamePopUp.Instance.FinalPopUp("Destination Reached");
        StartCoroutine(ReachedDestinationDock());
    }

    IEnumerator ReachedDestinationDock()
    {
        yield return new WaitForSecondsRealtime(5f);
        //Score_System.Instance.UploadFinalScore( Data.time, () =>
        //    {
        //        LoadingScreen.Instance.LoadSceneAsync(SceneEnum.Home, SceneEnum.Game);
        //    });

        Act.OffBoardAction?.Invoke();
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
