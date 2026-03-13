using System.Collections.Generic;
using UnityEngine;

namespace GF
{
    public enum LogType
    {
        Log = 0,
        Error = 1,
        HttpRequest = 2,
        HttpResponse = 3,
        Warning = 4
    }
    public static class Logger 
    {
        private static Dictionary<LogType, string> logColoursDictionary = new Dictionary<LogType, string>
        {
                {LogType.Error,"#FF0909"},//Red
                {LogType.Log,"#FFFFFF"},// white
                {LogType.HttpRequest,"#FFEC00"},//yellow
                {LogType.HttpResponse,"#00FF15"},//green
                {LogType.Warning,"#00F6FF"}//cyan
        };
        public static void Log(LogType logType, string msgBody)
        {
            Debug.Log($"<color={logColoursDictionary[logType]}>{logType.ToString()} : {msgBody}</color>");
        }
    }
}
