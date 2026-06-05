using Ferry_boat.Assets.Scripts.Web;
using GF;
using NUnit.Framework;
using System;
using TMPro;
using UnityEngine;

public class Score_System : MonoBehaviour
{
    [SerializeField] TMP_Text mScore;
    [SerializeField] int previousHighestScore;

    [SerializeField] int score;

    public int Score => score;

    static Score_System instance;
    public string currentDifficulty;

    public static  Score_System Instance {  get { return instance; } }

    private void Awake()
    {
        if(instance == null)
         instance = this;
    }

    private void OnEnable()
    {
        score = 0;
        currentDifficulty = PlayerPrefs.GetString(GamePrefs.difficulty_Level, DifficultyLevel.easy.ToString());
        PreviousHighestScore();
    }

    private void PreviousHighestScore()
    {
        if(currentDifficulty == DifficultyLevel.easy.ToString())
        {
            previousHighestScore = UserDataManager.Instance.UserDetails.data.scoresByDifficulty.easy.score;
        }
        else if(currentDifficulty == DifficultyLevel.medium.ToString())
        {
            previousHighestScore = UserDataManager.Instance.UserDetails.data.scoresByDifficulty.medium.score;
        }
        else if(currentDifficulty == DifficultyLevel.hard.ToString())
        {
            previousHighestScore = UserDataManager.Instance.UserDetails.data.scoresByDifficulty.hard.score;
        }
    }

    public void Set(int inx,float time = 0, Action onComplete = null)
    {  
        score =  score + inx;
        // Debug.LogWarning(score);
        mScore.text =  score.ToString();
        int timedata = (int)time;
        onComplete?.Invoke();
        // var request = new UpdateScoreRequest(UserDataManager.Instance.UserDetails.data.username, timedata, score ,currentDifficulty);
        // APIManager.PostAPI<UpdateScoreResponse>(new RequestData(Netconfig.RequestType.UpdateScore, request), (res,Response) =>
        // {
        //     onComplete?.Invoke();
        // });
    }
    public void UploadFinalScore( float time, Action onComplete = null)
    {
        
        int timedata = (int)time;
        if(score > previousHighestScore)
        {
            Debug.LogWarning(score);
            var request = new UpdateScoreRequest(UserDataManager.Instance.UserDetails.data.username, score, timedata, currentDifficulty);
            APIManager.PostAPI<UpdateScoreResponse>(new RequestData(Netconfig.RequestType.UpdateScore, request), (res, Response) =>
            {
                onComplete?.Invoke();
            });
        }
        else
        {
            onComplete.Invoke();
        }
        
    }
}

public enum ScoreState { add, overSpeed }
