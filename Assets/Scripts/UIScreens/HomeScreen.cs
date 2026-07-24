using System.Collections;
using DG.Tweening;
using Ferry.Loading;
using GF;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Ferry.Screens
{
    public class HomeScreen : BaseScreen<ScreenType>
    {
        public Button startSimulationBtn;
        public Button GameObjectivesBtn;
        public Button FerryHistoryBtn;
        public Button LeaderboardBtn;
        public Button SettingsBtn;
        public Button CloseBtn;

        public RectTransform startSimulationRect;
        public RectTransform GameObjectivesRect;
        public RectTransform FerryHistoryRect;
        public RectTransform LeaderboardRect;
        public RectTransform SettingsRect;
        public RectTransform CloseRect;
        public Transform gameObjectivePanel;
        public Button gameObjectiveCloseBtn;
        public TMP_Text playerNameTxt1;
        public GameObject gameLogo;
        public GameObject GroupBtn;
        public Button exitGameBtn;
        protected override void OnEnable()
        {
            Debug.LogWarning(UserDataManager.Instance.UserDetails.data.username);
            playerNameTxt1.text = $"Player : {UserDataManager.Instance.UserDetails.data.username}";
            gameObjectivePanel.gameObject.SetActive(false);
            startSimulationBtn.AddListener(null, StartGame);
            GameObjectivesBtn.AddListener(null, OpenGameObjective);
            FerryHistoryBtn.AddListener(null, OpenFerryHistory);
            LeaderboardBtn.AddListener(null, OpenLeaderboard);
            SettingsBtn.AddListener(null, OpenSettings);
            CloseBtn.AddListener(null, Logout);
            gameObjectiveCloseBtn.AddListener(null, CloseObjectivePanel);
            StartCoroutine(AnimateButtons());
            gameLogo.SetActive(true);
            exitGameBtn.onClick.AddListener(ExitGame);
        }

        private void CloseObjectivePanel()
        {
            gameObjectivePanel.gameObject.SetActive(false);
            gameLogo.SetActive(true);
            GroupBtn.SetActive(true);
        }

        private void Logout()
        {
            GamePopUp.Instance.PopControl("Are you sure you want to Logout", () =>
            {
                PlayerPrefs.DeleteAll();
                SwitchScreen(ScreenType.Login);
            }, null);
        }
        private void ExitGame()
        {
            GamePopUp.Instance.PopControl("Are you sure you want to Exit to Desktop", () =>
            {
                Application.Quit();
            }, null);
        }

        private void OpenSettings()
        {
            SwitchScreen(ScreenType.Settings);
        }

        private IEnumerator AnimateButtons()
        {
            yield return new WaitForSeconds(1);
            startSimulationRect.DOAnchorPosX(800, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
            GameObjectivesRect.DOAnchorPosX(800, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
            FerryHistoryRect.DOAnchorPosX(800, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
            LeaderboardRect.DOAnchorPosX(800, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
            SettingsRect.DOAnchorPosX(800, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
            CloseRect.DOAnchorPosX(800, 0.5f).SetEase(Ease.OutBack);
        }
        private void OpenLeaderboard()
        {
            SwitchScreen(ScreenType.Leaderboard);
        }

        private void OpenFerryHistory()
        {
            SwitchScreen(ScreenType.FerryHistory);
        }

        private void OpenGameObjective()
        {
            gameObjectivePanel.gameObject.SetActive(true);
            gameLogo.SetActive(false);
            GroupBtn.SetActive(false);
        }

        private void StartGame()
        {
            LoadingScreen.Instance.LoadSceneAsync(SceneEnum.Game, SceneEnum.Home);
        }
        protected override void OnDisable()
        {
            startSimulationBtn.RemoveListener();
            GameObjectivesBtn.RemoveListener();
            FerryHistoryBtn.RemoveListener();
            LeaderboardBtn.RemoveListener();
            gameLogo.SetActive(false);
            exitGameBtn.onClick.RemoveListener(ExitGame);
        }
    }
}
