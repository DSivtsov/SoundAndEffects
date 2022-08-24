using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Menu;
using System;

public class TopListSectionManager : SectionManager
{
    //[SerializeField] private GameObject _gameObjectResetButton;

    //private Dictionary<SectionName, ISectionControllerAction> SectionActions = new Dictionary<SectionName, ISectionControllerAction>();

    //public void ActivateResetButton(bool activate)
    //{
    //    _gameObjectResetButton.SetActive(activate);
    //}

    //public void ResetSectionValuesToDefault()
    //{
    //    //Debug.Log($"ResetDefault() ActiveSection={GetLastActiveSection.SectionName}");
    //    SectionActions[GetLastActiveSection.SectionName].ResetSectionValuesToDefault();
    //}
    /// <summary>
    /// Link the certain Section to class which realize for that Section the IButtonAction interface
    /// </summary>
    /// <param name="sectionName"></param>
    /// <param name="sectionAction"></param>
    //public void LinkToSectionActions(SectionName sectionName, ISectionControllerAction sectionAction) => SectionActions.Add(sectionName, sectionAction);

}
