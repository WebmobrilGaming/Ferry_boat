using GF;
using UnityEngine.UI;
namespace Ferry.Screens
{
    public class FerryHistoryScreen:BaseScreen<ScreenType>
    {
        public Button backBtn;
        protected override void OnEnable()
        {
            backBtn.AddListener(null,Close);
        }

        private void Close()
        {
            SwitchScreen(ScreenType.Home);
        }
        protected override void OnDisable()
        {
            backBtn.RemoveListener();
        }
    }
}
