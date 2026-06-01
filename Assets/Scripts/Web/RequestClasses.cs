using System;
using System.Collections.Generic;
using GF;
using Netconfig;
using Newtonsoft.Json;

namespace Ferry_boat.Assets.Scripts.Web
{
    public class CreatePlayer
    {
        public string type { get; }
        public string username { get; }
        public string firstName { get; }
        public string lastName { get; }
        public CreatePlayer(string type, string userName, string firstName, string lastName)
        {
            this.lastName = lastName;
            this.firstName = firstName;
            this.username = userName;
            this.type = type;
        }
        public CreatePlayer(string type, string userName)
        {
            this.type = type;
            this.username = userName;
        }
    }
    public class RequestData : Request
    {
        public RequestType requestType { get; private set; }

        public string requestData { get; private set; }
        public RequestData(RequestType requestType, object data)
        {
            this.requestType = requestType;
            this.requestData = JsonConvert.SerializeObject(data);
        }
    }
    public class ResponseBody : Response
    {
        public long code { get; private set; }
        public string message { get; private set; }
        public bool status { get; private set; }
        public ResponseBody(long status, string message)
        {
            this.code = status;
            this.status = code == 200;
            this.message = message;
        }
    }
    public class NewUserData
    {
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int score { get; set; }
        public string difficultyLevel { get; set; }
        public DateTime time { get; set; }
    }

    public class UserDetails : Response
    {
        public bool success { get; set; }
        public string message { get; set; }
        public NewUserData data { get; set; }

        public long code { get; set; }

        public bool status => success;
    }
    public class PlayerData
    {
        public string _id { get; set; }
        public int rank { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }
        public int score { get; set; }
        public string difficultyLevel { get; set; }
        public float totalTimeSecond {get ; set;}
        public DateTime time { get; set; }
    }
    // public class WindSpeedByDifficulty
    // {
    //     public int easy { get;set;}
    //     public int medium {get; set;}
    //     public int hard {get;set;}
    // }
    // public class WaveLevelRange
    // {
    //     public int minWave { get; set; }
    //     public int maxWave { get; set; }
    // }
    // public class WaveSpeedLimitRange
    // {
    //     public int min{get;set;}
    //     public int max{get;set;}
    // }
    // public class WavesSpeedLimit
    // {
    //     public WaveSpeedLimitRange easy{get;set;}
    //     public WaveSpeedLimitRange medium{get;set;}
    //     public WaveSpeedLimitRange hard{get;set;}
    // }
    // public class WaveDifficultyData
    // {
    //     public string waveDifficulty{get;set;}
    //     public WaveLevelRange waveLevelRange { get; set; }

    //     public int waveLevel {get; set;}

    //     public WindSpeedByDifficulty windSpeedByDifficulty {get;set;}
    // }[Serializable]
    public class MinMax
    {
        public int min { get; set; }
        public int? max { get; set; }
    }

    [Serializable]
    public class WaveLevelRange
    {
        public int minWave { get; set; }
        public int maxWave { get; set; }
    }

    [Serializable]
    public class WindSpeedLimits
    {
        public MinMax easy { get; set; }
        public MinMax medium { get; set; }
        public MinMax hard { get; set; }
    }

    [Serializable]
    public class WindSpeedByDifficulty
    {
        public int easy { get; set; }
        public int medium { get; set; }
        public int hard { get; set; }
    }
    [Serializable]
    public class WaveLevelByDifficulty
    {
        public int easy {get;set;}
        public int medium {get;set;}
        public int hard {get;set;}
    }

    [Serializable]
    public class WaveDifficultyData
    {
        public string waveDifficulty { get; set; }

        public WaveLevelRange waveLevelRange { get; set; }

        public int waveLevel { get; set; }
        public WaveLevelByDifficulty waveLevelByDifficulty {get;set;}

        public WindSpeedLimits windSpeedLimits { get; set; }

        public WindSpeedLimits windSpeedRanges { get; set; }

        public MinMax windSpeedRange { get; set; }

        public WindSpeedByDifficulty windSpeedByDifficulty { get; set; }

        public int windSpeed { get; set; }
    }

    public class LeaderboardResponse : Response
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<PlayerData> data { get; set; }
        public long code { get; set; }
        public bool status => success;
    }

    public class UpdateScoreRequest
    {
        public UpdateScoreRequest(string username, int score,int time,string currentDifficulty)
        {
            this.username = username;
            this.score = score;
            this.difficultyLevel = currentDifficulty;
            this.totalTimeSecond = time;
        }

        public string username { get; set; }
        public int score { get; set; }

        public int totalTimeSecond { get; set; }
        public string difficultyLevel{get;set;}
    }
    public class UpdateScoreResponse : Response
    {
        public bool success { get; set; }
        public string message { get; set; }
        public PlayerData data { get; set; }
        public long code { get; set; }
        public bool status => success;
    }

    public class WaveDifficultyResponse : Response
    {
        public bool success { get; set; }
        public string message { get; set; }
        public WaveDifficultyData data {get;set;}
        public long code { get; set; }
        public bool status => success;
    }


}