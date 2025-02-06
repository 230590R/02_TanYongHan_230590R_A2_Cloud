using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineMover : MonoBehaviour {
  [SerializeField] private float _speed;
  [SerializeField] private float _magnitude;
  private float _elapsed;
  private Vector3 _initialPosition;

  // Start is called before the first frame update
  void Start() {
    _initialPosition = transform.position;
  }

  // Update is called once per frame
  void Update() {
    _elapsed += Time.deltaTime * _speed;
    transform.position = _initialPosition + new Vector3(0,Mathf.Sin(_elapsed)*_magnitude, 0);
  }
}
