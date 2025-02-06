using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

  public Vector3 positionAsteroid;
  public GameObject asteroid;
  public GameObject asteroid2;
  public GameObject asteroid3;
  public int hazardCount;
  public float startWait;
  public float spawnWait;
  public float waitForWaves;
  public Text scoreText;
  public Text gameOverText;
  public Text restartText;
  public Text mainMenuText;

  private bool restart;
  private bool gameOver;
  private int score;
  private List<GameObject> asteroids;
  

  // new stuff
  public NotificationPanel SubmitPanel;
  public NotificationPanel successPanel, errorPanel;
  public int xp, xpGained;
  public int money, moneyGained;
  public int timesPlayed;
  public int speedLevel, livesLevel, moneyRateLevel;
  public int lives;
  private void Start() {
    asteroids = new List<GameObject> {
            asteroid,
            asteroid2,
            asteroid3
        };
    gameOverText.text = "";
    restartText.text = "";
    mainMenuText.text = "";
    restart = false;
    gameOver = false;
    score = 0;
    timesPlayed = 0;
    StartCoroutine(spawnWaves());
    StartCoroutine(GiveXP(1,0.5f));
    updateScore();
    //SubmitPanel.gameObject.SetActive(false);
    //PlayfabManager.SetUserData(OnSuccess, OnPlayfabErrorCallback, "XP", "0");
    PlayfabManager.GetUserData(
      (value) => {
        xp = System.Int32.Parse(value);
        updateScore();
        OnSuccess("Updated XP!");
      }, OnPlayfabErrorCallback, "XP");

    PlayfabManager.GetUserData(
      (value) => {
        timesPlayed = System.Int32.Parse(value);
        updateScore();
        OnSuccess("Updated Times played!");
      }, OnPlayfabErrorCallback, "TimesPlayed");

    PlayfabManager.GetVirtualCurrency(value => { 
      money = value; 
      updateScore();
      OnSuccess("Updated Money!");
    }, OnPlayfabErrorCallback);

    GetUpgrades();
  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Alpha9)) {
      moneyGained += 500;
      updateScore();
    }
  }

  private IEnumerator spawnWaves() {
    yield return new WaitForSeconds(startWait);
    while (true) {
      for (int i = 0; i < hazardCount; i++) {
        Vector3 position = new Vector3(Random.Range(-positionAsteroid.x, positionAsteroid.x), positionAsteroid.y, positionAsteroid.z);
        Quaternion rotation = Quaternion.identity;
        Instantiate(asteroids[Random.Range(0, 3)], position, rotation);
        yield return new WaitForSeconds(spawnWait);
      }
      yield return new WaitForSeconds(waitForWaves);
      if (gameOver) {
        break;
      }
    }
  }


  private IEnumerator GiveXP(int amount, float interval) {
    while (gameOver == false) {
      xpGained += amount;
      updateScore();
      yield return new WaitForSeconds(interval);
    }
  }

  public bool gameIsOver() {
    lives--;
    if (lives > 0) {
      return false;
    }
    timesPlayed++;

    updateScore();
    PlayfabManager.SetUserData(OnSuccess, OnPlayfabErrorCallback, "XP", (xp+xpGained).ToString());
    PlayfabManager.SetUserData(OnSuccess, OnPlayfabErrorCallback, "TimesPlayed", (timesPlayed).ToString());
    PlayfabManager.AddVirtualCurrency(OnSuccess, OnPlayfabErrorCallback, moneyGained);
    gameOverText.text = "Score: " + score.ToString() + "\n";
    gameOverText.text += "XP Gained:" + xpGained.ToString() +"\n";
    gameOverText.text += "Money Gained: $" + moneyGained.ToString();
    gameOver = true;
    SubmitPanel.OpenPanel(-1);
    return true;
  }

  public void addScore(int score) {
    if (gameOver) return;
    this.score += score;
    this.xpGained += 50;
    moneyGained += 10 + (int)Mathf.Floor(20 * moneyRateLevel / 12f);
    updateScore();
  }

  void updateScore() {
    scoreText.text = "Score: " + score + 
      "\nXP: " + (xp + xpGained).ToString() +
      "\nLives: " + lives.ToString() +
      "\nDabloons: $" + (money+moneyGained).ToString() + 
      "\nTimes played: " + timesPlayed.ToString();
  }

  public void OnClickSubmit() {
    PlayfabManager.SubmitLeaderboard(OnSuccess, OnPlayfabErrorCallback, score);
  }

  public void OnClickExit() {
    SceneManager.LoadScene("Menu");
  }

  public void OnClickRestart() {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
  }

  public void OnPlayfabErrorCallback(string msg) {
    errorPanel.DisplayMessage(msg, 2);
  }

  public void OnSuccess(string msg) {
    successPanel.DisplayMessage(msg, 2);
  }

  public void GetUpgrades() {
    PlayfabManager.GetJSONData((entries) => {
      foreach (ShipUpgrade e in entries) {
        if (e.name == "Speed") {
          speedLevel = e.level;
          PlayerControl.speedBoost = 1 + 2 * (speedLevel / 12f);
        }
        else if (e.name == "Lives") {
          livesLevel = e.level;
          lives = livesLevel;
        }
        else if (e.name == "Money") {
          moneyRateLevel = e.level;
        }
      }
      OnSuccess("Successfully received upgrades!");
    }, OnPlayfabErrorCallback, "Upgrades");
  }
}
