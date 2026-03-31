using System;
using Ferry.Config;
using Ferry.Loading;
using Ferry.Ship;
using Newtonsoft.Json;
using UnityEngine;

public class AIShipManager : MonoBehaviour
{
    public Transform shipParent;
    private DifficultyLevel level;
    private ShipLevel shipLevelData;
    void Awake()
    {
        this.level=GetDifficultyLevel();
        shipLevelData =ShipsLevelConfig.Instance.shipLevels[(int)level];
        SpawnShips();
    }

    private void SpawnShips()
    {
        foreach (var s in shipLevelData.ships)
        {
            var ship=Instantiate(s.ship,shipParent);
            AIController aIController=ship.GetComponent<AIController>();
            aIController.SetPath(s.start,s.end);
        }
        LoadingScreen.Instance.StopLoading();
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
}
