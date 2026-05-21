using System;
using DG.Tweening;
using GF;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
namespace Ferry.Screens
{
    public class SettingsScreen : BaseScreen<ScreenType>
    {
        [Header("Buttons")]
        [SerializeField] private ToggleBtn musicToggle;
        [SerializeField] private ToggleBtn sfxToggle;
        [SerializeField] private ToggleBtn vibrationToggle;
        [SerializeField] private Button difficultyButton;
        [SerializeField] private Button tutorialButton;
        [SerializeField] private Button helpButton;
        [SerializeField] private Button difficultyExitButton;

        [Space]
        [SerializeField] Button mEasyBtm;
        [SerializeField] Button mMediumBtn;
        [SerializeField] Button mHardBtn;
        [Header("Panels")]
        [SerializeField] private GameObject difficultyPanel;
        [SerializeField] private GameObject helpPanel;
        private bool tutorialOn = true;
        public Button backBtn;
        [SerializeField] DifficultyLevel difficultyLevel;

        protected override void OnEnable()
        {
            backBtn.onClick.AddListener(Close);
            musicToggle.AddListener(ToggleMusic);
            sfxToggle.AddListener(ToggleSFX);
            vibrationToggle.AddListener(ToggleVibration);
            difficultyButton.onClick.AddListener(OpenDifficultyPanel);
            tutorialButton.onClick.AddListener(ToggleTutorial);
            helpButton.onClick.AddListener(OpenHelpPanel);
            difficultyExitButton.onClick.AddListener(CloseDifficultyPanel);
            mEasyBtm.onClick.AddListener(() =>
            {
                Debug.LogWarning("Easy");
                difficultyLevel = DifficultyLevel.easy;
                var setting = GetSettingsData();
                setting.level = difficultyLevel;
                SaveSettings(setting);
                difficultyPanel.SetActive(false);
            });

            mMediumBtn.onClick.AddListener(() =>
            {   Debug.LogWarning("Medium");
                difficultyLevel = DifficultyLevel.medium;
                var setting = GetSettingsData();
                setting.level = difficultyLevel;
                SaveSettings(setting);
                difficultyPanel.SetActive(false);
            });

            mHardBtn.onClick.AddListener(() =>
            {   Debug.LogWarning("Hard");
                difficultyLevel = DifficultyLevel.hard;
                var setting = GetSettingsData();
                setting.level = difficultyLevel;
                SaveSettings(setting);
                difficultyPanel.SetActive(false);
            });

            GetSettings();
        }

        private void Close()
        {
            SwitchScreen(ScreenType.Home);
        }

        protected override void OnDisable()
        {
            backBtn.onClick.RemoveListener(Close);
            musicToggle.RemoveListener();
            sfxToggle.RemoveListener();
            vibrationToggle.RemoveListener();
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

        void SaveSettings(SettingsData settingsData)
        {
            string json = JsonConvert.SerializeObject(settingsData, Formatting.Indented);

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

        SettingsData GetSettingsData()
        {
            if (!PlayerPrefs.HasKey("Settings"))
                return new SettingsData();

            string json = PlayerPrefs.GetString("Settings");
            return JsonConvert.DeserializeObject<SettingsData>(json);
        }
        private void ToggleMusic(bool isOn)
        {
            var setting = GetSettingsData();
            setting.musicOn = isOn;
            SaveSettings(setting);
        }

        private void ToggleSFX(bool isOn)
        {
            var setting = GetSettingsData();
            setting.sfxOn=isOn;
            SaveSettings(setting);
        }

        private void ToggleVibration(bool isOn)
        {
            var setting=GetSettingsData();
            setting.vibrationOn=isOn;
            SaveSettings(setting);
        }

        private void ToggleTutorial()
        {
            tutorialOn = !tutorialOn;
            var setting=GetSettingsData();
            setting.tutorialOn=tutorialOn;
            SaveSettings(setting);
        }

        private void OpenDifficultyPanel()
        {
            difficultyPanel.SetActive(true);
            difficultyPanel.transform.localScale = Vector3.zero;
            difficultyPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
        }

        private void CloseDifficultyPanel()
        {
            difficultyPanel.transform.DOScale(0f, 0.2f).OnComplete(() =>
            {
                difficultyPanel.SetActive(false);
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
        public bool musicOn;
        public bool sfxOn;
        public bool vibrationOn;
        public bool tutorialOn;
        public DifficultyLevel level;
    }

    public enum DifficultyLevel { easy, medium, hard }
}
