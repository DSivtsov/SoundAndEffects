using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerDataController : MonoBehaviour
{
    [SerializeField] TMP_InputField _inputFieldPlayerName;
    [SerializeField] MainMenusSceneManager _mainMenusSceneManager;
    [SerializeField] TurnOffPressEnter _mainMenuCanvasTurnOffPressEnter;
    /// <summary>
    /// The Key used for temporary store a PlayerName in Offline mode
    /// </summary>
    private const string _localCopyPlayerName = "LocalCopyPlayerName";
    private const int MaxLenghtPlayerName = 16;
    private string _playerName;

    private void Awake()
    {
        ButtonActions.LinkPlayerDataController(this);
    }

    public string PlayerName
    {
        get => _playerName;
        private set
        {
            if (value.Length > MaxLenghtPlayerName)
            {
                value = value.Substring(0, MaxLenghtPlayerName);
            }
            _playerName = value;
            _inputFieldPlayerName.text = value;
            _inputFieldPlayerName.readOnly = true;
            _mainMenusSceneManager.ActivateButtonStart(true);
        }
    }

    public void CreateNewPlayer()
    {
        Debug.Log("CreateNewPlayer()");
        _mainMenusSceneManager.ActivateButtonStart(false);
        _inputFieldPlayerName.readOnly = false;
        _inputFieldPlayerName.ActivateInputField();
        _mainMenuCanvasTurnOffPressEnter.Active(true, TypeWaitMsg.waitContinue);
        _inputFieldPlayerName.onEndEdit.AddListener(EnteredNewPlayerName);
    }

    private void SetDemoData()
    {
        PlayerName = "";
        Debug.Log($"_inputFieldPlayerName=[{_inputFieldPlayerName.text}] == null is {_inputFieldPlayerName.text == null} but Lenght={_inputFieldPlayerName.text.Length}");
    }

    private void EnteredNewPlayerName(string newPlayerName)
    {
        if (newPlayerName.Length == 0)
        {
            Debug.LogWarning($"{this} : PlayerName can't be Empty");
            _inputFieldPlayerName.ActivateInputField();
            return;
        }
        PlayerName = newPlayerName;
        //Will  Save to PlayerPref directly for case playing Offlline
        PlayerPrefs.SetString(_localCopyPlayerName, PlayerName);
        _mainMenusSceneManager.CreateNewPlayerLootLocker(PlayerName);
        _mainMenuCanvasTurnOffPressEnter.Active(activate: false);
    }

    public void SetPlayerName(string playerNameLootLocker) => PlayerName = playerNameLootLocker;
}
