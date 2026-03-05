using UnityEngine;
using UnityEngine.UI;

public class IntroUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameObject newUserPanel;
    [SerializeField] private GameObject mainMenuPanel;

    [Header("Buttons")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button newUserButton;

    private void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
        newUserButton.onClick.AddListener(OpenNewUserPanel);
    }

    private void StartGame()
    {
        introPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void OpenNewUserPanel()
    {
        introPanel.SetActive(false);
        newUserPanel.SetActive(true);
    }
}