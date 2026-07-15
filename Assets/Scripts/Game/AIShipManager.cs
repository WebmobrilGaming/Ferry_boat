using System;
using Ferry.Config;
using Ferry.Loading;
using Ferry.Ship;
using Newtonsoft.Json;
using UnityEngine;

using System.Collections.Generic;
using System.Linq;

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
        SpawnShips();
    }

    private void SpawnShips()
    {
        mShips.ForEach(sh => sh.SetActive(false));

        int count = level switch
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
