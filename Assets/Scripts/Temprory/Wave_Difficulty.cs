using System;
using Ferry_boat.Assets.Scripts.Web;
using GF;
using Netconfig;
using UnityEngine;
using Newtonsoft.Json;

public class Wave_Difficulty : MonoBehaviour
{
    public static int windspeed;
    public static int wavelevel;
    public string currentdifficulty;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        APIManager.GetAPI<WaveDifficultyResponse>(new RequestData(Netconfig.RequestType.Wave_Diffiulty, null), OnRecieveWaveDifficulty);
        currentdifficulty = PlayerPrefs.GetString(GamePrefs.difficulty_Level,DifficultyLevel.easy.ToString());
        

    }

    private void OnRecieveWaveDifficulty(WaveDifficultyResponse data, Response response)
    {
        if (response.status)
        {
            
            if (currentdifficulty == DifficultyLevel.easy.ToString())
            {
                windspeed = data.data.windSpeedByDifficulty.easy;
                wavelevel = data.data.waveLevelByDifficulty.easy;
            }
            else if (currentdifficulty == DifficultyLevel.medium.ToString())
            {
                windspeed = data.data.windSpeedByDifficulty.medium;
                wavelevel = data.data.waveLevelByDifficulty.medium;
            }
            else if( currentdifficulty == DifficultyLevel.hard.ToString())
            {
                windspeed = data.data.windSpeedByDifficulty.hard;
                wavelevel = data.data.waveLevelByDifficulty.hard;
            }
                
                Debug.LogWarning($"current level : {currentdifficulty} \n windspeed : {windspeed} \n wavelevel : {wavelevel}");
            
        }
    }
}
