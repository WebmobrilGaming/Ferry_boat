using System;
using System.Collections.Generic;
using UnityEngine;
namespace Ferry.Config
{
    [CreateAssetMenu(fileName = "LoadConfig", menuName = "Config/LoadConfig")]
    public class LoadConfig : ScriptableObject
    {
        private static LoadConfig instance;
        public static LoadConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    // If the instance is null, try to load from resources
                    instance = Resources.Load<LoadConfig>("LoadConfig");

                    // If still null, create a new instance
                    if (instance == null)
                    {
                        instance = CreateInstance<LoadConfig>();
                    }
                }
                return instance;
            }
        }
        public LoadData[] loadDatas;
        private Dictionary<LoadType, LoadData> loadMap = new Dictionary<LoadType, LoadData>();
        public Dictionary<LoadType, LoadData> GetLoadMap()
        {
            if (loadMap.Count == 0)
                BuildMap();
            return loadMap;
        }
        public LoadData GetLoadData(LoadType loadType)
        {
            if (loadMap.Count == 0)
                BuildMap();
            if (loadMap.TryGetValue(loadType, out LoadData loadData))
            {
                return loadData;
            }
            return null;
        }
        private void BuildMap()
        {
            foreach (var l in loadDatas)
                loadMap[l.loadType] = l;
        }
    }
    [Serializable]
    public class LoadData
    {
        public LoadType loadType;
        public int points;
        public int timeToLoadUnLoad;
    }
    public enum LoadType
    {
        Passenger, Car, Truck
    }
}
