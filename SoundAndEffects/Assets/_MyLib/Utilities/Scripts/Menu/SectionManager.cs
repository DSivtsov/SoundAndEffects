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

        private Dictionary<SectionName, SectionObject> _dictSections = new Dictionary<SectionName, SectionObject>();
        private SectionObject _activeSectionObject;
        /// <summary>
        /// Give a possibility the Dirived Class detected the finishing initialization of SectionManager
        /// </summary>
        protected bool _sectionManagerInited = false;

        //The order in which Awake is called from  SectionManager and from SectionObject does not matter, as they are not related to each other
        protected void Awake() => InitDictandSectionObject();

        //Start will call only after all SectionObject will be activated
        protected void Start()
        {
            //Initially all sections must be not active, but coresponded button is Active
            foreach (SectionObject section in _dictSections.Values)
                ActivateSelectedSession(section, false);
            _sectionManagerInited = true;
        }

        private void InitDictandSectionObject()
        {
            foreach (SectionObject section in _sectionsNames.GetComponentsInChildren<SectionObject>(includeInactive: true))
            {
                _dictSections.Add(section.SectionName, section);
                section.LinkToSectionManager(this);
            }
        }

        public void SwitchToSection(SectionName switchedSectionName)
        {
            if (_dictSections.TryGetValue(switchedSectionName, out SectionObject desiredSectionObject))
            {
                if (BeforeSwitchToSectionCallSpecificActions(prevSectionObject: _activeSectionObject, nextSectionObject: desiredSectionObject))
                {
                    DeactivateCurrentSection();
                    ActivateSelectedSession(desiredSectionObject);
                    _activeSectionObject = desiredSectionObject; 
                }
            }
            else
                Debug.LogError($"The desired [{switchedSectionName}] Section was not found!");
        }
        /// <summary>
        /// Derived types can call the additional actions for that specific Sections types which must do at switching of Sections of that types
        /// </summary>
        /// <param name="prevSectionObject"></param>
        /// <param name="nextSectionObject"></param>
        /// <returns>true - if switching aproved</returns>
        protected virtual bool BeforeSwitchToSectionCallSpecificActions(SectionObject prevSectionObject, SectionObject nextSectionObject) => true;

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
