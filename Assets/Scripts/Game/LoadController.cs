using System.Collections.Generic;
using Ferry.Config;
using UnityEngine;
namespace Ferry.Controller
{
    public class LoadController : MonoBehaviour
    {
        Dictionary<LoadType,LoadData> loadMap=new Dictionary<LoadType, LoadData>();
        void Awake()
        {
            loadMap=LoadConfig.Instance.GetLoadMap();
        }
        public void Load(LoadType loadType)
        {
            
        }
    }
}
