using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NotificationPanel : MonoBehaviour {
  [SerializeField] Transform visiblePosition;
  [SerializeField] Transform hiddenPosition;
  private Transform _target;
  [SerializeField] TMPro.TMP_Text messageBox;

  IEnumerator CloseNotifAfterSeconds(float duration) {
    yield return new WaitForSeconds(duration);
    _target = hiddenPosition;
  }

  private void Start() {
    transform.position = Vector3.Lerp(transform.position, hiddenPosition.position, 1);
    _target = hiddenPosition;
  }
  // Update is called once per frame
  void Update() {
    if (_target != null) 
      transform.position = Vector3.Lerp(transform.position, _target.position, Time.deltaTime * 5);
  }

  public void DisplayMessage(string message, float duration) {
    _target = visiblePosition;
    messageBox.text = message;
    if (duration > 0) {
      StartCoroutine(CloseNotifAfterSeconds(duration));
    }
  }

  public void OpenPanel(float duration) {
    _target = visiblePosition;
    if (duration > 0) {
      StartCoroutine(CloseNotifAfterSeconds(duration));
    }
  }

  public void ClosePanel() {
    _target = hiddenPosition;

  }

  public void TogglePanel() {
    if (_target == visiblePosition) {
      _target = hiddenPosition;
    }
    else {
      _target = visiblePosition;
    }
  }
}
