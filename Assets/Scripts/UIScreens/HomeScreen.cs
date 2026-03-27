using System.Collections;
using DG.Tweening;
using GF;
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
        protected override void OnEnable()
        {
            gameObjectivePanel.gameObject.SetActive(false);
            startSimulationBtn.AddListener(null, StartGame);
            GameObjectivesBtn.AddListener(null, OpenGameObjective);
            FerryHistoryBtn.AddListener(null, OpenFerryHistory);
            LeaderboardBtn.AddListener(null, OpenLeaderboard);
            SettingsBtn.AddListener(null, OpenSettings);
            CloseBtn.AddListener(null, Logout);
            gameObjectiveCloseBtn.AddListener(null,CloseObjectivePanel);
            StartCoroutine(AnimateButtons());
        }

        private void CloseObjectivePanel()
        {
            gameObjectivePanel.gameObject.SetActive(false);
        }

        private void Logout()
        {
            PopupController.Instance.ShowYesNo(
           "Do you want to exit the game?",
           onYes: () =>
           {
               SwitchScreen(ScreenType.Login);
           },
           onNo: null
       );
        }

        private void OpenSettings()
        {
            SwitchScreen(ScreenType.Settings);
        }

        private IEnumerator AnimateButtons()
        {
            startSimulationRect.DOAnchorPosX(900, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
            GameObjectivesRect.DOAnchorPosX(900, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
            FerryHistoryRect.DOAnchorPosX(900, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
            LeaderboardRect.DOAnchorPosX(900, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
            SettingsRect.DOAnchorPosX(900, 0.5f).SetEase(Ease.OutBack);
            yield return new WaitForSeconds(0.1f);
            CloseRect.DOAnchorPosX(900, 0.5f).SetEase(Ease.OutBack);
        }
        private void OpenLeaderboard()
        {
            SwitchScreen(ScreenType.Leaderboard);
        }

        private void OpenFerryHistory()
        {

        }

        private void OpenGameObjective()
        {
            gameObjectivePanel.gameObject.SetActive(true);
        }

        private void StartGame()
        {
            SceneManager.LoadScene(1);
        }
        protected override void OnDisable()
        {
            startSimulationBtn.RemoveListener();
            GameObjectivesBtn.RemoveListener();
            FerryHistoryBtn.RemoveListener();
            LeaderboardBtn.RemoveListener();
        }
    }
}
