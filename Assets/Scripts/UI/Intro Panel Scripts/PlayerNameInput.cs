using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GF;
using Ferry_boat.Assets.Scripts.Web;
using static GF.UnityWebService;
using Unity.Android.Gradle.Manifest;

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
        //username, firstname, lastname  (validation)
        var request = new CreatePlayer("new", userNameInputFeild.text, firstNameInputField.text, lastNameInputField.text);
        APIManager.PostAPI<UserDetails>(new RequestData(Netconfig.RequestType.CreatePlayer, request), (data, res) =>
        {
            if (res.status == HttpCodes.OK)
            {
                this.userData = data;
                Debug.Log("UserDetails: " + userData);
                isNewUser = true;
            }
        });
    }
    private void LoginPreviousUser()
    {
        //username
        var request = new CreatePlayer("previous", previousUsernameInputField.text);
        APIManager.PostAPI<UserDetails>(new RequestData(Netconfig.RequestType.LoginPlayer, request), (data, res) =>
        {
            if (res.status == HttpCodes.OK)
            {
                this.userData = data;
                Debug.Log("UserDetails: " + userData);
                isNewUser = false;
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
        string fullName = "";

        if (isNewUser)
        {
            string firstName = firstNameInputField.text.Trim();
            string lastName = lastNameInputField.text.Trim();
            string extra = userNameInputFeild.text.Trim();

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(extra))
            {
                Debug.Log("Please fill all fields!");
                return;
            }

            fullName = firstName + " " + lastName;
            PlayerPrefs.SetString("ExtraField", extra);

            LoginPreviousUser();
        }
        else
        {
            string name = previousUsernameInputField.text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                Debug.Log("Please enter your name!");
                return;
            }

            fullName = name;
        }

        PlayerPrefs.SetString("PlayerName", fullName);

        userPanel.transform.DOScale(0, 0.2f).OnComplete(() =>
        {
            userPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
            mainMenuPanel.transform.localScale = Vector3.zero;
            mainMenuPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        });
    }
}