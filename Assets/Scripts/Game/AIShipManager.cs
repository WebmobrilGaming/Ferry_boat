using Ferry.Config;
using Ferry.Loading;
using Ferry.Ship;
using FerryBoat.Actions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIShipManager : MonoBehaviour
{
    private DifficultyLevel level;

    public List<GameObject> mShips = new List<GameObject>();

    void Awake()
    {
        this.level=GetDifficultyLevel();
        
    }

    private void OnEnable()
    {
        Act.EnableUser += EnableShipAction;
    }

    private void OnDisable()
    {
        Act.EnableUser -= EnableShipAction;
    }

    private void EnableShipAction(bool enable)
    {
        if (!enable)
            return;

        SpawnShips();      
    }

    public void SpawnShips()
    {
        mShips.ForEach(sh => sh.SetActive(false));

        int count = DifficultyLevel.hard switch
        {
            DifficultyLevel.easy => 1,
            DifficultyLevel.medium => 2,
            DifficultyLevel.hard =>3
        };

        List<GameObject> ships = mShips.Take(count).ToList();
        ships.ForEach(sh => sh.SetActive(true));
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
