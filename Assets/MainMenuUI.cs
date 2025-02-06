using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
  [SerializeField] PlayfabManager playfabManager;
  [SerializeField] string GameMenu;
  [SerializeField] TMPro.TMP_Text LoginTypeBtnText;
  [SerializeField] TMPro.TMP_Text InputBtnText;
  [SerializeField] DynamicInputField inputDisplayname;
  [SerializeField] DynamicInputField inputUsername;
  [SerializeField] DynamicInputField inputEmail;
  [SerializeField] DynamicInputField inputPassword;
  [SerializeField] DynamicInputField inputConfirmPassword;
  [SerializeField] NotificationPanel errorNotif;
  [SerializeField] NotificationPanel successNotif;

  public enum State {
    LOGIN,
    FORGOR,
    REGISTER
  }

  private enum LoginType {
    GUEST,
    EMAIL,
    USERNAME,
    MAX,
    NONE
  }
  [SerializeField] private State state = State.REGISTER;
  [SerializeField] private LoginType loginType = LoginType.NONE;


  // Start is called before the first frame update
  void Start() {
    
  }



  // Update is called once per frame
  void Update() {
  }

  public void OnToggleLoginType() {
    state = State.LOGIN;
    loginType++;
    if (loginType >= LoginType.MAX) loginType = 0;
    switch (loginType) {
      case LoginType.GUEST:
        LoginTypeBtnText.text = "Guest";
        InputBtnText.text = "Guest Login";

        inputDisplayname.Close();
        inputUsername.Close();
        inputEmail.Close();
        inputPassword.Close();
        inputConfirmPassword.Close();
        break;
      case LoginType.EMAIL:
        LoginTypeBtnText.text = "Email";
        InputBtnText.text = "Email Login";

        inputDisplayname.Close();
        inputUsername.Close();
        inputEmail.Open();
        inputPassword.Open();
        inputConfirmPassword.Close();
        break;
      case LoginType.USERNAME:
        LoginTypeBtnText.text = "Username";
        InputBtnText.text = "Username Login";

        inputDisplayname.Close();
        inputUsername.Open();
        inputEmail.Close();
        inputPassword.Open();
        inputConfirmPassword.Close();
        break;
    }
  }

  public void OnToggleRegister() {
    state = State.REGISTER;
    loginType = LoginType.NONE;
    LoginTypeBtnText.text = "-";
    InputBtnText.text = "Register";

    inputDisplayname.Open();
    inputUsername.Open();
    inputEmail.Open();
    inputPassword.Open();
    inputConfirmPassword.Open();
  }

  public void OnToggleForgotPassword() {
    state = State.FORGOR;
    loginType = LoginType.NONE;
    LoginTypeBtnText.text = "-";
    InputBtnText.text = "Reset Password";
    inputDisplayname.Close();
    inputUsername.Close();
    inputEmail.Open();
    inputPassword.Close();
    inputConfirmPassword.Close();
  }

  public void OnMainButtonPress() {
    if (state == State.LOGIN) {
      switch (loginType) {
        case LoginType.GUEST:
          PlayfabManager.LoginCustomID(OnLoginSuccess, OnPlayfabErrorCallback);
          break;        
        case LoginType.EMAIL:
          PlayfabManager.LoginEmail(OnLoginSuccess, OnPlayfabErrorCallback, 
            email: inputEmail.inputField.text, 
            password: inputPassword.inputField.text);
          break;        
        case LoginType.USERNAME:
          PlayfabManager.LoginUsername(OnLoginSuccess, OnPlayfabErrorCallback,
            username: inputUsername.inputField.text,
            password: inputPassword.inputField.text);
          break;
      }
    }
    else if (state == State.REGISTER) {
      if (inputPassword.inputField.text != inputConfirmPassword.inputField.text) {
        OnPlayfabErrorCallback("Password mismatch! Please input again.");
        return;
      }



      PlayfabManager.Register(OnRegisterSuccess, OnPlayfabErrorCallback,
        displayname: inputDisplayname.inputField.text,
        username: inputUsername.inputField.text,
        email: inputEmail.inputField.text,
        password: inputPassword.inputField.text);
    }
    else if (state == State.FORGOR) {
      PlayfabManager.ResetPassword(OnResetSuccess, OnPlayfabErrorCallback, email: inputEmail.inputField.text);
    }

  }


  public void OnPlayfabErrorCallback(string str) {
    Debug.Log("Error: " + str);
    errorNotif.DisplayMessage(str, 2);
  }

  public void OnLoginSuccess(string str) {
    Debug.Log("Login Success! " + str);
    successNotif.DisplayMessage(str, 1);
    StartCoroutine(WaitBeforeLoadScene(GameMenu, 0.5f));
  }

  public void OnRegisterSuccess(string str) {
    Debug.Log("Register Success! " + str);
    successNotif.DisplayMessage(str, 2);
    loginType = LoginType.USERNAME - 1;
    OnToggleLoginType();
  }

  public void OnResetSuccess(string str) {
    Debug.Log("Register Success! " + str);
    successNotif.DisplayMessage(str, 2);
    loginType = LoginType.USERNAME - 1;
    OnToggleLoginType();
  }


  IEnumerator WaitBeforeLoadScene(string sceneName, float duration) {
    yield return new WaitForSeconds(duration);
    SceneManager.LoadScene(sceneName);
  }
}
