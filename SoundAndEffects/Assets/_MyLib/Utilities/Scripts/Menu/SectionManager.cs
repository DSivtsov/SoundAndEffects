using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GMTools.Menu
{
    //Can be used multi times in Project, every SectionManager will be have own environment, related Sections are linked through the LinkToSectionManager(this)
    public class SectionManager : MonoBehaviour
    {
        [SerializeField] private Transform _groupsSections;
        [SerializeField] private bool _useInitialStartSection = true;
        [SerializeField] private SectionName _initialStartSection;
        
        private SectionObject _lastActiveSection;
        private Dictionary<SectionName, SectionObject> _dictSections = new Dictionary<SectionName, SectionObject>();

        //Initialization of dictSections must be made only after finishing of all Awakes of SectionObject
        //Start will call only after the CanvasOption will be activated
        private void Start()
        {
            //Debug.Log("SectionManager : Start()");
            //ButtonActions.InitButtonActions(this);
            foreach (SectionObject section in _groupsSections.GetComponentsInChildren<SectionObject>(includeInactive: true))
            {
                //Debug.Log(section.name);
                _dictSections.Add(section.SectionName, section);
                section.LinkToSectionManager(this);
                //Initially all sections must be not active, but coresponded button is Active
                ActivateSelectedSession(section, false);
            }
            if (_useInitialStartSection)
                SwitchSection(_initialStartSection);
        }

        public void SwitchSection(SectionName selectedSection)
        {
            if (_lastActiveSection != null)
                ActivateSelectedSession(_lastActiveSection, false);
            SectionObject desiredSection = _dictSections[selectedSection];
            if (desiredSection != null)
            {
                ActivateSelectedSession(desiredSection); ;
                _lastActiveSection = desiredSection;
            }
            else
                Debug.LogWarning($"The desired [{selectedSection}] Section was not found!");
        }

        private static void ActivateSelectedSession(SectionObject selectedSection, bool activate = true)
        {
            //When the Section is Active the coresponded Button must not active
            selectedSection.SetValueMenuButtonInteractable(!activate);
            selectedSection.SetVisibleSectionBody(activate);
        }
    }

}
