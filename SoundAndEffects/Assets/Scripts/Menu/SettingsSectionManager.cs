using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Menu;
using System;

public class SettingsSectionManager : SectionManager
{
    [Header("GameSettingsButton")]
    [SerializeField] private GameObject _loadSaved;
    [SerializeField] private GameObject _loadDef;
    [SerializeField] private GameObject _saveChanges;

    private GameSettingsSOController _gameSettingsSOController;

    private void Awake()
    {
        _gameSettingsSOController = GameSettingsSOController.Instance;
        if (_gameSettingsSOController.GameSettingsInited)
        {
            //initial update after set all flags after load
            UpdateGameSettingsControlButtons();
        }
        else
            Debug.LogError($"{this} : Attempt to UpdateGameSettingsControlButtons() before GameSettingsInited==true");

    }

    private void OnEnable()
    {
        _gameSettingsSOController.UpdateGameSettingsControlButtons += UpdateGameSettingsControlButtons;
    }

    private void OnDisable()
    {
        _gameSettingsSOController.UpdateGameSettingsControlButtons -= UpdateGameSettingsControlButtons;
    }

    private void UpdateGameSettingsControlButtons()
    {
        if (_gameSettingsSOController.ExistNotSavedChanges)
        {
            _loadDef.SetActive(true);
            _saveChanges.SetActive(true);
            if (_gameSettingsSOController.ExistCustomSavedSettings)
                _loadSaved.SetActive(true); 
            else
                _loadSaved.SetActive(false);
        }
        else
        {
            _saveChanges.SetActive(false);
            _loadSaved.SetActive(false);
            if (_gameSettingsSOController.ExistCustomSavedSettings)
                _loadDef.SetActive(true);
            else
                _loadDef.SetActive(false);
        }
    }

    //protected override void CallSpecificSectionsActions()
    //{
    //    base.CallSpecificSectionsActions();
    //}
}
