using GF;
namespace Ferry.Screens
{
    public class HomeSceneHandler : SceneHandler<ScreenType>
    {
        protected override void Awake()
        {
            var inst=ApplicationManager.Instance;
            base.Awake();
        }
        protected override void RegisterServices()
        {
            
        }
    }
    public enum ScreenType
    {
        Login,
        Home,
        Leaderboard
    }
}
