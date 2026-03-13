using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace GF
{
    /// <summary>
    /// Handles all Utility functionality in the game.
    /// </summary>
    public static class Utils
    {
        public static void CallEventAsync(GameEvent gameEvent)
        {
            EventManager.Instance.TriggerEvent(gameEvent);
        }
        public static void CallEventSync(GameEvent gameEvent)
        {
            EventManager.Instance.QueueEvent(gameEvent);
        }
        public static bool ColorCompare(Color a, Color b, int diff)
        {
            return (int)(Mathf.Abs(a.r - b.r) * 100) < diff &&
                (int)(Mathf.Abs(a.g - b.g) * 100) < diff &&
                (int)(Mathf.Abs(a.b - b.b) * 100) < diff;
        }
        public static IEnumerator DoLerp<T>(float delay, T obj) where T:Component
        {
            float lerp = 0;
            float v = 0;
            while (lerp <= 1)
            {
                 v= Mathf.Lerp(0, 1, lerp);
                lerp += (1 / delay) * Time.deltaTime;
                yield return null;
            }
        }

        [Obsolete]
        public static IEnumerator DownloadImage(string imageUrl, bool isLocal, Action<Texture> image)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.Log(request.error);
                    image?.Invoke(null);
                }
                else
                {
                    if (!isLocal)
                    {
                        string[] fileName = imageUrl.Split('/');
                        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, fileName[fileName.Length - 1]), request.downloadHandler.data);
                    }

                    image?.Invoke(DownloadHandlerTexture.GetContent(request));
                }
            }
        }
        public static Color GetColorByHashString(string hash)
        {
            Color color;
            if (ColorUtility.TryParseHtmlString(hash, out color))
            {

            }
            return color;
        }
        /// <summary>
        /// used to add componet if not,if true then to add more than one component of same type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="multipleComponentAdable"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(GameObject obj,bool multipleComponentAdable=false) where T : Component
        {
            if(multipleComponentAdable)
            {
                obj.AddComponent<T>();
            }
            T e = obj.GetComponent<T>();
            if (e == null)
            {
                obj.AddComponent<T>();
            }
            return obj.GetComponent<T>();
        }
        /// <summary>
        /// Finding the DotProduct of two vectors.
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        /// it returns the Dot Product in float.
        public static float DotProduct(Vector3 vector1,Vector3 vector2)
        {
            return (vector1.x * vector2.x + vector1.y * vector2.y);
        }
        /// <summary>
        /// It returns the angle between two vectors.
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        /// return angle value is in Radian.
        public static float GetAngleBetweenTwoVectors(Vector3 vector1,Vector3 vector2)
        {
            float angle = 0;
            angle = Mathf.Acos(DotProduct(vector1, vector2) / Distance(Vector3.zero, vector1) * Distance(Vector3.zero, vector2));
            return angle;
        }
        public static Vector3 GetNormalFormOfVector(Vector3 vector)
        {
            float length = Distance(Vector3.zero, vector);
            vector.x /= length;
            vector.y /= length;
            vector.z /= length;
            return vector;
        }
        /// <summary>
        /// return distance berween two vectors.
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <returns></returns>
        public static float Distance(Vector3 vector1,Vector3 vector2)
        {
            float squareDist=Sqaure(vector1.x - vector2.x) + Sqaure(vector1.y - vector2.y) + Sqaure(vector1.z - vector2.z);
            float squareRoot = Mathf.Sqrt(squareDist);
            return squareRoot;
        }
        public static float Sqaure(float val)
        {
            return val * val;
        }
    }
}