using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GF;
using Ferry_boat.Assets.Scripts.Web;

public class PlayerNameInput : MonoBehaviour
{
    [Header("Frames")]
    [SerializeField] private GameObject previousUserFrame;  // 1 field frame
    [SerializeField] private GameObject newUserFrame;       // 3 fields frame

    [Header("Input Fields - Previous User")]
    [SerializeField] private TMP_InputField prevFullNameInputField;

    [Header("Input Fields - New User")]
    [SerializeField] private TMP_InputField firstNameInputField;
    [SerializeField] private TMP_InputField lastNameInputField;
    [SerializeField] private TMP_InputField userNameInputFeild; // your 3rd field

    [Header("Buttons")]
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button exitButton;

    [Header("Panels")]
    [SerializeField] private GameObject userPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject introPanel;

    private bool isNewUser = false;

    private void Start()
    {
        startGameButton.onClick.AddListener(OnStartGameClicked);
        exitButton.onClick.AddListener(OnExitClicked);
    }

    public void OpenNewPlayer()
    {
        OpenUserPanel();
    }

    private void CreateNewUser()
    {
        var request = new CreatePlayer("new", userNameInputFeild.text, firstNameInputField.text, lastNameInputField.text);
        APIManager.PostAPI<UserDetails>(new RequestData(Netconfig.RequestType.NewUser, request), (res) =>
        {
            Debug.Log("UserDetails: " + res);
            if (res.success)
            {
                isNewUser = true;

            }
        });
    }

    public void OpenPreviousPlayer()
    {
        isNewUser = false;
        previousUserFrame.SetActive(true);
        newUserFrame.SetActive(false);
        OpenUserPanel();
    }

    private void OpenUserPanel()
    {
        previousUserFrame.SetActive(false);
        newUserFrame.SetActive(true);
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
        // Clear all fields
        prevFullNameInputField.text = "";
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
            // Save extra field however you need:
            PlayerPrefs.SetString("ExtraField", extra);
        }
        else
        {
            string name = prevFullNameInputField.text.Trim();

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