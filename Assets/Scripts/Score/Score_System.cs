using Ferry_boat.Assets.Scripts.Web;
using GF;
using NUnit.Framework;
using System;
using TMPro;
using UnityEngine;

public class Score_System : MonoBehaviour
{
    [SerializeField] TMP_Text mScore;

    int score;

    static Score_System instance;

    public static  Score_System Instance {  get { return instance; } }

    private void Awake()
    {
        if(instance == null)
         instance = this;
    }

    private void OnEnable()
    {
        score = 0;
    }

    public void Set(int inx,Action onComplete = null)
    {  
        score =  score + inx;
        mScore.text =  score.ToString();

        var request = new UpdateScoreRequest(UserDataManager.Instance.UserDetails.data.username, score);
        APIManager.PostAPI<UpdateScoreResponse>(new RequestData(Netconfig.RequestType.UpdateScore, request), (res) =>
        {
            onComplete?.Invoke();
        });
    }
}

public enum ScoreState { add, overSpeed }
