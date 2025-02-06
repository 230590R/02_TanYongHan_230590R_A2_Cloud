using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab.ClientModels;

public class HangarUI : MonoBehaviour {
  [SerializeField] NotificationPanel successPanel, errorPanel;
  [SerializeField] NotificationPanel upgradesPanel, shopPanel, inventoryPanel;



  // Start is called before the first frame update
  void Start() {
    UpgradeTabInit();
    InitStoreTab();
    RefreshInventory();
  }

  // Update is called once per frame
  void Update() {
    if (Input.GetKeyDown(KeyCode.Alpha1)) {
      PlayfabManager.GetVirtualCurrency(amount => { successPanel.DisplayMessage(amount.ToString(), 1); }, OnPlayfabError);
    }
    if (Input.GetKeyDown(KeyCode.Alpha2)) {
      PlayfabManager.AddVirtualCurrency(msg => { successPanel.DisplayMessage(msg, 1); }, OnPlayfabError, 10);
    }
  }
  public void OnToggleUpgradesTab() {
    upgradesPanel.OpenPanel(-1);
    shopPanel.ClosePanel();
    inventoryPanel.ClosePanel();
  }
  public void OnToggleShopTab() {
    upgradesPanel.ClosePanel();
    shopPanel.OpenPanel(-1);
    inventoryPanel.ClosePanel();
  }
  public void OnToggleInventoryTab() {
    upgradesPanel.ClosePanel();
    shopPanel.ClosePanel();
    inventoryPanel.OpenPanel(-1);
    RefreshInventory();
  }

  #region Upgrades Tab
  [Header("Upgrade Tab")]
  public int upgradePoints;
  [SerializeField] GameObject speedUpgradeUI, livesUpgradeUI, moneyUpgradeUI;
  List<GameObject> speedBars, livesBars, moneyRateBars;
  public int speedNum, livesNum, moneyRateNum = 0;


  private void UpgradeTabInit() {
    speedBars = new List<GameObject>();
    livesBars = new List<GameObject>();
    moneyRateBars = new List<GameObject>();
    for (int i = 0; i < speedUpgradeUI.transform.childCount; i++) {
      speedBars.Add(speedUpgradeUI.transform.GetChild(i).gameObject);
    }
    for (int i = 0; i < livesUpgradeUI.transform.childCount; i++) {
      livesBars.Add(livesUpgradeUI.transform.GetChild(i).gameObject);
    }
    for (int i = 0; i < moneyUpgradeUI.transform.childCount; i++) {
      moneyRateBars.Add(moneyUpgradeUI.transform.GetChild(i).gameObject);
    }

    foreach (GameObject go in speedBars) {
      go.SetActive(false);
    }
    foreach (GameObject go in livesBars) {
      go.SetActive(false);
    }
    foreach (GameObject go in moneyRateBars) {
      go.SetActive(false);
    }


    PlayfabManager.GetJSONData((entries) => {
      foreach (ShipUpgrade e in entries) {
        if (e.name == "Speed") {
          speedNum = e.level;
        }
        else if (e.name == "Lives") {
          livesNum = e.level;
        }
        else if (e.name == "Money") {
          moneyRateNum = e.level;
        }
      }
      UpdateUpgradeBars();
      OnSuccess("Successfully received upgrades!");
    }, OnPlayfabError, "Upgrades");
  }

  public void OnReturnToMenu() {
    SceneManager.LoadScene("Menu");
  }

  public void OnIncrement(int type) {
    Debug.Log(speedBars.Count);
    switch (type) {
      case 0:
        if (speedNum >= speedBars.Count) break;
        speedNum++;

        break;
      case 1:
        if (livesNum >= livesBars.Count) break;
        livesNum++;

        break;
      case 2:
        if (moneyRateNum >= moneyRateBars.Count) break;
        moneyRateNum++;

        break;
    }
    UpdateUpgradeBars();
  }

  public void OnDecrement(int type) {
    switch (type) {
      case 0:
        if (speedNum <= 0) break;
        speedNum--;

        break;
      case 1:
        if (livesNum <= 0) break;
        livesNum--;

        break;
      case 2:
        if (moneyRateNum <= 0) break;
        moneyRateNum--;

        break;
    }
    UpdateUpgradeBars();
  }
  
  public void OnSaveUpgrades() {
    List<ShipUpgrade> m_upgrades = new List<ShipUpgrade>();
    m_upgrades.Add(new ShipUpgrade("Speed", speedNum));
    m_upgrades.Add(new ShipUpgrade("Lives", livesNum));
    m_upgrades.Add(new ShipUpgrade("Money", moneyRateNum));
    PlayfabManager.SetJSONData(OnSuccess, OnPlayfabError, m_upgrades, "Upgrades");
  }

  private void UpdateUpgradeBars() {
    for (int i = 0; i < speedBars.Count; i++) {
      speedBars[i].SetActive(speedNum > i);
    }
    for (int i = 0; i < livesBars.Count; i++) {
      livesBars[i].SetActive(livesNum > i);
    }
    for (int i = 0; i < moneyRateBars.Count; i++) {
      moneyRateBars[i].SetActive(moneyRateNum > i);
    }
  }
  #endregion

  #region Store Tab
  [Header("Store Tab")]
  [SerializeField] Image previewImage;
  [SerializeField] TMPro.TMP_Text previewName, previewCost, previewDesc, money;
  public CatalogueItemUI selectedItem;
  public Dictionary<string, ItemSO> catalogueDictionary;
  [SerializeField] List<ItemSO> catalogueRegistry = new List<ItemSO>();
  public Transform cataloguePanel;
  public GameObject catalogueItemPrefab;
  private void InitStoreTab() {
    catalogueDictionary = new Dictionary<string, ItemSO>();
    foreach (ItemSO item in catalogueRegistry) {
      catalogueDictionary.Add(item.itemID, item);
    }

    UpdateMoney();
    PlayfabManager.GetCatalog(OnGetCatalogueSuccess, OnPlayfabError);
  }



  public void PurchaseItem() {
    if (selectedItem == null) return;
    PlayfabManager.PurchaseItem(result => {
      OnSuccess(result);
      UpdateMoney();
    }, OnPlayfabError, selectedItem.m.ItemId, selectedItem.m.VirtualCurrencyPrices["CN"]);
    
  }

  private void UpdateMoney() {
    PlayfabManager.GetVirtualCurrency(result => {
      money.text = "$" + result + " Dabloons";
      OnSuccess("Received money!");
    }, OnPlayfabError);
  }

  public void OnGetCatalogueSuccess(List<CatalogItem> items) {
    foreach (CatalogItem pfItem in items) {
      CatalogueItemUI newItem = Instantiate(catalogueItemPrefab, cataloguePanel).GetComponent<CatalogueItemUI>();

      ItemSO itemSO = (catalogueDictionary.ContainsKey(pfItem.ItemId)) ? catalogueDictionary[pfItem.ItemId] : null;
      newItem.InitItem(this, itemSO, pfItem);
    }
    
  }


  #endregion


  #region Inventory Tab
  [Header("Store Tab")]
  public Transform inventoryScroll;
  [SerializeField] Image inventoryImage;
  [SerializeField] TMPro.TMP_Text inventoryName, inventoryCost, inventoryDesc;

  public void RefreshInventory() {
    PlayfabManager.GetPlayerInventory(
      items => {
        // clear the inventory
        for (int i = inventoryScroll.childCount-1; i >= 0; i--) {
          Destroy(inventoryScroll.GetChild(i).gameObject);
        }

        // put back the inventory
        foreach (ItemInstance pfItem in items) {
          CatalogueItemUI newItem = Instantiate(catalogueItemPrefab, inventoryScroll).GetComponent<CatalogueItemUI>();
          ItemSO itemSO = (catalogueDictionary.ContainsKey(pfItem.ItemId)) ? catalogueDictionary[pfItem.ItemId] : null;
          newItem.InitItem(this, itemSO, pfItem);
        }

        OnSuccess("Inventory received");
    }, OnPlayfabError);
  }

  #endregion


  public void OnSuccess(string message) {
    if (successPanel != null)
      successPanel.DisplayMessage(message, 2);
  }
  public void OnPlayfabError(string message) {
    if (errorPanel != null)
      errorPanel.DisplayMessage(message, 5);
  }

  public void SetSelectedItem(CatalogueItemUI item, CatalogueItemUI.PANEL panelType) {
    if (item == null) {
      Debug.Log("??? item is null");
      return;
    }
    if (panelType == CatalogueItemUI.PANEL.SHOP) {
      selectedItem = item;
      previewName.text = item.m.DisplayName;
      previewCost.text = "$" + item.m.VirtualCurrencyPrices["CN"].ToString() + " Dabloons";
      previewDesc.text = item.m.Description.ToString();
      previewImage.sprite = item.sprite;
    }
    else if (panelType == CatalogueItemUI.PANEL.INVENTORY) {
      inventoryName.text = item.im.DisplayName;
      inventoryDesc.text = item.im.PurchaseDate.ToString();
      inventoryImage.sprite = item.sprite;
    }
  }
}
