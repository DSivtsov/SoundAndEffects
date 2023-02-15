using UnityEngine;
using System;

[Serializable]
public class PlayerAccount
{
    /// <summary>
    /// The Key used for store in registry a PlayerName
    /// </summary>
    private const string LocalPlayerName = "LocalPlayerName";
    /// <summary>
    /// The Key used for store a LootLockerGuestPlayerID
    /// </summary>
    public const string LocalLootLockerGuestPlayerID = "LocalLootLockerGuestPlayerID";
    /*
    * Doesn't used the LootLockerGuestPlayerID - the standart key in registry used by LootLocker
    * Computer\HKEY_CURRENT_USER\SOFTWARE\<DefaultCompany>\SoundAndEffects (for Build)
    * Computer\HKEY_CURRENT_USER\SOFTWARE\Unity\UnityEditor\<DefaultCompany>\SoundAndEffects (for Editor)
    */
    [SerializeField] private string name;
    [SerializeField] private string guestPlayerID;

    public PlayerAccount(string playerName) : this(playerName, null) { }

    public PlayerAccount(string playerName, string guestPlayerID)
    {
        this.name = playerName;
        this.guestPlayerID = guestPlayerID;
    }

    public string Name => name;
    public string GuestPlayerID => guestPlayerID;

    public override string ToString() => $"[{name}]-[{guestPlayerID}]";

    public void SaveToRegistry()
    {
        CountFrame.DebugLogUpdate($"PlayerAccount: SaveToRegistry()[{this}]");
        PlayerPrefs.SetString(LocalPlayerName, name);
        PlayerPrefs.SetString(LocalLootLockerGuestPlayerID, guestPlayerID);
    }

    public void SetGuestPlayerID(string playerIdentifierLootLocker) => guestPlayerID = playerIdentifierLootLocker;

    /// <summary>
    /// Remove all information related to the Current Guest Session, but not delete the record on the LootLocker Server
    /// </summary>
    public void SetToNullGuestPlayerID()
    {
        CountFrame.DebugLogUpdate($"PlayerAccount: DeleteCurrentGuestPlayerID()[{PlayerPrefs.GetString("LootLockerGuestPlayerID", "")}]");
        guestPlayerID = null;
    }

    public static PlayerAccount GetLastPlayerAccount()
    {
        string localPlayerName;
        string localLootLockerGuestPlayerID;
        if (PlayerPrefs.HasKey(LocalPlayerName) && (localPlayerName = PlayerPrefs.GetString(LocalPlayerName)).Length != 0)
        {
            if (PlayerPrefs.HasKey(LocalLootLockerGuestPlayerID) && (localLootLockerGuestPlayerID = PlayerPrefs.GetString(LocalLootLockerGuestPlayerID)).Length != 0)
                return new PlayerAccount(localPlayerName, localLootLockerGuestPlayerID);
            else
                return new PlayerAccount(localPlayerName);
        }
        else
            return null;
    }
}
