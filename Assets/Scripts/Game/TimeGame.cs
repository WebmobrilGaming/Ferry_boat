using System.Collections;
using UnityEngine;
using TMPro;
using FerryBoat;
using System;
using Newtonsoft.Json;
using DG.Tweening;
using GF;
using Ferry_boat.Assets.Scripts.Web;

public class TimerAndScore : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private float timer = 0f;
    private int score = 0;
    private int lastMinute = 0;

    [SerializeField] DifficultyLevel difficultyLevel;
    public bool enableScore = false;
    public int easyMaxTime;
    public int mediumMaxTime;
    public int hardMaxTime;
    private int maxTime;
    private bool isTimeOut = false;
    private bool IsInitialized = false;
    private void OnEnable()
    {
        enableScore = false;
        Act.EnableScore += EnableScore;
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

    private void OnDisable()
    {
        Act.EnableScore -= EnableScore;
    }

    private void EnableScore(bool enable)
    {
        enableScore = enable;
        IsInitialized = true;
    }

    private void Update()
    {
        if (!IsInitialized && !enableScore)
        {
            return;
        }
        timer += Time.deltaTime;
        if (timer >= maxTime)
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
        int currentMinute = Mathf.FloorToInt(timer / 60f);
        if (currentMinute > lastMinute)
        {
            lastMinute = currentMinute;
            AddScore(1);
        }
    }

    private void AddScore(int amount)
    {
        score += amount;
        scoreText.text = $"{score}";
        var request=new UpdateScoreRequest(UserDataManager.Instance.UserDetails.data.username,score);
        APIManager.PostAPI<UpdateScoreResponse>(new RequestData(Netconfig.RequestType.UpdateScore,request),OnScoreUpdate);
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
}