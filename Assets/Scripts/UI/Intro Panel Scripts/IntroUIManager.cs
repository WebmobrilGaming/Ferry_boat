using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class IntroUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameObject userPanel;
    [SerializeField] private GameObject mainMenuPanel;

    [Header("Buttons")]
    [SerializeField] private Button previousUserButton;
    [SerializeField] private Button newUserButton;
    [Header("Player Script")]
    [SerializeField] private PlayerNameInput playerNameInput;
    

    private void Start()
    {
        newUserButton.onClick.AddListener(OpenNewUser);
        previousUserButton.onClick.AddListener(OpenPreviousUser);
    }

    private void StartGame()
    {
        introPanel.transform.DOScale(0, 0.2f).OnComplete(() =>
        {
            introPanel.SetActive(false);

            mainMenuPanel.SetActive(true);
            mainMenuPanel.transform.localScale = Vector3.zero;
            mainMenuPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        });
    }

    private void OpenNewUser()
    {
        playerNameInput.OpenNewPlayer();

        OpenUserPanel();
    }

    private void OpenPreviousUser()
    {
        playerNameInput.OpenPreviousPlayer();
        OpenUserPanel();
    }

    private void OpenUserPanel()
    {
        introPanel.transform.DOScale(0, 0.2f).OnComplete(() =>
        {
            introPanel.SetActive(false);
            userPanel.SetActive(true);
            userPanel.transform.localScale = Vector3.zero;
            userPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        });
    }
}