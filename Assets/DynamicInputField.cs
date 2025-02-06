using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class DynamicInputField : MonoBehaviour {

  public TMPro.TMP_InputField inputField;
  public RectTransform rectTransform;
  public float targetHeight = 55;
  [SerializeField] private float defaultHeight = 55;
  public float DefaultHeight { get => defaultHeight; }
  public static float speed = 7.5f;

  // Start is called before the first frame update
  void Start() {
    rectTransform = GetComponent<RectTransform>();
  }

  // Update is called once per frame
  void Update() {
    float height = Mathf.Lerp(rectTransform.sizeDelta.y, targetHeight, Time.deltaTime * speed);

    rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, height);
  }

  public void Close() {
    targetHeight = 0;
  }

  public void Open() {
    targetHeight = defaultHeight;
  }
}
