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

public class GameTimer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text timerText;

    public float timer = 0f;

    private int lastSecond = 0;

    [SerializeField] DifficultyLevel difficultyLevel;
    public bool enableScore = false;
    public int easyMaxTime;
    public int mediumMaxTime;
    public int hardMaxTime;
    private int maxTime;
    private bool isTimeOut = false;
    public bool IsTimeOut => isTimeOut;
    private bool IsInitialized = false;
    public static int negativePoints;
    [SerializeField] private bool isLevelFinished=false;
    private bool timerAlreadyStarted;
    private float TimeElapsedOffBoarding;
    private float lastsecondOffBoarding;

    int timeLimit;

    [SerializeField] bool _running  = false;

    static GameTimer instance;
    public static GameTimer Instance { get { return instance; }  }

    public bool isDrive = false;

    private float timeoutElapsed;
    private int lastCheckedSecond;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void OnEnable()
    {
        TimeElapsedOffBoarding=0;
        enableScore = false;
        timerAlreadyStarted = false;
        negativePoints=0;
        lastsecondOffBoarding=0;
       // Act.EnableScore += EnableTime;
        Act.ReachedDestination += LevelFinish;
        Act.BoatDestroyedAction += LevelFinish;
        //Act.BoatDestroyedAction += ToHomeScreen;
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

        isDrive = false;
    }

    private void OnDisable()
    {
        //Act.EnableScore -= EnableTime;
        Act.ReachedDestination -= LevelFinish;
        Act.BoatDestroyedAction -= LevelFinish;
        //Act.BoatDestroyedAction -= ToHomeScreen;
    }

    // private void ToHomeScreen()
    // {
    //     StartCoroutine(EndGame());
    // }

    // IEnumerator EndGame()
    // {
    //     yield return new WaitForSeconds(3.0f);
    //     Debug.LogWarning("!!!!!!to home screen comment here !!!!");
    //     LoadingScreen.Instance.LoadSceneAsync(SceneEnum.Home,SceneEnum.Game);
    // }

    private void SetTimeLimit(int time)
    {
        timeLimit = time;
    }

    private void LevelFinish()
    {
       // isLevelFinished=true;

        isDrive = false;
    }

    public void EnableTime()
    {
        enableScore = true;  // = enable;
        IsInitialized = true;

        if (!timerAlreadyStarted)
        {
            timer = maxTime;
            lastSecond = Mathf.FloorToInt(timer);

            Data.time = timer;
            timerAlreadyStarted = true;

            _running = true;

            Debug.LogWarning("Timer Started");
        }
    }

    private void Update()
    {
       
        if (!IsInitialized && !enableScore) return;
        if (isLevelFinished) return;
        if (!_running) return;
        if (BoardingManager.isOffBoardingComplete) return;

        timer -= Time.deltaTime;
        //timer = Mathf.Clamp(timer, 0, 660);
        Data.time = timer;
        
        if (BoardingManager.isOffBoardingStarted)
        {  
            TimeElapsedOffBoarding += Time.deltaTime;
            int currentTime = Mathf.FloorToInt(TimeElapsedOffBoarding);
            if (currentTime > lastsecondOffBoarding)
            {
                //addScorePositive();
                lastsecondOffBoarding = currentTime;
            }
            
           
        }

        UpdateTimerUI();

        if (timer <= 0)
        {
            if (!isTimeOut)
            {
                isTimeOut = true;
                timeoutElapsed = 0f;
                lastCheckedSecond = 0;

                Act.ShowWarn?.Invoke("Oops...! Time out");
            }

            timeoutElapsed += Time.deltaTime;

            int currentSecond = Mathf.FloorToInt(timeoutElapsed);

            if (currentSecond > lastCheckedSecond)
            {
                lastCheckedSecond = currentSecond;

                Debug.Log($"Timeout: {currentSecond} seconds");

                // Your code here
                CheckSomethingAfterTimeout(timer);
            }

            return; 
        }

        Score_System.Instance.Set(1, Data.time);
    }

    private void addScorePositive()
    {
        if (timer > 0)
        {
            AddScore(1);
        }
    }

    void CheckSomethingAfterTimeout(float time)
    {
        Score_System.Instance.Set(-1, Data.time);

        if (time < 0)
        {
            if(BoardingManager.isOffBoardingComplete) return;
            //negativePoints++;
        }
      
    }

    public void Pause() => _running = false;
    public void Resume() => _running = true;

    private void CheckMinutePassed()
    {
        int currentSecond = Mathf.FloorToInt(timer);
        if (currentSecond < lastSecond)
        {
            lastSecond = currentSecond;
            //AddScore(1);
        }
    }

    private void AddScore(int amount)
    {
        if (timer >= maxTime) return;
      
        Score_System.Instance.Set(amount,Data.time);
    }

    private void OnScoreUpdate(UpdateScoreResponse data, Response response)
    {
        
    }

    private void UpdateTimerUI()
    {
        int totalSeconds = Mathf.FloorToInt(Mathf.Abs(timer));
        int minutes = totalSeconds/60;
        int seconds = totalSeconds%60;
        string sign = timer < 0 ? "-":"";
        timerText.text = $"{sign}{minutes:00}:{seconds:00}";
        
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