using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void Exit()
    {
        PopupController.Instance.ShowYesNo(
            "Do you want to exit the game?",
            onYes: () =>
            {
                Application.Quit();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            },
            onNo: null
        );
    }
}