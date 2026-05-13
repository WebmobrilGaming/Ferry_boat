using System.Collections;
using DG.Tweening;
using GF;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ferry.Loading
{
    public class LoadingScreen : Singleton<LoadingScreen>
    {
        public CanvasGroup splashScreen;
        public GameObject loadingObj;
        public GameObject cameraObj;
        private bool isLoading;
        private Coroutine loadingCoroutine;
        private Coroutine sceneLoadCoroutine;
        private float currentTime = 0;
        private float maxTime = 17f;
        protected override void Awake()
        {
            canDestroy = true;
            base.Awake();
            var inst = ApplicationManager.Instance;
        }
        void Start()
        {
            StartCoroutine(LoadSplash());
        }
        public void StopLoading()
        {
            isLoading = false;
        }
        private IEnumerator LoadSplash()
        {
            splashScreen.DOFade(1, 1f);
            yield return new WaitForSeconds(2f);
            splashScreen.DOFade(0, 1f);
            yield return new WaitForSeconds(2f);
            ShowLoadingScreen();
            splashScreen.gameObject.SetActive(false);
            LoadSceneAsync(SceneEnum.Home);
        }
        public void ShowLoadingScreen()
        {
            isLoading = true;
            cameraObj.SetActive(true);
            if (loadingCoroutine != null)
            {
                StopCoroutine(loadingCoroutine);
            }
            loadingCoroutine = StartCoroutine(StartLoading());
        }
        private IEnumerator StartLoading()
        {
            loadingObj.SetActive(true);
            yield return new WaitUntil(() => !isLoading);
            yield return new WaitForSeconds(1.0f);
            loadingObj.SetActive(false);
            cameraObj.SetActive(false);
        }
        public void LoadSceneAsync(SceneEnum targetScene, SceneEnum currentScene = SceneEnum.Persistence)
        {
            if (sceneLoadCoroutine != null)
            {
                StopCoroutine(sceneLoadCoroutine);
            }
            sceneLoadCoroutine = StartCoroutine(LoadSceneDelay((int)targetScene, (int)currentScene));
        }
        private IEnumerator LoadSceneDelay(int sceneIndex, int previousSceneIndex = 0)
        {
            ShowLoadingScreen();
            isLoading = false;

            // check if target scene exists
            if (!SceneIndexExists(sceneIndex))
            {
                Debug.LogError($"Scene with index {sceneIndex} does not exist in Build Settings!");
                StopLoading();
                yield break; // stop coroutine safely
            }

            if (previousSceneIndex != 0)
            {
                //unload previous scene first.
                Scene previousScene = SceneManager.GetSceneByBuildIndex(previousSceneIndex);
                if (previousScene.isLoaded)
                {
                    yield return SceneManager.UnloadSceneAsync(previousSceneIndex);
                }
                
            }
            yield return SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            Scene currentScene = SceneManager.GetSceneByBuildIndex(sceneIndex);
            SceneManager.SetActiveScene(currentScene);
        }
        private bool SceneIndexExists(int sceneIndex)
        {
            return sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings;
        }
        void Update()
        {
            if (isLoading)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= maxTime)
                {
                    currentTime = 0;
                    isLoading = false;
                }
            }
        }
    }
    public enum SceneEnum
    {
        Persistence,
        Home,
        Game
    }
}
