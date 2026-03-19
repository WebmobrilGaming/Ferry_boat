using System;
using Unity.VisualScripting;

namespace GF
{
    public static class APIManager
    {
        public static void GetAPI<T>(Request request, Action<T, Response> Callback)
        {
            Utils.CallEventAsync(new ApiEvent(HttpRequestType.GET, request, (res) =>
            {
                var result = MapResponse<T>(res);
                Callback?.Invoke(result.Item1, result.Item2);
            }));
        }
        public static void PostAPI<T>(Request request, Action<T, Response> Callback)
        {
            Utils.CallEventAsync(new ApiEvent(HttpRequestType.POST, request, (res) =>
            {
                var result = MapResponse<T>(res);
                Callback?.Invoke(result.Item1, result.Item2);

            }));
        }
        public static void PutAPI<T>(Request request, Action<T, Response> Callback)
        {
            Utils.CallEventAsync(new ApiEvent(HttpRequestType.PUT, request, (res) =>
            {
                var result = MapResponse<T>(res);
                Callback?.Invoke(result.Item1, result.Item2);
            }));
        }
        public static void DeleteAPI<T>(Request request, Action<T, Response> Callback)
        {
            Utils.CallEventAsync(new ApiEvent(HttpRequestType.DELETE, request, (res) =>
            {
                var result = MapResponse<T>(res);
                Callback?.Invoke(result.Item1, result.Item2);
            }));
        }

        private static (T, Response) MapResponse<T>(Response res)
        {
            try
            {
                T data = (T)res;
                return (data, res);
            }
            catch (Exception e)
            {
                return (default(T), res);
            }
        }
    }
}