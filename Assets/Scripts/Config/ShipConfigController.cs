using System.Collections.Generic;
using UnityEngine;
namespace Ferry.Config
{
    [CreateAssetMenu(fileName = "ShipConfigController", menuName = "Config/ShipConfigController")]
    public class ShipConfigController : ScriptableObject
    {
        private static ShipConfigController instance;
        public static ShipConfigController Instance
        {
            get
            {
                if (instance == null)
                {
                    // If the instance is null, try to load from resources
                    instance = Resources.Load<ShipConfigController>("ShipConfigController");

                    // If still null, create a new instance
                    if (instance == null)
                    {
                        instance = CreateInstance<ShipConfigController>();
                    }
                }

                return instance;
            }
        }
        public List<ShipConfig> ShipConfigs;
        private Dictionary<ShipType, ShipConfig> shipMap=new Dictionary<ShipType, ShipConfig>();

        public void BuildMap()
        {
            if (ShipConfigs == null || ShipConfigs.Count == 0) return;
            foreach (var config in ShipConfigs)
                shipMap[config.shipType] = config;
        }

        public ShipConfig GetShipConfig(ShipType shipType)
        {
            if(shipMap.TryGetValue(shipType,out ShipConfig shipConfig))
            {
                return shipConfig;
            }
            return null;
        }
    }
}