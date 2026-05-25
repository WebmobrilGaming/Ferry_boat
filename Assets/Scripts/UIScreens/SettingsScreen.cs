using System;
using DG.Tweening;
using GF;
using Newtonsoft.Json;
using Unity.VisualScripting;
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
            Debug.LogWarning("Music key Player pref");
            if(setting.musicOn == true)
            {
                PlayerPrefs.SetInt("isMusicON",1);
            }
            else
            {
                PlayerPrefs.SetInt("isMusicON",0);
            }
            SaveSettings(setting);
        }

        private void ToggleSFX(bool isOn)
        {
            var setting = GetSettingsData();
            setting.sfxOn=isOn;
            Debug.LogWarning("SFX key Player pref");
            if (setting.sfxOn == true)
            {
                PlayerPrefs.SetInt(GamePrefs.isSFXOn, 1);
            }
            else if(setting.sfxOn == false )
            {
                PlayerPrefs.SetInt(GamePrefs.isSFXOn, 0);
            }
            Debug.LogWarning(setting.sfxOn);
            Debug.LogWarning("SFX value : "+ PlayerPrefs.GetInt(GamePrefs.isSFXOn,0));
            SaveSettings(setting);
        }

        private void ToggleVibration(bool isOn)
        {
            var setting=GetSettingsData();
            setting.vibrationOn=isOn;
            Debug.LogWarning("Vibration key Player pref");
            if (setting.vibrationOn == true)
            {
                PlayerPrefs.SetInt(GamePrefs.isVibrationOn, 1);
            }
            else if (setting.vibrationOn == false)
            {
                PlayerPrefs.SetInt(GamePrefs.isVibrationOn, 0);
            }
            Debug.LogWarning(setting.vibrationOn
            );
            Debug.LogWarning("Vibration value : " + PlayerPrefs.GetInt(GamePrefs.isVibrationOn, 0));
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
        // public void ToggleChecker()
        // {
        //      int musicCheck = PlayerPrefs.GetInt(GamePrefs.isMusicOn,0);
        //      int SfxCheck = PlayerPrefs.GetInt(GamePrefs.isSFXOn);
        //      int vibrationCheck=PlayerPrefs.GetInt(GamePrefs.isVibrationOn,0);
             
        // }
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
