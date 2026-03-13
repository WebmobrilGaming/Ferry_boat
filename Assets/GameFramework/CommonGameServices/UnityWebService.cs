using System;
using System.Collections;
using Netconfig;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

namespace GF
{
    public class UnityWebService : IService
    {
        private bool isUpdateRequired = false;
        private string BearerTokenKey = "BearerToken";
        public bool IsUpdateRequired => isUpdateRequired;
        public void Initialize()
        {
            Logger.Log(LogType.Log, "UnityWebService created");
            if (!PlayerPrefs.HasKey(BearerTokenKey))
            {
                PlayerPrefs.SetString(BearerTokenKey, "");
            }
        }

        public void RegisterListener()
        {
            EventManager.Instance.AddListener<ApiEvent>(OnApiRequest);
        }

        public void RemoveListener()
        {
            EventManager.Instance.RemoveListener<ApiEvent>(OnApiRequest);
        }

        private void OnApiRequest(ApiEvent e)
        {
            switch (e.httpRequestType)
            {
                case HttpRequestType.POST:
                    HTTPPost(e.url,
                        e.request.requestType,
                        e.request.requestData,
                        e.responseCallback);
                    break;
                case HttpRequestType.GET:
                    HTTPGet(e.url,
                        e.request.requestType,
                        e.responseCallback);
                    break;
                case HttpRequestType.PUT:
                    HTTPPut(e.url,
                    e.request.requestType,
                    e.request.requestData,
                    e.responseCallback);
                    break;
                case HttpRequestType.DELETE:
                    HTTPDelete(e.url,
                        e.request.requestType,
                        e.request.requestData,
                        e.responseCallback);
                    break;
            }
        }
        /// <summary>
        /// It Post data to server using url.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestType"></param>
        /// <param name="data"></param>
        /// <param name="OnPostCompleteCalback"></param>
        /// <param name="OnProgressCallback"></param>

        protected void HTTPPost(string url, RequestType requestType, string data, Action<Response> OnPostCompleteCalback)
        {
            Utils.CallEventAsync(new CoroutineEvent(PostRequest(url, requestType, data, OnPostCompleteCalback)));
        }

        private IEnumerator PostRequest(string url, RequestType requestType, string data, Action<Response> onPostCompleteCalback)
        {
            Logger.Log(LogType.HttpRequest, $"[Request-POST]-{requestType}-data-{data}");
            // Convert JSON string to bytes
            using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
                    // Attach upload handler with JSON body
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                }

                // Attach download handler
                request.downloadHandler = new DownloadHandlerBuffer();

                // Headers
                request.SetRequestHeader("Content-Type", "application/json");

                if (PlayerPrefs.HasKey("BearerToken") && !string.IsNullOrEmpty(PlayerPrefs.GetString("BearerToken")))
                    request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("BearerToken"));

                // Send request
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Logger.Log(LogType.HttpResponse, $"[Response]-{requestType}-data-{request.downloadHandler.text}");
                    onPostCompleteCalback?.Invoke(ServerConfig.Instance.MapResponse(request.downloadHandler.text,requestType));
                }
                else
                {
                    Logger.Log(LogType.Error, $"[Response]-{requestType}-data-{request.error}");
                    onPostCompleteCalback?.Invoke(ServerConfig.Instance.MapErrorResponse(request.responseCode, request.error));
                }
            }

        }

        /// <summary>
        /// It Get data from  server using url.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestType"></param>
        /// <param name="OnGetCompleteCallback"></param>
        /// <param name="progressCallback"></param>
        protected void HTTPGet(string url, RequestType requestType, Action<Response> OnGetCompleteCallback)
        {
            Utils.CallEventAsync(new CoroutineEvent(GetRequest(url, requestType, OnGetCompleteCallback)));
        }

        private IEnumerator GetRequest(string url, RequestType requestType, Action<Response> onGetCompleteCallback)
        {
            Logger.Log(LogType.HttpRequest, $"[Request-GET]-{requestType}");
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                // Headers
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("BearerToken"));

                // Send request
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Logger.Log(LogType.HttpResponse, $"[Response]-{requestType}-data-{request.downloadHandler.text}");
                    onGetCompleteCallback?.Invoke(ServerConfig.Instance.MapResponse(request.downloadHandler.text,requestType));
                }
                else
                {
                    Logger.Log(LogType.Error, $"[Response]-{requestType}-data-{request.error}");
                    onGetCompleteCallback?.Invoke(ServerConfig.Instance.MapErrorResponse(request.responseCode, request.error));
                }
            }
        }

        /// <summary>
        /// PUT REQUEST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestType"></param>
        /// <param name="data"></param>
        /// <param name="OnPutCompleteCallback"></param>
        protected void HTTPPut(string url, RequestType requestType, string data, Action<Response> OnPutCompleteCallback)
        {
            Utils.CallEventAsync(new CoroutineEvent(PutRequest(url, requestType, data, OnPutCompleteCallback)));
        }
        private IEnumerator PutRequest(string url, RequestType requestType, string data, Action<Response> onPostCompleteCalback)
        {
            Logger.Log(LogType.HttpRequest, $"[Request-POST]-{requestType}-data-{data}");
            // Convert JSON string to bytes
            using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPUT))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
                    // Attach upload handler with JSON body
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                }

                // Attach download handler
                request.downloadHandler = new DownloadHandlerBuffer();

                // Headers
                request.SetRequestHeader("Content-Type", "application/json");

                if (PlayerPrefs.HasKey("BearerToken") && !string.IsNullOrEmpty(PlayerPrefs.GetString("BearerToken")))
                    request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("BearerToken"));

                // Send request
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Logger.Log(LogType.HttpResponse, $"[Response]-{requestType}-data-{request.downloadHandler.text}");
                    onPostCompleteCalback?.Invoke(ServerConfig.Instance.MapResponse(request.downloadHandler.text,requestType));
                }
                else
                {
                    Logger.Log(LogType.Error, $"[Response]-{requestType}-data-{request.error}");
                    onPostCompleteCalback?.Invoke(ServerConfig.Instance.MapErrorResponse(request.responseCode, request.error));
                }
            }
        }
        /// <summary>
        /// DELETE REQUEST
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestType"></param>
        /// <param name="data"></param>
        /// <param name="OnPutCompleteCallback"></param>
        protected void HTTPDelete(string url, RequestType requestType, string data, Action<Response> OnPutCompleteCallback)
        {
            Utils.CallEventAsync(new CoroutineEvent(DeleteRequest(url, requestType, data, OnPutCompleteCallback)));
        }
        private IEnumerator DeleteRequest(string url, RequestType requestType, string data, Action<Response> onPostCompleteCalback)
        {
            Logger.Log(LogType.HttpRequest, $"[Request-POST]-{requestType}-data-{data}");
            // Convert JSON string to bytes
            using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbDELETE))
            {
                if (!string.IsNullOrEmpty(data))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
                    // Attach upload handler with JSON body
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                }

                // Attach download handler
                request.downloadHandler = new DownloadHandlerBuffer();

                // Headers
                request.SetRequestHeader("Content-Type", "application/json");

                if (PlayerPrefs.HasKey("BearerToken") && !string.IsNullOrEmpty(PlayerPrefs.GetString("BearerToken")))
                    request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("BearerToken"));

                // Send request
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Logger.Log(LogType.HttpResponse, $"[Response]-{requestType}-data-{request.downloadHandler.text}");
                    onPostCompleteCalback?.Invoke(ServerConfig.Instance.MapResponse(request.downloadHandler.text,requestType));
                }
                else
                {
                    Logger.Log(LogType.Error, $"[Response]-{requestType}-data-{request.error}");
                    onPostCompleteCalback?.Invoke(ServerConfig.Instance.MapErrorResponse(request.responseCode, request.error));
                }
            }
        }
        public void Update()
        {

        }
    }
}
