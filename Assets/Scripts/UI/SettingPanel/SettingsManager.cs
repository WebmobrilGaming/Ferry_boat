using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button musicButton;
    [SerializeField] private Button sfxButton;
    [SerializeField] private Button vibrationButton;
    [SerializeField] private Button difficultyButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button helpButton;
    [SerializeField] private Button difficultyExitButton;

    [Header("Settings Content")]
    [SerializeField] private GameObject settingsContent;

    [Header("Panels")]
    [SerializeField] private GameObject difficultyPanel;
    [SerializeField] private GameObject helpPanel;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private bool musicOn = true;
    private bool sfxOn = true;
    private bool vibrationOn = true;
    private bool tutorialOn = true;

    private void Start()
    {
        musicButton.onClick.AddListener(ToggleMusic);
        sfxButton.onClick.AddListener(ToggleSFX);
        vibrationButton.onClick.AddListener(ToggleVibration);
        difficultyButton.onClick.AddListener(OpenDifficultyPanel);
        tutorialButton.onClick.AddListener(ToggleTutorial);
        helpButton.onClick.AddListener(OpenHelpPanel);
        difficultyExitButton.onClick.AddListener(CloseDifficultyPanel);
    }

    private void ToggleMusic()
    {
        musicOn = !musicOn;
        musicSource.volume = musicOn ? 1 : 0;
    }

    private void ToggleSFX()
    {
        sfxOn = !sfxOn;
        sfxSource.volume = sfxOn ? 1 : 0;
    }

    private void ToggleVibration()
    {
        vibrationOn = !vibrationOn;
        Debug.Log("Vibration: " + vibrationOn);
    }

    private void ToggleTutorial()
    {
        tutorialOn = !tutorialOn;
        Debug.Log("Tutorial: " + tutorialOn);
    }

    private void OpenDifficultyPanel()
    {
        settingsContent.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    private void CloseDifficultyPanel()
    {
        difficultyPanel.SetActive(false);
        settingsContent.SetActive(true);
    }

    private void OpenHelpPanel()
    {
        helpPanel.SetActive(true);
    }
}