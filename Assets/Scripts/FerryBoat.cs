using System;
using UnityEngine;

namespace  FerryBoat
{
   public static  class Act
   {  
       public static Action<float> SpeedChange;
       public static Action<float> SpeedInit;
       public static Action HitAction;
       public static Action ReachedDestination;
       public static Action BoatDestroyedAction;
       public static Action MainMenuAction;
       public static Action<bool> EnableScore;
   }
}
