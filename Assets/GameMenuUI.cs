using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuUI : MonoBehaviour {
  [SerializeField] string LoginMenu;
  [SerializeField] NotificationPanel motdPanel;
  [SerializeField] NotificationPanel successPanel;
  [SerializeField] NotificationPanel errorPanel;

  [SerializeField] NotificationPanel localLBPanel;
  [SerializeField] NotificationPanel globalLBPanel;

  public List<LBEntry> localEntries = new List<LBEntry>();
  public List<LBEntry> globalEntries = new List<LBEntry>();

  // Start is called before the first frame update
  void Start() {
    PlayfabManager.GetMOTD(OnGetMOTD, OnPlayfabError);
    LBEntry[] localLBs = localLBPanel.GetComponentsInChildren<LBEntry>();
    foreach (LBEntry lb in localLBs) {
      localEntries.Add(lb);
    }
    LBEntry[] globalLBs = globalLBPanel.GetComponentsInChildren<LBEntry>();
    foreach (LBEntry lb in globalLBs) {
      globalEntries.Add(lb);
    }
    StartCoroutine(AutoUpdateLB(3));
  }

  // Update is called once per frame
  void Update() {

  }

  public void OnLogoutClicked() {
    PlayfabManager.Logout(OnLogout, OnPlayfabError);
  }

  public void OnToggleLocalLB() {
    localLBPanel.TogglePanel();
  }

  public void OnToggleGlobalLB() {
    globalLBPanel.TogglePanel();
  }




  public void OnPlayfabError(string message) {
    if (errorPanel != null)
      errorPanel.DisplayMessage(message, 5);
  }

  public void OnGetMOTD(string message) {
    motdPanel.DisplayMessage(message, -1);
  }


  public void OnSuccess(string message) {
    if (successPanel != null)
      successPanel.DisplayMessage(message, 2);
  }

  public void OnLocalLeaderboardSuccess(List<LBEntry.LeaderboardEntry> entries) {
    
    int max = Mathf.Min(localEntries.Count, entries.Count);
    int rank = 0;
    for (int i = 0; i < localEntries.Count; i++) {
      if (i >= entries.Count) {
        rank++;
        localEntries[i].SetEntry(rank.ToString(), "-", "-");
        continue;
      }
      rank = entries[i].rank;

      localEntries[i].SetEntry(
        entries[i].rank.ToString(),
        entries[i].username,
        entries[i].score.ToString()
      );
      if (successPanel != null)
        successPanel.DisplayMessage("Successfully fetched local leaderboard!", 2);
    }

  }

  public void OnGlobalLeaderboardSuccess(List<LBEntry.LeaderboardEntry> entries) {

    int max = Mathf.Min(globalEntries.Count, entries.Count);
    int rank = 0;
    for (int i = 0; i < globalEntries.Count; i++) {
      if (i >= entries.Count) {
        rank++;
        globalEntries[i].SetEntry(rank.ToString(), "-", "-");
        continue;
      }
      rank = entries[i].rank;

      globalEntries[i].SetEntry(
        entries[i].rank.ToString(),
        entries[i].username,
        entries[i].score.ToString()
      );

      if (successPanel != null)
        successPanel.DisplayMessage("Successfully fetched global leaderboard!", 2);
    }

  }

  public void OnLogout(string message) {
    successPanel.DisplayMessage(message, 2);
    StartCoroutine(SwitchSceneAfterDelay(LoginMenu, 1));
  }

  public void OnHangar() {
    SceneManager.LoadScene("Hangar");
  }

  IEnumerator SwitchSceneAfterDelay(string scene, float delay) {
    yield return new WaitForSeconds(delay);
    SceneManager.LoadScene(scene);  
  }

  IEnumerator AutoUpdateLB(float delay) {
    while (true) {
      PlayfabManager.GetLocalLeaderboard(OnLocalLeaderboardSuccess, OnPlayfabError);
      PlayfabManager.GetGlobalLeaderboard(OnGlobalLeaderboardSuccess, OnPlayfabError);
      yield return new WaitForSeconds(delay);
    }

  }
}
