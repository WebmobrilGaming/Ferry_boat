using System.Collections;
using UnityEngine;
using TMPro;
using FerryBoat.Actions;
using System;
using Newtonsoft.Json;
using DG.Tweening;
using GF;
using Ferry_boat.Assets.Scripts.Web;
using FerryBoat.Store;
using UnityEngine.SceneManagement;
using Ferry.Loading;

public class TimerAndScore : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;

    public float timer = 0f;


    private int lastSecond = 0;

    [SerializeField] DifficultyLevel difficultyLevel;
    public bool enableScore = false;
    public int easyMaxTime;
    public int mediumMaxTime;
    public int hardMaxTime;
    private int maxTime;
    private bool isTimeOut = false;
    private bool IsInitialized = false;
    private bool isLevelFinished=false;
    private bool timerAlreadyStarted;

    int timeLimit;

    private void OnEnable()
    {
        enableScore = false;
        timerAlreadyStarted = false;
        Act.EnableScore += EnableScore;
        Act.ReachedDestination += LevelFinish;
        Act.BoatDestroyedAction += LevelFinish;
        Act.BoatDestroyedAction += ToHomeScreen;
        Act.EndPointReached += OnEndPointReached;

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
        maxTime = difficultyLevel switch
        {
            DifficultyLevel.easy => easyMaxTime,
            DifficultyLevel.medium => mediumMaxTime,
            DifficultyLevel.hard => hardMaxTime,
            _ => easyMaxTime
        };
    }

    private void ToHomeScreen()
    {
        StartCoroutine(EndGame());
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSecondsRealtime(10f);
        Debug.LogWarning("!!!!!!to home screen comment here !!!!");
        LoadingScreen.Instance.LoadSceneAsync(SceneEnum.Home,SceneEnum.Game);
        
    }

    private void SetTimeLimit(int time)
    {
        timeLimit = time;
    }

    private void LevelFinish()
    {
        isLevelFinished=true;
    }

    private void OnDisable()
    {
        Act.EnableScore -= EnableScore;
        Act.ReachedDestination -= LevelFinish;
        Act.BoatDestroyedAction -= LevelFinish;
        Act.BoatDestroyedAction -= ToHomeScreen;
    }

    private void EnableScore()
    {
        enableScore = true;  // = enable;
        IsInitialized = true;

         if (!timerAlreadyStarted)
        {
            //if (enable)
                timer = maxTime;
                lastSecond = Mathf.FloorToInt(timer);

            Data.time = timer;
            timerAlreadyStarted = true;
            Debug.LogWarning("Timer Started");
        }
        
    }

    private void Update()
    {
        if (!IsInitialized && !enableScore)
        {
            return;
        }
        if (isLevelFinished) return;

        timer -= Time.deltaTime;
        timer = Mathf.Clamp(timer,0,660);
        Data.time = timer;

        if (timer <=0 )
        {
            if (!isTimeOut)
            {
                isTimeOut = true;
                Utils.ShowInGamePopup("Opps...! Time out");
            }
        }
        UpdateTimerUI();
        CheckMinutePassed();
    }

    private void CheckMinutePassed()
    {
        int currentSecond = Mathf.FloorToInt(timer);
        if (currentSecond < lastSecond)
        {
            lastSecond = currentSecond;
            AddScore(1);
        }
    }

    private void AddScore(int amount)
    {
        if (timer >= maxTime) return;
        //score += amount;
        // scoreText.text = $"{score}";
        // if(timer<=0)
        // {
        //     // Debug.LogWarning("Behind scheduled time");
        //     // return;
        // }
        Score_System.Instance.Set(amount,Data.time);
    }

    private void OnScoreUpdate(UpdateScoreResponse data, Response response)
    {
        
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        timerText.text = $"{minutes:00} : {seconds:00}";
        
    }
    private void OnEndPointReached()
    {
        StartCoroutine(IEndPointReached());
    }

    IEnumerator IEndPointReached()
    {
        yield return new WaitForSecondsRealtime(10f);
        Debug.LogWarning("!!!!!End Point Reached - Game Over!!!!");
        LoadingScreen.Instance.LoadSceneAsync(SceneEnum.Home, SceneEnum.Game);
    }

    
}