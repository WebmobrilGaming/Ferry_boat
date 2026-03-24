using System;
using System.Collections.Generic;
using GF;
using Netconfig;
using Newtonsoft.Json;
using static GF.UnityWebService;

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
    }

    public class LeaderboardResponse : Response
    {
        public bool success { get; set; }
        public string message { get; set; }
        public List<PlayerData> data { get; set; }
        public long code { get; set; }
        public bool status => success;
    }
}