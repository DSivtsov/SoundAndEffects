using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Menu;
using System;

public class TopListSectionManager : SectionManager
{
    [Header("TopListParameters")]
    [SerializeField] private MainMenusSceneManager _mainMenusSceneManager;
    [Tooltip("Value from GameSetting will override the InitialStartSection parameters upper")]
    [SerializeField] private GameSettingsSO _gameSettings;

    protected void OnEnable()
    {
        if (_sectionManagerInited)
            SelectInitialSectionToShowBasedOnGameSettings();
    }

    protected new void Start()
    {
        base.Start();
        SelectInitialSectionToShowBasedOnGameSettings();
    }

    private void SelectInitialSectionToShowBasedOnGameSettings()
    {
        if (_mainMenusSceneManager.IsConnectedToServer && _gameSettings.FieldByDefaultShowGlobalTopList.GetCurrentValue())
            SwitchToSection(SectionName.Global);
        else
            SwitchToSection(SectionName.Local);

    }

    protected override bool BeforeSwitchToSectionCallSpecificActions(SectionObject prevSectionObject, SectionObject nextSectionObject)
    {
        if (nextSectionObject.SectionName == SectionName.Global)
        {
            if (_mainMenusSceneManager.IsConnectedToServer)
            {
                TopListSectionObject topListSectionObject = (TopListSectionObject)nextSectionObject;
                if (topListSectionObject != null)
                {
                    topListSectionObject.SectionTopListController.LoadAndShow();
                    return true;
                }
                else
                    Debug.LogError($"{this}: Wrong Type of Section [{nextSectionObject.SectionName}]"); 
            }
            //else
            //    Debug.LogError($"{this}: _mainMenusSceneManager.IsConnectedToServer [{_mainMenusSceneManager.IsConnectedToServer}]");
            //by default switching to SectionName.Global in case of error always is forbad
            return false;
        }
        //by default switching to SectionName.Local always aproved
        return true;
    }
}
