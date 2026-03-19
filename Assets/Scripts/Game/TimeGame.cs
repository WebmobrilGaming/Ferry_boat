using System.Collections;
using UnityEngine;
using TMPro;
using FerryBoat;
using System;

public class TimerAndScore : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private float timer = 0f;
    private int score = 0;
    private int lastMinute = 0;


    bool enableScore = false;

    private void OnEnable()
    {
        enableScore = false;
        Act.EnableScore += EnableScore;
    }

    private void OnDisable()
    {
        Act.EnableScore -= EnableScore;
    }

    private void EnableScore(bool enable)
    {
        enableScore = enable;
    }

    private void Update()
    {
        if (!enableScore)
            return;

        timer += Time.deltaTime;
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
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer % 60f);
        timerText.text = $"{minutes:00} : {seconds:00}";
    }
}