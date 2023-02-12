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

    public bool ExistGuestPlayerID => Player.GuestPlayerID != null;

    private Action ActionAfterEnteredNewPlayerName;

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
                EnteredRightNewPlayerName(newPlayerName);
            });
        ActivateInputFieldAndInfoMessage(activate: false);
    }

    /// <summary>
    /// Init Player Account
    /// </summary>
    /// <returns>true if PlayerAccount inited</returns>
    public bool InitPlayerAccount()
    {
        Player = PlayerAccount.GetLastPlayerAccount();
        CountFrame.DebugLogUpdate(this,$"GetLastPlayerAccount()[{Player}]");
        UpdateFieldPlayerName();
        return Player != null;
    }

    /// <summary>
    /// Activate _inputFieldPlayerName
    /// </summary>
    /// <param name="action"></param>
    public void ActivateCreateNewPlayer(Action action)
    {
        ActionAfterEnteredNewPlayerName = action;
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

    private void EnteredRightNewPlayerName(string newPlayerName)
    {
        Player = new PlayerAccount(newPlayerName);
        Player.SaveToRegistry();
        UpdateFieldPlayerName();
        ActivateInputFieldAndInfoMessage(false);
        ActionAfterEnteredNewPlayerName();
    }

    public void UpdateFieldPlayerName()
    {
        if (Player != null)
        {
            _inputFieldPlayerName.text = Player.Name;
        }
        //_inputFieldPlayerName.readOnly = true;
    }
}
