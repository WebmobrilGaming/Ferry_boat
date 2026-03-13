using System;

namespace GF
{
    public class MonoService : IService
    {
        private bool isUpdateRequired = false;
        public bool IsUpdateRequired => isUpdateRequired;
        public void Initialize()
        {
            Logger.Log(LogType.Log, "MonoService created");
        }

        public void RegisterListener()
        {
            EventManager.Instance.AddListener<CoroutineEvent>(StartCoroutine);
        }

        private void StartCoroutine(CoroutineEvent e)
        {
            ApplicationManager.Instance.StartCoroutine(e.Enumerator);
        }

        public void RemoveListener()
        {
            EventManager.Instance.RemoveListener<CoroutineEvent>(StartCoroutine);
        }
        public void Update()
        {
            
        }
    }
}
