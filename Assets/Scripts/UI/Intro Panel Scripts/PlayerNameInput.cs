using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GF;
using Ferry_boat.Assets.Scripts.Web;
using static GF.UnityWebService;

public class PlayerNameInput : MonoBehaviour
{
    [Header("Frames")]
    [SerializeField] private GameObject previousUserFrame;
    [SerializeField] private GameObject newUserFrame;

    [Header("Input Fields - Previous User")]
    [SerializeField] private TMP_InputField previousUsernameInputField;

    [Header("Input Fields - New User")]
    [SerializeField] private TMP_InputField firstNameInputField;
    [SerializeField] private TMP_InputField lastNameInputField;
    [SerializeField] private TMP_InputField userNameInputFeild;

    [Header("Buttons")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button previousGameButton;
    [SerializeField] private Button exitButton;

    [Header("Panels")]
    [SerializeField] private GameObject userPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject introPanel;

    private UserDetails userData = null;
    private bool isNewUser = false;

    private void Start()
    {
        startGameButton.onClick.AddListener(CreateNewUser);
        previousGameButton.onClick.AddListener(LoginPreviousUser);
        exitButton.onClick.AddListener(OnExitClicked);
    }

    public void OpenNewPlayer()
    {
        isNewUser = true;
        OpenUserPanel(isNewUser: true);
    }

    public void OpenPreviousPlayer()
    {
        isNewUser = false;
        OpenUserPanel(isNewUser: false);
    }

    private void OpenUserPanel(bool isNewUser)
    {
        previousUserFrame.SetActive(!isNewUser);
        newUserFrame.SetActive(isNewUser);

        introPanel.transform.DOScale(0, 0.2f).OnComplete(() =>
        {
            introPanel.SetActive(false);
            userPanel.SetActive(true);
            userPanel.transform.localScale = Vector3.zero;
            userPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        });
    }



    private void CreateNewUser()
    {
        string firstName = firstNameInputField.text.Trim();
        string lastName = lastNameInputField.text.Trim();
        string userName = userNameInputFeild.text.Trim();

        // Validate all fields before hitting the API
        if (string.IsNullOrEmpty(firstName) ||
            string.IsNullOrEmpty(lastName) ||
            string.IsNullOrEmpty(userName))
        {
            PopupController.Instance.ShowFillAllFields();
            return;
        }

        SetButtonsInteractable(false);

        var request = new CreatePlayer("new", userName, firstName, lastName);
        APIManager.PostAPI<UserDetails>(
            new RequestData(Netconfig.RequestType.CreatePlayer, request),
            (data, response) =>
            {
                SetButtonsInteractable(true);

                if (response.status)
                {
                    userData = data;
                    isNewUser = true;

                    OnStartGameClicked();
                }
                else
                {
                    PopupController.Instance.ShowUsernameAlreadyExists();
                }
            });
    }

    private void LoginPreviousUser()
    {OnStartGameClicked();
        string username = previousUsernameInputField.text.Trim();

        if (string.IsNullOrEmpty(username))
        {
            PopupController.Instance.ShowFillAllFields();
            return;
        }

        SetButtonsInteractable(false);

        var request = new CreatePlayer("previous", username);
        APIManager.PostAPI<UserDetails>(
            new RequestData(Netconfig.RequestType.LoginPlayer, request),
            (data, response) =>
            {
                SetButtonsInteractable(true);

                if (response.status)
                {
                    userData = data;
                    isNewUser = false;
                    Debug.Log("Login successful: " + userData);
                    OnStartGameClicked();   // ← only proceed AFTER a successful login
                }
                else
                {
                    PopupController.Instance.ShowInvalidUsername();
                }
            });
    }

    private void OnExitClicked()
    {
        previousUsernameInputField.text = "";
        firstNameInputField.text = "";
        lastNameInputField.text = "";
        userNameInputFeild.text = "";

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
        userPanel.transform.DOScale(0, 0.2f).OnComplete(() =>
        {
            userPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
            mainMenuPanel.transform.localScale = Vector3.zero;
            mainMenuPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        });
    }

    private void SetButtonsInteractable(bool interactable)
    {
        startGameButton.interactable = interactable;
        previousGameButton.interactable = interactable;
    }
}