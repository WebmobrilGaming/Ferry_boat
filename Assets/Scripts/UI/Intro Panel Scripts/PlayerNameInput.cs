using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerNameInput : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField firstNameInputField;
    [SerializeField] private TMP_InputField lastNameInputField;
    [SerializeField] private Button startGameButton;

    [Header("UI")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Button exitButton;

    [SerializeField] private GameObject userPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject introPanel;

    private void Start()
    {
        startGameButton.onClick.AddListener(OnStartGameClicked);
        exitButton.onClick.AddListener(OnExitClicked);
    }

    public void OpenNewPlayer()
    {
        titleText.text = "Create a new account";
        OpenUserPanel();
    }

    public void OpenPreviousPlayer()
    {
        titleText.text = "Enter user detail";
        OpenUserPanel();
    }

    private void OpenUserPanel()
    {
        // Scale down introPanel first, then open userPanel
        introPanel.transform.DOScale(0, 0.2f).OnComplete(() =>
        {
            introPanel.SetActive(false);

            userPanel.SetActive(true);
            userPanel.transform.localScale = Vector3.zero;
            userPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        });
    }

    private void OnExitClicked()
    {
        firstNameInputField.text = "";
        lastNameInputField.text = "";

        // Scale down userPanel first, then open introPanel
        userPanel.transform.DOScale(0, 0.2f).OnComplete(() =>
        {
            userPanel.SetActive(false);

            introPanel.SetActive(true);
            introPanel.transform.localScale = Vector3.zero;
            introPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        });
    }

    private void OnStartGameClicked()
    {
        string firstName = firstNameInputField.text.Trim();
        string lastName = lastNameInputField.text.Trim();

        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            Debug.Log("Please enter both first and last name!");
            return;
        }

        string fullName = firstName + " " + lastName;
        PlayerPrefs.SetString("PlayerName", fullName);

        // Scale down userPanel first, then open mainMenuPanel
        userPanel.transform.DOScale(0, 0.2f).OnComplete(() =>
        {
            userPanel.SetActive(false);

            mainMenuPanel.SetActive(true);
            mainMenuPanel.transform.localScale = Vector3.zero;
            mainMenuPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        });
    }
}