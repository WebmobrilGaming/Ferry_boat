using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private Button startGameButton;

    [SerializeField] private GameObject newUserPanel;
    [SerializeField] private GameObject mainMenuPanel;

    private void Start()
    {
        startGameButton.onClick.AddListener(OnStartGameClicked);
    }

    private void OnStartGameClicked()
    {
        string playerName = nameInputField.text;

        if (string.IsNullOrWhiteSpace(playerName))
        {
            Debug.Log("Please enter your name!");
            return;
        }

        PlayerPrefs.SetString("PlayerName", playerName);

        newUserPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}