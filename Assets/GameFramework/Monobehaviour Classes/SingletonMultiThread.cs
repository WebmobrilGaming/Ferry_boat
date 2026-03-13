using JetBrains.Annotations;
using UnityEngine;
namespace GF
{
    public class SingletonMultiThread<T> : MonoBehaviour where T : MonoBehaviour
    {
        #region  Fields
        [CanBeNull]
        private static T _instance;

        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        private static readonly object Lock = new object();
        /// <summary>
        /// Specify whether singleton persistant across all scenes.
        /// </summary>
        public bool DontDestroyWhenLoad;
        #endregion

        #region  Properties
        public static bool Quitting { get; private set; }
        [NotNull]
        public static T Instance
        {
            get
            {
                if (Quitting)
                {
                    Debug.LogWarning($"({nameof(SingletonMultiThread<T>)}<{typeof(T)}>) Instance will not be returned because the application is quitting.");
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return null;
                }
                lock (Lock)
                {
                    if (_instance != null)
                        return _instance;
                    var instances = FindObjectsOfType<T>();
                    var count = instances.Length;
                    if (count > 0)
                    {
                        if (count == 1)
                            return _instance = instances[0];
                        Debug.LogWarning($"({nameof(SingletonMultiThread<T>)}<{typeof(T)}>) There should never be more than one {nameof(SingletonMultiThread<T>)} of type {typeof(T)} in the scene, but {count} were found. The first instance found will be used, and all others will be destroyed.");
                        for (var i = 1; i < instances.Length; i++)
                            Destroy(instances[i]);
                        return _instance = instances[0];
                    }

                    Debug.Log($"[{nameof(SingletonMultiThread<T>)}<{typeof(T)}>] An instance is needed in the scene and no existing instances were found, so a new instance will be created.");
                    return _instance = new GameObject($"(Singleton){typeof(T)}")
                               .AddComponent<T>();
                }
            }
        }
        #endregion

        #region  Methods
        protected virtual void Awake()
        {
            if (DontDestroyWhenLoad)
                DontDestroyOnLoad(gameObject);

        }
        protected void ApplyHighlighter(Color bg, Color text)
        {
            gameObject.AddComponent<HierarchyHighlighter>();
            HierarchyHighlighter hr = GetComponent<HierarchyHighlighter>();
            hr.Background_Color = bg;
            hr.Text_Color = text;
            hr.TextStyle = FontStyle.BoldAndItalic;
        }
        protected virtual void OnApplicationQuit()
        {
            Quitting = true;
        }
        #endregion

    }
}