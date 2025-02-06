using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordHider : MonoBehaviour {
  [SerializeField] TMPro.TMP_Text passwordText;
  [SerializeField] TMPro.TMP_Text confirmText;
  [SerializeField] TMPro.TMP_Text passwordCensor;
  [SerializeField] TMPro.TMP_Text confirmCensor;
  [SerializeField] Image visibleIcon;
  [SerializeField] Image hiddenIcon;

  bool visible = true;

  public void OnToggleVisibility() {
    visible = !visible;
    if (visible) {
      passwordText.enabled = true;
      confirmText.enabled = true;
      passwordCensor.enabled = false;
      confirmCensor.enabled = false;
      visibleIcon.enabled = true;
      hiddenIcon.enabled = false;
    }
    else {
      passwordText.enabled = false;
      confirmText.enabled = false;
      passwordCensor.enabled = true;
      confirmCensor.enabled = true;
      visibleIcon.enabled = false;
      hiddenIcon.enabled = true;

      passwordCensor.text = new string('*', passwordText.text.Length);
      confirmCensor.text = new string('*', confirmText.text.Length);
    }
  }
}
