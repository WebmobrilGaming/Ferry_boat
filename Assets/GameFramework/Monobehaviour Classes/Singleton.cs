using UnityEngine;

namespace BigJoker
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject); // keep it alive across scenes
            }
            else if (instance != this)
            {
                Destroy(gameObject); // prevent duplicates
            }
        }
    }
}

