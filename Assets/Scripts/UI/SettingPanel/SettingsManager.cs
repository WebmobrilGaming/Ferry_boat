using DG.Tweening;
using Newtonsoft.Json;
using System;
using UnityEditor.Overlays;
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

    [Space]
    [SerializeField] Button mEasyBtm;
    [SerializeField] Button mMediumBtn;
    [SerializeField] Button mHardBtn;

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

    [SerializeField]DifficultyLevel difficultyLevel;

    private void OnEnable()
    {
        musicButton.onClick.AddListener(ToggleMusic);
        sfxButton.onClick.AddListener(ToggleSFX);
        vibrationButton.onClick.AddListener(ToggleVibration);
        difficultyButton.onClick.AddListener(OpenDifficultyPanel);
        tutorialButton.onClick.AddListener(ToggleTutorial);
        helpButton.onClick.AddListener(OpenHelpPanel);
        difficultyExitButton.onClick.AddListener(CloseDifficultyPanel);

        mEasyBtm.onClick.AddListener(() =>
        {
            difficultyLevel = DifficultyLevel.easy;
            SaveSettings();
        });

        mMediumBtn.onClick.AddListener(() =>
        {
            difficultyLevel = DifficultyLevel.medium;
            SaveSettings();
        });

        mHardBtn.onClick.AddListener(() =>
        {
            difficultyLevel = DifficultyLevel.hard;
            SaveSettings();
        });

        GetSettings();
    }

    private void OnDisable()
    {
        musicButton.onClick.RemoveListener(ToggleMusic);
        sfxButton.onClick.RemoveListener(ToggleSFX);
        vibrationButton.onClick.RemoveListener(ToggleVibration);
        difficultyButton.onClick.RemoveListener(OpenDifficultyPanel);
        tutorialButton.onClick.RemoveListener(ToggleTutorial);
        helpButton.onClick.RemoveListener(OpenHelpPanel);
        difficultyExitButton.onClick.RemoveListener(CloseDifficultyPanel);

        mEasyBtm.onClick.RemoveListener(() =>
        {
            difficultyLevel = DifficultyLevel.easy;
        });

        mMediumBtn.onClick.RemoveListener(() =>
        {
            difficultyLevel = DifficultyLevel.medium;
        });
            
        mHardBtn.onClick.RemoveListener(() =>
        {
            difficultyLevel = DifficultyLevel.hard;
        });
    }

    private void Start()
    {
        
    }

    void SaveSettings()
    {
        var data = new SettingsData { level = difficultyLevel};
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);

        PlayerPrefs.SetString("Settings", json);
    }

    void GetSettings()
    {
        if (!PlayerPrefs.HasKey("Settings"))
            return;

        string json = PlayerPrefs.GetString("Settings");
        var loaded = JsonConvert.DeserializeObject<SettingsData>(json);

        difficultyLevel = loaded.level;
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
        difficultyPanel.transform.localScale = Vector3.zero;
        difficultyPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }

    private void CloseDifficultyPanel()
    {
        difficultyPanel.transform.DOScale(0f, 0.2f).OnComplete(() =>
        {
            difficultyPanel.SetActive(false);
            settingsContent.SetActive(true);

            settingsContent.transform.localScale = Vector3.zero;
            settingsContent.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        });
    }

    private void OpenHelpPanel()
    {
        helpPanel.SetActive(true);

        helpPanel.transform.localScale = Vector3.zero;
        helpPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }
}

[Serializable]
public class SettingsData
{
    public DifficultyLevel level;
}

public enum DifficultyLevel {easy, medium,hard }