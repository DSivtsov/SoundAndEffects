using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Menu;
using System;

public class TopListSectionManager : SectionManager
{
    [Header("TopListParameters")]
    [Tooltip("Value from GameSetting will override the InitialStartSection parameters upper")]
    [SerializeField] private GameSettingsSO _gameSettings;

    protected override void InitSectionCallActions()
    {
        if (_gameSettings)
        {
            if (_gameSettings.FieldByDefaultShowGlobalTopList.GetCurrentValue())
                SwitchToSection(SectionName.Global);
            else
                SwitchToSection(SectionName.Local);
        }
        else
            base.InitSectionCallActions();
    }
}
