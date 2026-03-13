using System;
using GF;

namespace Ferry_boat.Assets.Scripts.Web
{
    public static class APIManager
    {
        public static void GetAPI<T>(IRequest request, Action<T> Callback)
        {
            Utils.CallEventAsync(new ApiEvent(HttpRequestType.GET, request, (res) =>
            {
                Callback?.Invoke((T)res);
            }));
        }
        public static void PostAPI<T>(IRequest request, Action<T> Callback)
        {
            Utils.CallEventAsync(new ApiEvent(HttpRequestType.POST, request, (res) =>
            {
                Callback?.Invoke((T)res);
            }));
        }
        public static void PutAPI<T>(IRequest request, Action<T> Callback)
        {
            Utils.CallEventAsync(new ApiEvent(HttpRequestType.PUT, request, (res) =>
            {
                Callback?.Invoke((T)res);
            }));
        }
        public static void DeleteAPI<T>(IRequest request, Action<T> Callback)
        {
            Utils.CallEventAsync(new ApiEvent(HttpRequestType.DELETE, request, (res) =>
            {
                Callback?.Invoke((T)res);
            }));
        }
    }
}