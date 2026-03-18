using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField firstNameInputField;
    [SerializeField] private TMP_InputField lastNameInputField;
    [SerializeField] private Button startGameButton;
    [Header("UI")]
    [SerializeField] private TMP_Text titleText;

    [SerializeField] private GameObject userPanel;
    [SerializeField] private GameObject mainMenuPanel;
   

    private void Start()
    {
        startGameButton.onClick.AddListener(OnStartGameClicked);
       
    }

    public void OpenNewPlayer()
    {
        titleText.text = "Create a new account";
        userPanel.SetActive(true);
    }

    public void OpenPreviousPlayer()
    {
        titleText.text = "Enter user detail";
        userPanel.SetActive(true);
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

        userPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    
}