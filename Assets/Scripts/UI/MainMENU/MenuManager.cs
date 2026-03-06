using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
        if (startGameButton)
            startGameButton.onClick.AddListener(OpenMainMenu);

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
        introPanel.transform.DOScale(0, 0.2f).OnComplete(() =>
        {
            introPanel.SetActive(false);

            mainMenu.SetActive(true);
            mainMenu.transform.localScale = Vector3.zero;
            mainMenu.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        });
    }

    void ShowMainMenu()
    {
        Debug.Log("Exit Clicked");

        mainMenu.SetActive(true);

        if (startSimulationPanel) startSimulationPanel.SetActive(false);
        if (gameObjectivePanel) gameObjectivePanel.SetActive(false);
        if (ferryHistoryPanel) ferryHistoryPanel.SetActive(false);
        if (leaderboardPanel) leaderboardPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
    }

    void OpenStartSimulation()
    {
        mainMenu.SetActive(false);

        startSimulationPanel.SetActive(true);
        startSimulationPanel.transform.localScale = Vector3.zero;
        startSimulationPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }

    void OpenGameObjective()
    {
        mainMenu.SetActive(false);

        gameObjectivePanel.SetActive(true);
        gameObjectivePanel.transform.localScale = Vector3.zero;
        gameObjectivePanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }

    void OpenFerryHistory()
    {
        mainMenu.SetActive(false);

        ferryHistoryPanel.SetActive(true);
        ferryHistoryPanel.transform.localScale = Vector3.zero;
        ferryHistoryPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }

    void OpenLeaderboard()
    {
        mainMenu.SetActive(false);

        leaderboardPanel.SetActive(true);
        leaderboardPanel.transform.localScale = Vector3.zero;
        leaderboardPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }

    void OpenSettings()
    {
        mainMenu.SetActive(false);

        settingsPanel.SetActive(true);
        settingsPanel.transform.localScale = Vector3.zero;
        settingsPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }
}