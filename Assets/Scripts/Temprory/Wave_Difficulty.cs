using DynamicWeatherSystem;
using Ferry.Loading;
using Ferry.Motion;
using Ferry.Screens;
using Ferry_boat.Assets.Scripts.Web;
using FerryBoat.Store;
using GF;
using Netconfig;
using Newtonsoft.Json;
using System;
using UnityEngine;

public class Wave_Difficulty : MonoBehaviour
{
    public static int windspeed;
    public static int wavelevel;
    public string currentdifficulty;

    [Header("Enivorment Settings:")]
    [SerializeField] WeatherManager weatherManager;

    [Space]
    [SerializeField] WeatherStateData calmWeather;
    [SerializeField] WeatherStateData rainWeather;
    [SerializeField] WeatherStateData stormWeather;

    [Space]
    [SerializeField] GameObject mCalmWater;
    [SerializeField] GameObject mRainyWater;
    [SerializeField] GameObject mStormWater;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        APIManager.GetAPI<WaveDifficultyResponse>(new RequestData(Netconfig.RequestType.Wave_Diffiulty, null), OnRecieveWaveDifficulty);
        currentdifficulty = PlayerPrefs.GetString(GamePrefs.difficulty_Level,DifficultyLevel.easy.ToString());
        

    }

    private void OnRecieveWaveDifficulty(WaveDifficultyResponse data, Response response)
    {
        if(!response.status)
        {
            Debug.LogError("Unable to fetch the difficulty");

            GamePopUp.Instance.PopStat("Seomthing went on difficulty !!", 2.0f, () =>
            {
                LoadingScreen.Instance.LoadSceneAsync(SceneEnum.Home, SceneEnum.Game);
            });

            return;
        }


        if (response.status)
        {
            WeatherStateData stateData = calmWeather;

            switch(currentdifficulty)
            {
                case nameof(DifficultyLevel.easy):

                    windspeed = data.data.windSpeedByDifficulty.easy;
                    wavelevel = data.data.waveLevelByDifficulty.easy;
                    stateData = calmWeather;

                    mCalmWater.SetActive(true);
                    mRainyWater.SetActive(false);
                    mStormWater.SetActive(false);
                    WaveMotion.wave_MaxValue = 1f;

                    break;

                case nameof(DifficultyLevel.medium):

                    windspeed = data.data.windSpeedByDifficulty.medium;
                    wavelevel = data.data.waveLevelByDifficulty.medium;
                    stateData = rainWeather;

                    mCalmWater.SetActive(false);
                    mRainyWater.SetActive(true);
                    mStormWater.SetActive(false);
                    WaveMotion.wave_MaxValue = 1.5f;

                    break;


                case nameof(DifficultyLevel.hard):

                    windspeed = data.data.windSpeedByDifficulty.hard;
                    wavelevel = data.data.waveLevelByDifficulty.hard;
                    stateData = stormWeather;
                    WaveMotion.wave_MaxValue = 6f;

                    mCalmWater.SetActive(false);
                    mRainyWater.SetActive(false);
                    mStormWater.SetActive(true);
                    break;

                default:
                    windspeed = data.data.windSpeedByDifficulty.easy;
                    wavelevel = data.data.waveLevelByDifficulty.easy;
                    stateData = calmWeather;
                    WaveMotion.wave_MaxValue = 1f;

                    mCalmWater.SetActive(true);
                    mRainyWater.SetActive(false);
                    mStormWater.SetActive(false);

                    break;

            }

            //if (currentdifficulty == DifficultyLevel.easy.ToString())
            //{
            //    windspeed = data.data.windSpeedByDifficulty.easy;
            //    wavelevel = data.data.waveLevelByDifficulty.easy;
            //    stateData = calmWeather;

            //    mWater.materials[0] = mCalmWater;
            //}
            //else if (currentdifficulty == DifficultyLevel.medium.ToString())
            //{
            //    windspeed = data.data.windSpeedByDifficulty.medium;
            //    wavelevel = data.data.waveLevelByDifficulty.medium;
            //    stateData = rainWeather;

            //    mWater.materials[0] = mRainyWater;
            //}
            //else if( currentdifficulty == DifficultyLevel.hard.ToString())
            //{
            //    windspeed = data.data.windSpeedByDifficulty.hard;
            //    wavelevel = data.data.waveLevelByDifficulty.hard;
            //    stateData = stormWeather;

            //    mWater.materials[0] = mStormWater;
            //}

            weatherManager.SetWeather(stateData);

            Debug.LogWarning($"current level : {currentdifficulty} \n windspeed : {windspeed} \n wavelevel : {wavelevel}");
            
        }
    }
}
