using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class IntroUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject introPanel;
    [SerializeField] private GameObject newUserPanel;
    [SerializeField] private GameObject mainMenuPanel;

    [Header("Buttons")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button newUserButton;

    private void Start()
    {
        startGameButton.onClick.AddListener(StartGame);
        newUserButton.onClick.AddListener(OpenNewUserPanel);
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

    private void OpenNewUserPanel()
    {
        introPanel.transform.DOScale(0, 0.2f).OnComplete(() =>
        {
            introPanel.SetActive(false);

            newUserPanel.SetActive(true);
            newUserPanel.transform.localScale = Vector3.zero;
            newUserPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        });
    }
}