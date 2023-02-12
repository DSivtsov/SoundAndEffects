using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerDataController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputFieldPlayerName;
    [SerializeField] private MainMenusSceneManager _mainMenusSceneManager;
    [SerializeField] private GameObject _menuMessagesContinue;

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
        CountFrame.DebugLogUpdate(this,$"GetLastPlayerAccount()[{Player}]");
        UpdateFieldPlayerName();
    }

    public void CreateNewPlayer()
    {
        _mainMenusSceneManager.ActivateButtonStart(false);
        _inputFieldPlayerName.readOnly = false;
        _inputFieldPlayerName.ActivateInputField();
        _menuMessagesContinue.SetActive(true);
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
        _menuMessagesContinue.SetActive(false);
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
