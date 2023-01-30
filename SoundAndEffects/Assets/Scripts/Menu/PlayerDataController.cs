using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[Serializable]
public class PlayerAccount
{
    /// <summary>
    /// The Key used for temporary store a PlayerName in Offline mode
    /// </summary>
    private const string LocalPlayerName = "LocalPlayerName";
    /// <summary>
    /// The standart key PlayerPref used by LootLocker
    /// </summary>
    public const string LocalLootLockerGuestPlayerID = "LocalLootLockerGuestPlayerID";
    // Computer\HKEY_CURRENT_USER\SOFTWARE\<DefaultCompany>\SoundAndEffects for Build
    // Computer\HKEY_CURRENT_USER\SOFTWARE\Unity\UnityEditor\<DefaultCompany>\SoundAndEffects for Player in Editor

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

public class PlayerDataController : MonoBehaviour
{
    [SerializeField] TMP_InputField _inputFieldPlayerName;
    [SerializeField] MainMenusSceneManager _mainMenusSceneManager;
    [SerializeField] TurnOffPressEnter _mainMenuCanvasTurnOffPressEnter;

    private const int MaxLenghtPlayerName = 16;

    public PlayerAccount Player { get; private set; }

    public bool ExistGuestPlayerID => Player.GuestPlayerID != null;

    private void Awake()
    {
        ButtonActions.LinkPlayerDataController(this);
    }

    private void OnEnable() => _inputFieldPlayerName.onEndEdit.AddListener(EnteredNewPlayerName);

    private void OnDisable() => _inputFieldPlayerName.onEndEdit.RemoveListener(EnteredNewPlayerName);

    public void InitPlayerAccount()
    {
        Player = PlayerAccount.GetLastPlayerAccount();
        Debug.LogWarning($"{this} GetLastPlayerAccount()[{Player}]");
        UpdateFieldPlayerName();
    }

    public void CreateNewPlayer()
    {
        _mainMenusSceneManager.ActivateButtonStart(false);
        _inputFieldPlayerName.readOnly = false;
        _inputFieldPlayerName.ActivateInputField();
        _mainMenuCanvasTurnOffPressEnter.Active(true, TypeWaitMsg.waitContinue);
    }

    /// <summary>
    /// Check New Player Name and store it
    /// </summary>
    /// <param name="newPlayerName"></param>
    private void EnteredNewPlayerName(string newPlayerName)
    {
        if (newPlayerName.Length == 0)
        {
            Debug.LogWarning($"{this} : PlayerName can't be Empty");
            _inputFieldPlayerName.ActivateInputField();
            return;
        }
        if (newPlayerName.Length > MaxLenghtPlayerName)
        {
            newPlayerName = newPlayerName.Substring(0, MaxLenghtPlayerName);
        }
        Player = new PlayerAccount(newPlayerName);
        Player.SaveToRegistry();
        UpdateFieldPlayerName();
        _mainMenusSceneManager.CreateNewPlayerLootLocker();
        _mainMenuCanvasTurnOffPressEnter.Active(activate: false);
    }

    /// <summary>
    /// Update Player Name and use it for update the objects in the Menu Scene
    /// </summary>
    public void UpdateFieldPlayerName()
    {
        bool existPlayer = Player != null;
        _mainMenusSceneManager.ActivateButtonStart(existPlayer);
        if (existPlayer)
        {
            _inputFieldPlayerName.text = Player.Name;
            _inputFieldPlayerName.readOnly = true;
        }
    }
}
