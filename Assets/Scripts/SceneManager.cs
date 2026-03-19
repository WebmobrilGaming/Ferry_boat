using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    static SceneManager instance;
    public static SceneManager Instance { get { return instance; } }

    private Scene? _additiveScene;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // ── Public API ────────────────────────────────────────────────

    /// <summary>Load a scene additively by SceneType enum.</summary>
    public void LoadAdditive(SceneType sceneType)
    {
        string sceneName = SceneTypeToName(sceneType);

        if (IsSceneLoaded(sceneName))
        {
            Debug.LogWarning($"[SceneManager] Scene '{sceneName}' is already loaded.");
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        Debug.Log($"[SceneManager] Loading additive scene: {sceneName}");
    }

    /// <summary>Unload a previously loaded additive scene by SceneType enum.</summary>
    public void UnloadAdditive(SceneType sceneType)
    {
        string sceneName = SceneTypeToName(sceneType);

        if (!IsSceneLoaded(sceneName))
        {
            Debug.LogWarning($"[SceneManager] Scene '{sceneName}' is not loaded.");
            return;
        }

        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        Debug.Log($"[SceneManager] Unloading additive scene: {sceneName}");
    }

    /// <summary>Reload the active base scene, keeping DontDestroyOnLoad intact.</summary>
    public void ReloadActiveScene()
    {
        int activeIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(activeIndex, LoadSceneMode.Single);
    }

    // ── Helpers ───────────────────────────────────────────────────

    private bool IsSceneLoaded(string sceneName)
    {
        Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
        return scene.IsValid() && scene.isLoaded;
    }

    private string SceneTypeToName(SceneType sceneType)
    {
        // Scene names in Build Settings must match these exactly
        return sceneType switch
        {
            SceneType.Home => "Home",
            SceneType.Game => "Game",
            _ => throw new System.ArgumentOutOfRangeException(nameof(sceneType), sceneType, null)
        };
    }
}

public enum SceneType { Home, Game }