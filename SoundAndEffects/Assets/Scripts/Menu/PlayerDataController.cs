using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayerDataController : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputFieldPlayerName;
    [SerializeField] private TextMeshProUGUI _txtInputFieldNoteLenght;
    [SerializeField] private TextMeshProUGUI _menuMessagesContinue;

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
        _txtInputFieldNoteLenght.text = GetInputFieldNoteLenght;
        _inputFieldPlayerName.onEndEdit.AddListener(CheckNewPlayerName);
        ActivateInputFieldAndInfoMessage(activate: false);
    }

    private void CheckNewPlayerName(string newPlayerName)
    {
        if (newPlayerName.Length<MinNameLenght)
        {
            Debug.LogWarning($"{this} : PlayerName can't less than {MinNameLenght} symbols");
            _txtInputFieldNoteLenght.color = Color.red;
            _inputFieldPlayerName.ActivateInputField();
            return;
        }
        else
            _txtInputFieldNoteLenght.color = Color.green;
        if (newPlayerName.Length > MaxLenghtPlayerName)
        {
            newPlayerName = newPlayerName.Substring(0, MaxLenghtPlayerName);
        }
        EnteredNameNewPlayerAccount(newPlayerName);
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
        _inputFieldPlayerName.text = null;
        ActivateInputFieldAndInfoMessage();
    }

    private void ActivateInputFieldAndInfoMessage(bool activate = true)
    {
        if (activate)
        {
            //_txtInputFieldNoteLenght.gameObject.SetActive(true);
            _menuMessagesContinue.gameObject.SetActive(true);
            _inputFieldPlayerName.readOnly = false;
            _inputFieldPlayerName.ActivateInputField(); 
        }
        else
        {
            _inputFieldPlayerName.DeactivateInputField();
            _inputFieldPlayerName.readOnly = true;
            _menuMessagesContinue.gameObject.SetActive(false);
            //_txtInputFieldNoteLenght.gameObject.SetActive(false);
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
