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

    public void SaveToRegistry() => PlayerPrefs.SetString(LocalPlayerName, name);
    public void SetGuestPlayerID(string playerIdentifierLootLocker)
    {
        PlayerPrefs.SetString(LocalLootLockerGuestPlayerID, playerIdentifierLootLocker);
        guestPlayerID = playerIdentifierLootLocker;
    }

    /// <summary>
    /// Remove all information related to the Current Guest Session, but not delete the record on the LootLocker Server
    /// </summary>
    public void DeleteCurrentGuestPlayerID()
    {
        Debug.LogWarning($"DeleteCurrentGuestPlayer() [{guestPlayerID}]");
        guestPlayerID = null;
        PlayerPrefs.DeleteKey(LocalLootLockerGuestPlayerID);
    }

    public void BackUpCurrentPlayerRecord() => Debug.LogError($"Not Realized BackUpCurrentPlayerRecord() [{guestPlayerID}]");

    public static PlayerAccount GetLastPlayerAccount()
    {
        if (PlayerPrefs.HasKey(LocalPlayerName))
        {
            if (PlayerPrefs.HasKey(LocalLootLockerGuestPlayerID))
                return new PlayerAccount(PlayerPrefs.GetString(LocalPlayerName), PlayerPrefs.GetString(LocalLootLockerGuestPlayerID));
            else
                return new PlayerAccount(PlayerPrefs.GetString(LocalPlayerName));
        }
        else
            return null;
    }
}
