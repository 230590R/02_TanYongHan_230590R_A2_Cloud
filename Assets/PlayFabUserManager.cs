using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class PlayFabUserManager : MonoBehaviour {
  [SerializeField] private TMPro.TMP_Text messageBox, MOTDBox;
  [SerializeField] private TMPro.TMP_InputField usernameField, emailField, passwordField;
  [SerializeField] private string MenuScene;
  [SerializeField] private string LoginScene;

  private void Start() {
    if (MOTDBox) OnGetMOTD();
  }


  private void OnError(PlayFabError error) {
    messageBox.text = "Error: " + error.GenerateErrorReport();
  }


  #region Registration
  public void OnRegister() {
    var registrationRequest = new RegisterPlayFabUserRequest {
      Email = emailField.text,
      Password = passwordField.text,
      Username = usernameField.text
    };
    PlayFabClientAPI.RegisterPlayFabUser(registrationRequest, OnRegistrationSucc, OnError);
  }

  private void OnRegistrationSucc(RegisterPlayFabUserResult result) {
    messageBox.text = "Registration success! Your id: " + result.PlayFabId;
  }

  #endregion

  #region Login
  public void OnLogin() {
    var loginRequest = new LoginWithEmailAddressRequest() {
      Email = emailField.text,
      Password = passwordField.text
    };
    PlayFabClientAPI.LoginWithEmailAddress(loginRequest, OnLoginSucc, OnError);


  }
  private void OnLoginSucc(LoginResult result) {
    messageBox.text = "Login Success! ID: " + result.PlayFabId;

    SceneManager.LoadScene(MenuScene);
  }
  #endregion

  #region Log out
  private void OnLogout() {
    PlayFabClientAPI.ForgetAllCredentials();
    SceneManager.LoadScene(LoginScene);
  }


  #endregion

  #region Reset Password
  public void OnResetPassword() {
    var ResetPasswordRequest = new SendAccountRecoveryEmailRequest {
      Email = emailField.text,
      TitleId = PlayFabSettings.TitleId
    };

    PlayFabClientAPI.SendAccountRecoveryEmail(ResetPasswordRequest, OnResetPasswordSuccess, OnError);
  }

  void OnResetPasswordSuccess(SendAccountRecoveryEmailResult result) {
    messageBox.text = "Recovery email sent! Please check your inbox.";
  }
  #endregion

  #region Message Of The Day 
  void OnGetMOTD() {
    PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(), OnMOTDSuccess, OnError);
  }

  void OnMOTDSuccess(GetTitleDataResult result) {
    if (result.Data == null || !result.Data.ContainsKey("MOTD"))
      MOTDBox.text = "Error: MOTD not found.";
    else
      MOTDBox.text = "MOTD: " + result.Data["MOTD"];
  }
  #endregion
}
