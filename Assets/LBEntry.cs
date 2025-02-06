using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LBEntry : MonoBehaviour {
  public TMPro.TMP_Text rankText, usernameText, scoreText;

  public class LeaderboardEntry {
    public int rank;
    public string username;
    public int score;
    public LeaderboardEntry(int rank, string username, int score) { this.rank = rank; this.username = username; this.score = score; }
  }


  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  public void SetEntry(string rank, string username, string score) {
    rankText.text = rank;
    usernameText.text = username;
    scoreText.text = score;
  }
}
