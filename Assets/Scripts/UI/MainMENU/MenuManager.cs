using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GF;
using Ferry_boat.Assets.Scripts.Web;

public class MenuManager : MonoBehaviour
{
    [Header("Intro panel")]
    public GameObject introPanel;
    [Header("MainMenu")]
    public GameObject mainMenu;

    [Header("startSimulationPanel")]
    public GameObject startSimulationPanel;
    public Button startSimulationButton;
    public Button exitStartSimulation;
    public Button startGameButton;


    [Header("gameObjectivePanel")]
    public GameObject gameObjectivePanel;
    public Button gameObjectiveButton;
    public Button exitGameObjective;

    [Header("ferryHistoryPanel")]

    public GameObject ferryHistoryPanel;
    public Button ferryHistoryButton;
    public Button exitFerryHistory;

    [Header("leaderboardPanel")]

    public GameObject leaderboardPanel;
    public Button leaderboardButton;
    public Button exitLeaderboard;

    [Header("settingsPanel")]

    public GameObject settingsPanel;
    public Button settingsButton;
    public Button exitSettings;

    void Start()
    {
        ApplicationManager.Instance.Initialize();
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
        // APIManager.PostAPI<ResponseBody>(new RequestData(Netconfig.RequestType.SignIn, "hello"), (res) =>
        // {
        //     Debug.Log(res.status);
        //     Debug.Log(res.message);
        // });
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
        //mainMenu.SetActive(false);
        SceneManager.Instance.LoadScene(SceneType.Game);

        //startSimulationPanel.SetActive(true);
        // startSimulationPanel.transform.localScale = Vector3.zero;
        //startSimulationPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack).OnComplete(() => 
        //{
        //    SceneManager.Instance.LoadAdditive(SceneType.Game);
        //});
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