using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerDataController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputFieldPlayerName;
    [SerializeField] private GameObject _inputFieldNoteLenght;
    [SerializeField] private GameObject _menuMessagesContinue;

    private const int MinNameLenght = 8;
    private const int MaxLenghtPlayerName = 16;

    private readonly string GetInputFieldNoteLenght = $"(min {MinNameLenght} max {MaxLenghtPlayerName} symbols)";

    public PlayerAccount Player { get; private set; }
    public PlayerAccount PlayerPrevious { get; private set; }

    public bool PlayerAccountInited => Player != null;

    public bool ExistGuestPlayerID => Player.GuestPlayerID != null;

    private Action _actionAfterEnteredNewPlayerName;

    private void Awake()
    {
        _inputFieldNoteLenght.GetComponent<TextMeshProUGUI>().text = GetInputFieldNoteLenght;
        _inputFieldPlayerName.onEndEdit.AddListener((string newPlayerName) =>
            {
                if (newPlayerName.Length < MinNameLenght)
                {
                    Debug.LogWarning($"{this} : PlayerName can't less than {MinNameLenght} symbols");
                    _inputFieldPlayerName.ActivateInputField();
                    return;
                }
                if (newPlayerName.Length > MaxLenghtPlayerName)
                {
                    newPlayerName = newPlayerName.Substring(0, MaxLenghtPlayerName);
                }
                EnteredNameNewPlayerAccount(newPlayerName);
            });
        ActivateInputFieldAndInfoMessage(activate: false);
    }

    public void LoadLastPlayerAccount()
    {
        Player = PlayerAccount.GetLastPlayerAccount();
        CountFrame.DebugLogUpdate(this,$"GetLastPlayerAccount()[{Player}]");
        UpdateFieldPlayerName();
    }

    /// <summary>
    /// Activate _inputFieldPlayerName
    /// </summary>
    /// <param name="actionAfterCreateNewPlayerAccount"></param>
    public void CreateNewPlayerAccount(Action actionAfterCreateNewPlayerAccount)
    {
        _actionAfterEnteredNewPlayerName = actionAfterCreateNewPlayerAccount;
        ActivateInputFieldAndInfoMessage();
    }

    private void ActivateInputFieldAndInfoMessage(bool activate = true)
    {
        if (activate)
        {
            _inputFieldNoteLenght.SetActive(true);
            _menuMessagesContinue.SetActive(true);
            _inputFieldPlayerName.readOnly = false;
            _inputFieldPlayerName.ActivateInputField(); 
        }
        else
        {
            _inputFieldPlayerName.DeactivateInputField();
            _inputFieldPlayerName.readOnly = true;
            _menuMessagesContinue.SetActive(false);
            _inputFieldNoteLenght.SetActive(false);
        }
    }

    private void EnteredNameNewPlayerAccount(string newCheckedPlayerName)
    {
        PlayerPrevious = Player;
        BackUpCurrentPlayerRecord(PlayerPrevious);
        Player = new PlayerAccount(newCheckedPlayerName);
        UpdateFieldPlayerName();
        ActivateInputFieldAndInfoMessage(false);
        _actionAfterEnteredNewPlayerName();
    }

    public void UpdateFieldPlayerName()
    {
        if (Player != null)
        {
            _inputFieldPlayerName.text = Player.Name;
        }
    }

    public void BackUpCurrentPlayerRecord(PlayerAccount playerPrevious) => Debug.LogError($"Not Realized BackUpCurrentPlayerRecord() [{playerPrevious}]");

    public void RestorePreviousPlayer() => Player = PlayerPrevious;
}
