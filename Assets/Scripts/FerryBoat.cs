using System;
using UnityEngine;


using System.Collections.Generic;

namespace  FerryBoat
{
    namespace Actions
    {
        public static class Act
        {
            public static Action<float> SpeedChange;
            public static Action<float> SpeedInit;
            public static Action HitAction;
            public static Action ReachedDestination;
            public static Action BoatDestroyedAction;
            public static Action MainMenuAction;
            public static Action EnableScore; //<bool>EnableScore;
            public static Action EndPointReached;
            public static Action OffBoardAction;

            public static Action<bool> EnableUser;
        }
    }

    namespace Store
    {
        public static class Data
        {
            public static int passengerCount;
            public static int carCount;
            public static int truckCount;

            public static float time;

            public static BoardData boardData;
        }
    }
}

[Serializable]
public class BoardData
{
    public List<NPC> passengerData;
    public List<Vehicle> vehicleDatas;
}

