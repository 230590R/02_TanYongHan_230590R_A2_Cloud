using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;

[System.Serializable]
public class Boundary {
  public float xMin, xMax, zMin, zMax;
}

public class PlayerControl : MonoBehaviour {

  private Rigidbody playerRb;
  private AudioSource playerWeapon;
  public float speed;
  public float tiltMultiplier;
  public Boundary boundary;

  public GameObject shot;
  public Transform shotSpawn;
  public Transform shotSpawn2;
  public float fireRate;

  private float nextFire;
  private CharacterSelection characterSelection;

  public static float speedBoost = 1;

  public List<ItemInstance> inventory = new List<ItemInstance>();
  public Dictionary<string, bool> weapons = new Dictionary<string, bool>();

  private void Start() {
    GameObject cSelectionObject = GameObject.FindWithTag("CharacterSelection");
    if (cSelectionObject != null) {
      characterSelection = cSelectionObject.GetComponent<CharacterSelection>();
    }


    playerRb = GetComponent<Rigidbody>();
    playerWeapon = GetComponent<AudioSource>();


    PlayfabManager.GetPlayerInventory(items => OnGetInventory(items), (msg) => Debug.Log("boo womp"));
  }

  private void OnGetInventory(List<ItemInstance> items) {
    inventory = items;
    foreach (ItemInstance item in inventory) {
      weapons.TryAdd(item.ItemId, true);
    }
  }

  private void Update() {


    if (Input.GetButton("Jump") && Time.time > nextFire) {
      nextFire = Time.time + fireRate;

      Shoot(0);
      if (weapons.ContainsKey("Backshots")) {
        Shoot(180);
      }

      if (weapons.ContainsKey("BackshotsDeluxe")) {
        int count = 180;
        for (int i = 0; i <= count; i++) {
          float t = (float)i / count;
          float angle = 135f + (90*t);
          Shoot(angle);
        }
      }

      if (weapons.ContainsKey("Spreadshot1")) {
        int count = 2;
        for (int i = 0; i <= count; i++) {
          float t = (float)i / count;
          float angle = -45 + (90 * t);
          Shoot(angle);
        }
      }

      if (weapons.ContainsKey("Spreadshot2")) {
        int count = 5;
        for (int i = 0; i <= count; i++) {
          float t = (float)i / count;
          float angle = -45 + (90 * t);
          Shoot(angle);
        }
      }

      playerWeapon.Play();
    }

  }

  private void Shoot(float angle) {
    Quaternion rotation = Quaternion.Euler(0, angle, 0);
    if (characterSelection.getIndex() == 1) {
      Instantiate(shot, shotSpawn.position, Quaternion.Euler(0, shotSpawn.eulerAngles.y + angle, 0));
      Instantiate(shot, shotSpawn2.position, Quaternion.Euler(0, shotSpawn2.eulerAngles.y + angle, 0));
    }
    else {
      Instantiate(shot, shotSpawn.position, Quaternion.Euler(0, shotSpawn.eulerAngles.y + angle, 0));
    }
  }

  private void FixedUpdate() {
    float moveHorizontal = Input.GetAxis("Horizontal");
    float moveVertical = Input.GetAxis("Vertical");

    playerRb.velocity = new Vector3(moveHorizontal * speed * speedBoost, 0.0f, moveVertical * speed * speedBoost);

    playerRb.position = new Vector3(
        Mathf.Clamp(playerRb.position.x, boundary.xMin, boundary.xMax),
        0.0f,
        Mathf.Clamp(playerRb.position.z, boundary.zMin, boundary.zMax)
    );

    playerRb.rotation = Quaternion.Euler(0.0f, 0.0f, -playerRb.velocity.x * tiltMultiplier);
  }
}
