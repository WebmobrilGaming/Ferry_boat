using System;
using GF;
using TMPro;
namespace Ferry.Screens
{
    public class HomeSceneHandler : SceneHandler<ScreenType>
    {
        protected override void Awake()
        {
            var inst = ApplicationManager.Instance;
            base.Awake();
        }
        protected override void RegisterServices()
        {
            UserDataManager.OnLogin += SetPlayerName;
        }

        private void SetPlayerName(string obj)
        {
            var playerNameTxt = GUI.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
            playerNameTxt.text = obj;
        }
        void OnDisable()
        {
            UserDataManager.OnLogin -= SetPlayerName;
        }
    }
    public enum ScreenType
    {
        Login,
        Home,
        Leaderboard,
        Settings
    }
}
