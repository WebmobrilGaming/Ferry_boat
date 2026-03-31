using System;
using Ferry.Loading;
using GF;
using TMPro;
namespace Ferry.Screens
{
    public class HomeSceneHandler : SceneHandler<ScreenType>
    {
        protected override void RegisterServices()
        {
            LoadingScreen.Instance.StopLoading();
        }
    }
    public enum ScreenType
    {
        Login,
        Home,
        Leaderboard,
        Settings,
        FerryHistory
    }
}
