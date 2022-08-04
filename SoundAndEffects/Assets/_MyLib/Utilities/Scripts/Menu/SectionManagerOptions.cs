using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Menu;
using System;

public class SectionManagerOptions : SectionManager
{
    [SerializeField] private GameObject _gameObjectResetButton;

    private Dictionary<SectionName,IButtonAction> buttonActions = new Dictionary<SectionName, IButtonAction>();

    public void ActivateResetButton(bool activate)
    {
        _gameObjectResetButton.SetActive(activate);
    }

    public void ResetDefault()
    {
        //Debug.Log($"ResetDefault() ActiveSection={GetLastActiveSection.SectionName}");
        buttonActions[GetLastActiveSection.SectionName].ResetDefault();
    }
    /// <summary>
    /// Link the certain Section to class which realize for that Section the IButtonAction interface
    /// </summary>
    /// <param name="sectionName"></param>
    /// <param name="buttonAction"></param>
    public void LinkToButtonActions(SectionName sectionName, IButtonAction buttonAction) => buttonActions.Add(sectionName, buttonAction);

}
