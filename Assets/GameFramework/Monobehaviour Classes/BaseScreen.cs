using System;
using UnityEngine;
namespace GF
{
    public abstract class BaseScreen<E> : MonoBehaviour where E: Enum
    {
        public E Screen;
        protected virtual void OnEnable()
        {
            //EventManager.Instance.RemoveListener<UnLoadingCompletedEvent>(DoInitialLoading);
        }
        public void OpenScreen()
        {
            gameObject.SetActive(true);
        }
        public void CloseScreen()
        {
            gameObject.SetActive(false);
        }
        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackKeyPressed();
            }
        }
        protected void SwitchScreen(E screenId)
        {
            SceneHandler<E>.Instance.ChangeScreen(screenId);
        }
        protected abstract void OnBackKeyPressed();
        protected virtual void OnDisable()
        {
            //EventManager.Instance.RemoveListener<UnLoadingCompletedEvent>(DoInitialLoading);
        }
    }
}