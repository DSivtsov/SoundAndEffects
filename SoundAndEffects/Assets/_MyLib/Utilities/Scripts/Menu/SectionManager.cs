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
        [SerializeField] private Transform _sectionsNames;
        [SerializeField] private bool _useInitialStartSection = true;
        [SerializeField] private SectionName _initialStartSection;

        private Dictionary<SectionName, SectionObject> _dictSections = new Dictionary<SectionName, SectionObject>();
        private SectionObject _activeSectionObject;

        public SectionObject GetSelectedSection => _activeSectionObject;
        public static SectionManager ActiveSectionManager { get; private set; }

        //InitDictAndSections() must be made only after finishing of all Awake of SectionObject
        //Start will call only after the CanvasOption will be activated
        private void Start()
        {
            InitDictAndSections();
            InitSectionCallActions();
        }

        private void InitDictAndSections()
        {
            foreach (SectionObject section in _sectionsNames.GetComponentsInChildren<SectionObject>(includeInactive: true))
            {
                _dictSections.Add(section.SectionName, section);
                section.LinkToSectionManager(this);
                //Initially all sections must be not active, but coresponded button is Active
                ActivateSelectedSession(section, false);
            }
        }

        //Derived types can call the additional actions for that specific Sections types which must do at Init Sections of that types
        protected virtual void InitSectionCallActions()
        {
            if (_useInitialStartSection)
                SwitchToSection(_initialStartSection);
        }

        public void SetActiveSectionManager() => ActiveSectionManager = this;

        public void SwitchToSection(SectionName switchedSectionName)
        {
            if (_dictSections.TryGetValue(switchedSectionName, out SectionObject desiredSectionObject))
            {
                DeactivateCurrentSection();
                ActivateSelectedSession(desiredSectionObject);
                _activeSectionObject = desiredSectionObject;
                SwitchToSectionCallSpecificActions();
            }
            else
                Debug.LogError($"The desired [{switchedSectionName}] Section was not found!");
        }

        //Derived types can call the additional actions for that specific Sections types which must do at switching of Sections of that types
        protected virtual void SwitchToSectionCallSpecificActions() {}

        private void DeactivateCurrentSection()
        {
            if (_activeSectionObject != null)
                ActivateSelectedSession(_activeSectionObject, false);
        }

        private void ActivateSelectedSession(SectionObject selectedSection, bool activate = true)
        {
            //When the Section is Active the coresponded Button must not active
            selectedSection.SetValueMenuButtonInteractable(!activate);
            selectedSection.SetVisibleSectionBody(activate);
        }
    }
}
