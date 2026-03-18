using System;

namespace GF
{
    public static class APIManager
    {
        public static void GetAPI<T>(Request request, Action<T> Callback)
        {
            Utils.CallEventAsync(new ApiEvent(HttpRequestType.GET, request, (res) =>
            {
                Callback?.Invoke((T)res);
            }));
        }
        public static void PostAPI<T>(Request request, Action<T> Callback)
        {
            Utils.CallEventAsync(new ApiEvent(HttpRequestType.POST, request, (res) =>
            {
                Callback?.Invoke((T)res);
            }));
        }
        public static void PutAPI<T>(Request request, Action<T> Callback)
        {
            Utils.CallEventAsync(new ApiEvent(HttpRequestType.PUT, request, (res) =>
            {
                Callback?.Invoke((T)res);
            }));
        }
        public static void DeleteAPI<T>(Request request, Action<T> Callback)
        {
            Utils.CallEventAsync(new ApiEvent(HttpRequestType.DELETE, request, (res) =>
            {
                Callback?.Invoke((T)res);
            }));
        }
    }
}