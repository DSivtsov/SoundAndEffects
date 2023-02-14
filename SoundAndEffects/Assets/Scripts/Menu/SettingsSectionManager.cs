using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Menu;
using System;

public class SettingsSectionManager : SectionManager
{
    [Header("InitSectionOption")]
    [SerializeField] private bool _useInitialStartSection = true;
    [SerializeField] private SectionName _initialStartSection; 
    [Header("GameSettingsButton")]
    [SerializeField] private GameObject _loadSaved;
    [SerializeField] private GameObject _loadDef;
    [SerializeField] private GameObject _saveChanges;

    private GameSettingsSOController _gameSettingsSOController;

    protected new void Awake()
    {
        base.Awake();
        CountFrame.DebugLogUpdate(this, "LoadSectionValues()");
        //LinkFieldToElementBase.UpdateElementsValues();
        _gameSettingsSOController = GameSettingsSOController.Instance;
        if (_gameSettingsSOController.GameSettingsInited)
        {
            _gameSettingsSOController.UpdateGameSettingsControlButtons += UpdateGameSettingsControlButtons;
            //initial update after set all flags after load
            UpdateGameSettingsControlButtons();
        }
        else
            Debug.LogError($"{this} : Attempt to use the UpdateGameSettingsControlButtons() before GameSettingsInited==true");
    }

    protected new void Start()
    {
        base.Start();
        if (_useInitialStartSection)
                SwitchToSection(_initialStartSection); 
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
}
