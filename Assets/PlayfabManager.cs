using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShipUpgrade {
  public string name;
  public int level;
  public ShipUpgrade(string name, int level) { this.name = name; this.level = level; }
}

[Serializable]
public class JSListWrapper<T> {
  public List<T> list;
  public JSListWrapper(List<T> list) => this.list = list;

}
public class PlayfabManager : MonoBehaviour {
  public delegate void OnSuccessString(string str);
  public delegate void OnSuccessInt(int value);
  public delegate void OnErrorString(string str);
  public delegate void OnSuccessLeaderboard(List<LBEntry.LeaderboardEntry> entries);
  public delegate void OnSuccessUpgrades(List<ShipUpgrade> entries);
  public delegate void OnSuccessItems(List<CatalogItem> entries);
  public delegate void OnSuccessItemInstance(List<ItemInstance> entries);

  public static void LoginCustomID(OnSuccessString OnSuccess, OnErrorString OnError) {
    var loginRequest = new LoginWithCustomIDRequest {
      CreateAccount = true,
      CustomId = "PlayfabTest"
    };

    PlayFabClientAPI.LoginWithCustomID(loginRequest,
      result => {
        string message = "Your id: " + result.PlayFabId;
        OnSuccess(message);
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }

  public static void LoginEmail(OnSuccessString OnSuccess, OnErrorString OnError, string email, string password) {
    var loginRequest = new LoginWithEmailAddressRequest {
      Email = email,
      Password = password
    };

    PlayFabClientAPI.LoginWithEmailAddress(loginRequest,
      result => {
        string message = "Your id: " + result.PlayFabId;
        OnSuccess(message);
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }

  public static void LoginUsername(OnSuccessString OnSuccess, OnErrorString OnError, string username, string password) {
    var loginRequest = new LoginWithPlayFabRequest {
      Username = username,
      Password = password
    };

    PlayFabClientAPI.LoginWithPlayFab(loginRequest,
      result => {
        string message = "Your id: " + result.PlayFabId;
        OnSuccess(message);
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }

  public static void Register(OnSuccessString OnSuccess, OnErrorString OnError, string displayname, string username, string email, string password) {
    var loginRequest = new RegisterPlayFabUserRequest {
      DisplayName = displayname,
      Username = username,
      Email = email,
      Password = password
    };

    PlayFabClientAPI.RegisterPlayFabUser(loginRequest,
      result => {
        string message = "Your id: " + result.PlayFabId;
        OnSuccess(message);
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }

  public static void ResetPassword(OnSuccessString OnSuccess, OnErrorString OnError, string email) {
    var request = new SendAccountRecoveryEmailRequest {
      Email = email
    };


    PlayFabClientAPI.SendAccountRecoveryEmail(request,
      result => {
        string message = "Successfully sent recovery email! Check your email.";
        OnSuccess(message);
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }

  public static void Logout(OnSuccessString OnSuccess, OnErrorString OnError) {
    PlayFabClientAPI.ForgetAllCredentials();
    OnSuccess("Logged out!");
  }


  public static void GetMOTD(OnSuccessString OnSuccess, OnErrorString OnError) {
    try {
      PlayFabClientAPI.GetTitleData(new GetTitleDataRequest(),
        result => {
          if (result.Data == null || !result.Data.ContainsKey("MOTD"))
            OnError("MOTD not found!");
          else
            OnSuccess(result.Data["MOTD"]);
        },
        error => {
          OnError(ParsePlayfabError(error));
        });
    }
    catch (PlayFabException e) {
      OnError(e.ToString());
    }

  }


  public static void SubmitLeaderboard(OnSuccessString OnSuccess, OnErrorString OnError, int score) {
    var request = new UpdatePlayerStatisticsRequest {
      Statistics = new List<StatisticUpdate> {
        new StatisticUpdate { StatisticName = "Highscore", Value = score }
      }
    };
    PlayFabClientAPI.UpdatePlayerStatistics(request,
      result => {
        OnSuccess("Successfully added your score!");
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }

  public static void GetLocalLeaderboard(OnSuccessLeaderboard OnSuccess, OnErrorString OnError) {
    var request = new GetLeaderboardAroundPlayerRequest {
      StatisticName = "Highscore",
      MaxResultsCount = 10,
    };
    PlayFabClientAPI.GetLeaderboardAroundPlayer(request,
      result => {
        List<LBEntry.LeaderboardEntry> entries = new List<LBEntry.LeaderboardEntry>();
        foreach (var item in result.Leaderboard) {
          string name = item.DisplayName;
          if (name == null) name = item.PlayFabId;
          entries.Add(new LBEntry.LeaderboardEntry(item.Position, name, item.StatValue));
        }
        OnSuccess(entries);
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }
  public static void GetGlobalLeaderboard(OnSuccessLeaderboard OnSuccess, OnErrorString OnError) {
    var request = new GetLeaderboardRequest {
      StatisticName = "Highscore",
      StartPosition = 0,
      MaxResultsCount = 10,
    };
    PlayFabClientAPI.GetLeaderboard(request,
      result => {
        List<LBEntry.LeaderboardEntry> entries = new List<LBEntry.LeaderboardEntry>();
        foreach (var item in result.Leaderboard) {
          string name = item.DisplayName;
          if (name == null) name = item.PlayFabId;
          entries.Add(new LBEntry.LeaderboardEntry(item.Position, name, item.StatValue));
        }
        OnSuccess(entries);
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }



  private static string ParsePlayfabError(PlayFabError error) {
    string output = error.GenerateErrorReport();
    return output;
  }


  public static void SetUserData(OnSuccessString OnSuccess, OnErrorString OnError, string dataName, string value) {
    var request = new UpdateUserDataRequest() {
      Data = new Dictionary<string, string>() {
        {dataName, value}
      }
    };


    PlayFabClientAPI.UpdateUserData(request,
      result => {
        OnSuccess("Successfully updated data");
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }


  public static void GetUserData(OnSuccessString OnSuccess, OnErrorString OnError, string dataName) {
    var request = new GetUserDataRequest() { };
    PlayFabClientAPI.GetUserData(request,
      result => {
        if (result.Data == null || !result.Data.ContainsKey(dataName))
          OnError("Could not find user data " + dataName);
        else {
          OnSuccess(result.Data[dataName].Value);
        }
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }


  public static void SetJSONData(OnSuccessString OnSuccess, OnErrorString OnError, List<ShipUpgrade> upgrades, string dataName) {
    string jsonData = JsonUtility.ToJson(new JSListWrapper<ShipUpgrade>(upgrades));
    Debug.Log(jsonData);
    var request = new UpdateUserDataRequest() {
      Data = new Dictionary<string, string>() { { dataName, jsonData } }
    };

    PlayFabClientAPI.UpdateUserData(request,
      result => {
        OnSuccess("Successfully updated json data");
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }

  public static void GetJSONData(OnSuccessUpgrades OnSuccess, OnErrorString OnError, string dataName) {
    var request = new GetUserDataRequest() { };
    PlayFabClientAPI.GetUserData(request,
      result => {
        if (result.Data == null || !result.Data.ContainsKey(dataName))
          OnError("Could not find user data " + dataName);
        else {
          string jsonData = result.Data[dataName].Value;
          //OnSuccess(result.Data[dataName].Value);
          Debug.Log("Received: " + jsonData);
          JSListWrapper<ShipUpgrade> listWrapper = JsonUtility.FromJson<JSListWrapper<ShipUpgrade>>(jsonData);
          OnSuccess(listWrapper.list);
        }
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }

  public static void GetVirtualCurrency(OnSuccessInt OnSuccess, OnErrorString OnError) {
    var request = new GetUserInventoryRequest();
    PlayFabClientAPI.GetUserInventory(request,
      result => {
        int currency = result.VirtualCurrency["CN"];
        OnSuccess(currency);
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }

  public static void AddVirtualCurrency(OnSuccessString OnSuccess, OnErrorString OnError, int amount) {
    var request = new AddUserVirtualCurrencyRequest() {
      Amount = amount,
      VirtualCurrency = "CN"
    };
    PlayFabClientAPI.AddUserVirtualCurrency(request,
      result => {
        OnSuccess("Currency added! New balance: " + result.Balance.ToString());
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }


  public static void GetCatalog(OnSuccessItems OnSuccess, OnErrorString OnError) {
    var request = new GetCatalogItemsRequest {
      CatalogVersion = "Weapons"
    };
    PlayFabClientAPI.GetCatalogItems(request,
      result => {
        List<CatalogItem> items = result.Catalog;
        OnSuccess(items); 
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }

  public static void PurchaseItem(OnSuccessString OnSuccess, OnErrorString OnError, string itemID, uint price) {
    var request = new PurchaseItemRequest {
      CatalogVersion = "Weapons",
      ItemId = itemID,
      VirtualCurrency = "CN",
      Price = (int)price
    };

    PlayFabClientAPI.PurchaseItem(request,
      result => {
        OnSuccess("Purchase successful!");
      },
      error => {
        OnError(ParsePlayfabError(error));
      });
  }

  public static void GetPlayerInventory(OnSuccessItemInstance OnSuccess, OnErrorString OnError) {
    var request = new GetUserInventoryRequest();
    PlayFabClientAPI.GetUserInventory(request,
      result => {
        OnSuccess(result.Inventory);
      },
      error => { OnError(ParsePlayfabError(error)); });
  }
}

