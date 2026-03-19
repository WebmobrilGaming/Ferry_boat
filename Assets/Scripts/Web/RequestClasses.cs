using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GF;
using Netconfig;
using Newtonsoft.Json;

namespace Ferry_boat.Assets.Scripts.Web
{
    public class CreatePlayer
    {
        public string Type { get; }
        public string UserName { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public CreatePlayer(string type,string userName,string firstName,string lastName)
        {
            this.LastName = lastName;
            this.FirstName = firstName;
            this.UserName = userName;
            this.Type = type;
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
        public long status { get; private set; }

        public string message { get; private set; }
        public ResponseBody(long status, string message)
        {
            this.status = status;
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

        public long status => throw new NotImplementedException();
    }
}