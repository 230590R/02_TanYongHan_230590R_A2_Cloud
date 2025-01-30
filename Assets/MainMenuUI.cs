using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {

  [Serializable] public class LerpableInputField {
    public InputField field;
    public RectTransform target;
    public float speed;
    public void Update() {
      field.transform.position = Vector3.Lerp(field.transform.position, target.position, speed * Time.deltaTime);
    }
  }



  LerpableInputField inputDisplayName;
  LerpableInputField inputUsername;
  LerpableInputField inputEmail;
  LerpableInputField inputPassword;
  LerpableInputField inputConfirmPassword;




  public enum State {
    LOGIN_GUEST,
    LOGIN,
    FORGOR,
    REGISTER
  }


  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    inputDisplayName.Update();
    inputUsername.Update();
    inputEmail.Update();
    inputPassword.Update();
    inputConfirmPassword.Update();
  }
}
