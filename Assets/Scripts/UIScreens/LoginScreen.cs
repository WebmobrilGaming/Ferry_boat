using System;
using DG.Tweening;
using Ferry_boat.Assets.Scripts.Web;
using GF;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Ferry.Screens
{
    public class LoginScreen : BaseScreen<ScreenType>
    {
        public Button newUserBtn;
        public Button prevUserBtn;
        public Button submitNewUserBtn;
        public Button submitPrevUserBtn;
        public Button closePrevUserPanelBtn;
        public Button closeNewUserPanelBtn;
        public Transform newUserPanel;
        public Transform prevUserPanel;
        public Transform loginContentPanel;

        public TMP_InputField prevUsernameInput;
        public TMP_InputField newFirstNameInput;
        public TMP_InputField newLastNameInput;
        public TMP_InputField newUsernameInput;

        public TMP_Text newFirstNameErrorTxt;
        public TMP_Text newLastNameErrorTxt;
        public TMP_Text newUsernameErrorTxt;
        public TMP_Text prevUsernameErrorTxt;
        protected override void OnEnable()
        {
            if (PlayerPrefs.HasKey("username"))
            {
                DebugUtils.DevDebug.Log($"Username is available: {PlayerPrefs.GetString("username")}", DebugColor.Indigo);

                string username = PlayerPrefs.GetString("username");

                SubmitPrevUser(username);
               // SwitchScreen(ScreenType.Home);
            }
            else
            {
                prevUserPanel.localScale = Vector3.zero;
                newUserPanel.localScale = Vector3.zero;
                loginContentPanel.DOScale(1, 0.5f).SetEase(Ease.OutBack);
                newUserBtn.AddListener(null, CreateNewUser);
                prevUserBtn.AddListener(null, LogInPreviousUser);
                submitNewUserBtn.AddListener(null, SubmitNewUser);
                submitPrevUserBtn.AddListener(null, () =>
                {
                    string prevUsername = prevUsernameInput.text.Trim();

                    SubmitPrevUser(prevUsername);
                });
                closeNewUserPanelBtn.AddListener(null, CloseNewUserPanel);
                closePrevUserPanelBtn.AddListener(null, ClosePrevUserPanel);
            }
        }

        private void ClosePrevUserPanel()
        {
            newUserPanel.localScale = Vector3.zero;
            prevUserPanel.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                loginContentPanel.DOScale(1, 0.5f).SetEase(Ease.OutBack);
            });
        }

        private void CloseNewUserPanel()
        {
            prevUserPanel.localScale = Vector3.zero;
            newUserPanel.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                loginContentPanel.DOScale(1, 0.5f).SetEase(Ease.OutBack);
            });
        }

        private void SubmitPrevUser(string username)
        {
           // string prevUsername = prevUsernameInput.text.Trim();

            if (string.IsNullOrEmpty(username) || username.Length < 3)
            {
                return;
            }
            var request = new CreatePlayer("previous", username);
            APIManager.PostAPI<UserDetails>(new RequestData(Netconfig.RequestType.LoginPlayer, request), OnLogin);
        }

        private void OnLogin(UserDetails details, Response response)
        {
            if (response.status)
            {
                UserDataManager.Instance.SignIn(details);
                PlayerPrefs.SetString("username",JsonConvert.SerializeObject(details));
                SwitchScreen(ScreenType.Home);
            }
            else
            {
                string prevUsername = prevUsernameInput.text.Trim();
                var request = new CreatePlayer("new", prevUsername, $"New player{UnityEngine.Random.Range(0, 999)}", $"wos{UnityEngine.Random.Range(0, 999)}");
                APIManager.PostAPI<UserDetails>(new RequestData(Netconfig.RequestType.CreatePlayer, request), OnReceivedNewUser);
            }
        }

        private void SubmitNewUser()
        {
            string firstName = newFirstNameInput.text.Trim();
            string lastName = newLastNameInput.text.Trim();
            string username = newUsernameInput.text.Trim();
            if (!ValidDetails(firstName, lastName, username)) return;
            var request = new CreatePlayer("new", username, firstName, lastName);
            APIManager.PostAPI<UserDetails>(new RequestData(Netconfig.RequestType.CreatePlayer, request), OnReceivedNewUser);
        }

        private void OnReceivedNewUser(UserDetails details, Response response)
        {
            if (response.status)
            {
                UserDataManager.Instance.SignIn(details);
                PlayerPrefs.SetString("username",details.data.username);
                SwitchScreen(ScreenType.Home);
            }
            else
            {
                Utils.ShowOkPopup("Error", response.message, null);
            }
        }

        private bool ValidDetails(string firstName, string lastName, string username)
        {
            bool status = true;
            if (string.IsNullOrEmpty(firstName))
            {
                status = false;
                newFirstNameErrorTxt.text = "First name should not be empty";
            }
            if (firstName.Length < 3)
            {
                status = false;
                newFirstNameErrorTxt.text = "First name length should be greater than or equal to 3";
            }
            if (string.IsNullOrEmpty(lastName))
            {
                status = false;
                newLastNameErrorTxt.text = "Last name should not be empty";
            }
            if (string.IsNullOrEmpty(username))
            {
                status = false;
                newUsernameErrorTxt.text = "User name should not be empty";
            }
            if (username.Length < 3)
            {
                status = false;
                newUsernameErrorTxt.text = "User name length should be greater than or equal to 3";
            }
            return status;
        }

        private void LogInPreviousUser()
        {
            loginContentPanel.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                newUserPanel.localScale = Vector3.zero;
                prevUserPanel.DOScale(1, 0.5f).SetEase(Ease.OutBack);
            });
        }

        private void CreateNewUser()
        {
            loginContentPanel.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                prevUserPanel.localScale = Vector3.zero;
                newUserPanel.DOScale(1, 0.5f).SetEase(Ease.OutBack);
            });
        }
        protected override void OnDisable()
        {
            newUserBtn.RemoveListener();
            prevUserBtn.RemoveListener();
            submitNewUserBtn.RemoveListener();
            submitPrevUserBtn.RemoveListener();
        }
    }
}
