using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ferry.Config
{
    [CreateAssetMenu(fileName = "ShipsLevelConfig", menuName = "Config/ShipsLevelConfig")]
    public class ShipsLevelConfig : ScriptableObject
    {
        private static ShipsLevelConfig instance;
        public static ShipsLevelConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    // If the instance is null, try to load from resources
                    instance = Resources.Load<ShipsLevelConfig>("ShipsLevelConfig");

                    // If still null, create a new instance
                    if (instance == null)
                    {
                        instance = CreateInstance<ShipsLevelConfig>();
                    }
                }

                return instance;
            }
        }
        public ShipLevel[] shipLevels;
    }
    [Serializable]
    public class ShipLevel
    {
        public ShipsData[] ships;
    }
    [Serializable]
    public class ShipsData
    {
        public GameObject ship;
        public DockSet start;
        public DockSet end;
    }
}