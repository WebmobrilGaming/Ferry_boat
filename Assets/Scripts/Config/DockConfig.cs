using System;
using System.Collections.Generic;
using UnityEngine;
namespace Ferry.Config
{
    [CreateAssetMenu(fileName = "DockConfig", menuName = "Config/DockConfig")]
    public class DockConfig : ScriptableObject
    {
        private static DockConfig instance;
        public static DockConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    // If the instance is null, try to load from resources
                    instance = Resources.Load<DockConfig>("DockConfig");

                    // If still null, create a new instance
                    if (instance == null)
                    {
                        instance = CreateInstance<DockConfig>();
                    }
                }

                return instance;
            }
        }
        public List<Dock> docks;
        private Dictionary<DockSet, Dock> dockMap = new Dictionary<DockSet, Dock>();
        public void BuildDock()
        {
            if (docks == null || docks.Count == 0)
            {
                return;
            }
            foreach (var d in docks)
            {
                dockMap[d.dockSet] = d;
            }
        }
        public Dock GetDock(DockSet dockSet)
        {
            if(dockMap==null || dockMap.Count == 0)
            {
                BuildDock();
            }
            if (dockMap.TryGetValue(dockSet, out Dock dock))
            {
                return dock;
            }
            return null;
        }
    }
    [Serializable]
    public class Dock
    {
        public DockSet dockSet;
        public Vector3 dockPosition;
    }
    public enum DockSet
    {
        CanadianDock1,
        CanadianDock2,
        CanadianDock3,
        CanadianDock4,
        CanadianDock5,
        CanadianDock6,
        CanadianDock7,
        AmericanDock1,
        AmericanDock2,
        AmericanDock3,
        AmericanDock4,
        AmericanDock5,
        AmericanDock6
    }
}
