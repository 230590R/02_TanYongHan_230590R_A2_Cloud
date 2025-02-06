using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.ClientModels;


public class CatalogueItemUI : MonoBehaviour {
  public HangarUI hangarUIManager;
  public Image previewImage;
  public ItemSO itemSO;
  public TMPro.TMP_Text previewName, previewCost;
  //public CatalogueItemData m;
  public CatalogItem m;
  public ItemInstance im;

  public Sprite defaultSprite;
  public Sprite sprite;
  public enum PANEL {
    SHOP, INVENTORY
  }
  public PANEL panelType = PANEL.SHOP;


  public void InitItem(HangarUI hangarUI, ItemSO itemSO, CatalogItem catalogueItemData) {
    m = catalogueItemData;
    im = null;
    panelType = PANEL.SHOP;
    this.hangarUIManager = hangarUI;
    if (itemSO != null && itemSO.icon != null)
      sprite = itemSO.icon;
    else
      sprite = defaultSprite;
    RefreshUI();
  }

  public void InitItem(HangarUI hangarUI, ItemSO itemSO, ItemInstance catalogueItemData) {
    im = catalogueItemData;
    m = null;
    panelType = PANEL.INVENTORY;
    this.hangarUIManager = hangarUI;
    if (itemSO != null && itemSO.icon != null)
      sprite = itemSO.icon;
    else
      sprite = defaultSprite;
    RefreshUI();
  }

  private void RefreshUI() {
    if (m != null) {
      previewName.text = m.DisplayName;
      previewCost.text = m.VirtualCurrencyPrices["CN"].ToString();
      previewImage.sprite = sprite;
    }
    else if (im != null) {
      previewName.text = im.DisplayName;
      previewCost.text = " ";
      previewImage.sprite = sprite;
    }
  }

  void Start() {

  }

  public void OnSelect() {
    hangarUIManager.SetSelectedItem(this, panelType);
  }
}
