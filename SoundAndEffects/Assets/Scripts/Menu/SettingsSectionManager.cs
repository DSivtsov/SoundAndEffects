using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Menu;
using System;

public class SettingsSectionManager : SectionManager
{
    [SerializeField] private GameObject _gameObjectResetButton;
    /// <summary>
    /// Dictionary the SectionControllers which support the ISectionAction
    /// </summary>
    private Dictionary<SectionName, ISectionControllerAction> SettingsSectionControllers = new Dictionary<SectionName, ISectionControllerAction>();

    public void ActivateResetButton(bool activate)
    {
        _gameObjectResetButton.SetActive(activate);
    }

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
    public new void LinkToSectionActions(SectionName sectionName, ISectionControllerAction sectionController)
    {
        SettingsSectionControllers.Add(sectionName, sectionController);
        SectionControllers.Add((sectionName, typeof(SettingsSectionManager)), sectionController);
    }
}
