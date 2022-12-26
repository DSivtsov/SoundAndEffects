using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Menu;
using System;

public class SettingsSectionManager : SectionManager
{
    //[SerializeField] private GameObject _gameObjectResetButton;
    //[SerializeField] private GameSettingsSO _gameSettingsSO;
    [Header("GameSettingsButton")]
    [SerializeField] private GameObject _loadSaved;
    [SerializeField] private GameObject _loadDef;
    [SerializeField] private GameObject _saveChanges;

    private GameSettingsSOController _gameSettingsSOController;
    /// <summary>
    /// Dictionary the SectionControllers which support the ISectionAction
    /// </summary>
    //private Dictionary<SectionName, ISectionControllerAction> SettingsSectionControllers = new Dictionary<SectionName, ISectionControllerAction>();

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

    //public void ActivateResetButton(bool activate)
    //{
    //    //_gameObjectResetButton.SetActive(activate);
    //}

    //public void ResetSectionValuesToDefault()
    //{
    //    //Debug.Log($"ResetDefault() ActiveSection={GetLastActiveSection.SectionName}");
    //    SettingsSectionControllers[GetLastActiveSection.SectionName].ResetSectionValuesToDefault();
    //}
    /// <summary>
    /// Link the certain Section to class which realize for that Section the IButtonAction interface
    /// </summary>
    /// <param name="sectionName"></param>
    /// <param name="sectionController"></param>
    //public new void LinkToSectionActions(SectionName sectionName, ISectionControllerAction sectionController)
    //{
    //    //SettingsSectionControllers.Add(sectionName, sectionController);
    //    //SectionControllers.Add((sectionName, typeof(SettingsSectionManager)), sectionController);
    //}
}
