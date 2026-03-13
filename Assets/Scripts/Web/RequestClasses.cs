using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GF;
using Netconfig;
using Newtonsoft.Json;

namespace Ferry_boat.Assets.Scripts.Web
{
    public class SignInRequest
    {
        public string username;
        public SignInRequest(string userName)
        {
            username = userName;
        }
    }
    public class Request<T> : IRequest where T : class
    {
        public RequestType RequestType { get; private set; }

        public string RequestData { get; private set; }
        public Request(RequestType requestType, T data)
        {
            RequestType = requestType;
            RequestData = JsonConvert.SerializeObject(data);
        }
    }
}