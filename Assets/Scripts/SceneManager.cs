using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    static SceneManager instance;
    public static SceneManager Instance { get { return instance; } }

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

    /// <summary>Load a scene by SceneType enum.</summary>
    public void LoadScene(SceneType sceneType)
    {
        string sceneName = SceneTypeToName(sceneType);

        if (IsSceneLoaded(sceneName))
        {
            Debug.LogWarning($"[SceneManager] Scene '{sceneName}' is already loaded.");
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        Debug.Log($"[SceneManager] Loading scene: {sceneName}");
    }

    /// <summary>Reload the currently active scene.</summary>
    public void ReloadActiveScene()
    {
        int activeIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(activeIndex);
    }

    // ── Helpers ───────────────────────────────────────────────────

    private bool IsSceneLoaded(string sceneName)
    {
        Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
        return scene.IsValid() && scene.isLoaded;
    }

    private string SceneTypeToName(SceneType sceneType)
    {
        return sceneType switch
        {
            SceneType.Home => "Home",
            SceneType.Game => "Game",
            _ => throw new System.ArgumentOutOfRangeException(nameof(sceneType), sceneType, null)
        };
    }
}

public enum SceneType { Home, Game }