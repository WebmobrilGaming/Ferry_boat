using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject introPanel;
    public GameObject mainMenu;

    public GameObject startSimulationPanel;
    public GameObject gameObjectivePanel;
    public GameObject ferryHistoryPanel;
    public GameObject leaderboardPanel;
    public GameObject settingsPanel;

    public Button startGameButton;

    public Button startSimulationButton;
    public Button gameObjectiveButton;
    public Button ferryHistoryButton;
    public Button leaderboardButton;
    public Button settingsButton;

    public Button exitStartSimulation;
    public Button exitGameObjective;
    public Button exitFerryHistory;
    public Button exitLeaderboard;
    public Button exitSettings;

    void Start()
    {
        // Intro
        if (startGameButton)
            startGameButton.onClick.AddListener(OpenMainMenu);

        // Main Menu
        if (startSimulationButton)
            startSimulationButton.onClick.AddListener(OpenStartSimulation);

        if (gameObjectiveButton)
            gameObjectiveButton.onClick.AddListener(OpenGameObjective);

        if (ferryHistoryButton)
            ferryHistoryButton.onClick.AddListener(OpenFerryHistory);

        if (leaderboardButton)
            leaderboardButton.onClick.AddListener(OpenLeaderboard);

        if (settingsButton)
            settingsButton.onClick.AddListener(OpenSettings);

        // Exit Buttons
        if (exitStartSimulation)
            exitStartSimulation.onClick.AddListener(ShowMainMenu);

        if (exitGameObjective)
            exitGameObjective.onClick.AddListener(ShowMainMenu);

        if (exitFerryHistory)
            exitFerryHistory.onClick.AddListener(ShowMainMenu);

        if (exitLeaderboard)
            exitLeaderboard.onClick.AddListener(ShowMainMenu);

        if (exitSettings)
            exitSettings.onClick.AddListener(ShowMainMenu);
    }

    void OpenMainMenu()
    {
        introPanel.SetActive(false);
        mainMenu.SetActive(true);
    }

    void ShowMainMenu()
    {
        Debug.Log("Exit Clicked");

        mainMenu.SetActive(true);

        if (startSimulationPanel != null)
            startSimulationPanel.SetActive(false);

        if (gameObjectivePanel != null)
            gameObjectivePanel.SetActive(false);

        if (ferryHistoryPanel != null)
            ferryHistoryPanel.SetActive(false);

        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    // void ShowMainMenu()
    // {
    //     mainMenu.SetActive(true);

    //     startSimulationPanel.SetActive(false);
    //     gameObjectivePanel.SetActive(false);
    //     ferryHistoryPanel.SetActive(false);
    //     leaderboardPanel.SetActive(false);
    //     settingsPanel.SetActive(false);
    // }

    void OpenStartSimulation()
    {
        mainMenu.SetActive(false);
        startSimulationPanel.SetActive(true);
    }

    void OpenGameObjective()
    {
        mainMenu.SetActive(false);
        gameObjectivePanel.SetActive(true);
    }

    void OpenFerryHistory()
    {
        mainMenu.SetActive(false);
        ferryHistoryPanel.SetActive(true);
    }

    void OpenLeaderboard()
    {
        mainMenu.SetActive(false);
        leaderboardPanel.SetActive(true);
    }

    void OpenSettings()
    {
        mainMenu.SetActive(false);
        settingsPanel.SetActive(true);
    }
}