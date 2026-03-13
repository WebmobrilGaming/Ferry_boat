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
    public class RequestData: Request 
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
}