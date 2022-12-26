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

        //protected Dictionary<(SectionName, Type), ISectionControllerAction> SectionControllers = new Dictionary<(SectionName, Type), ISectionControllerAction>();

        public static SectionManager ActiveSectionManager { get; private set; }

        //public static ISectionControllerAction SectionController { get; private set; }

        private SectionObject _selectedSection;
        public SectionObject GetSelectedSection => _selectedSection;
        private Dictionary<SectionName, SectionObject> _dictSections = new Dictionary<SectionName, SectionObject>();

        //Initialization of dictSections must be made only after finishing of all Awakes of SectionObject
        //Start will call only after the CanvasOption will be activated
        private void Start()
        {
            foreach (SectionObject section in _sectionsNames.GetComponentsInChildren<SectionObject>(includeInactive: true))
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

        public void SetActiveSectionManager() => ActiveSectionManager = this;

        public void SwitchSection(SectionName selectedSection)
        {
            if (_selectedSection != null)
                ActivateSelectedSession(_selectedSection, false);
            SectionObject desiredSection = _dictSections[selectedSection];
            if (desiredSection != null)
            {
                ActivateSelectedSession(desiredSection);
                _selectedSection = desiredSection;
                if (!(GetSelectedSection.SectionName == SectionName.Global || GetSelectedSection.SectionName == SectionName.Local))
                {
                    //SectionController = SectionControllers[(GetSelectedSection.SectionName, ActiveSectionManager.GetType())];
                    Debug.Log($"LoadValuesSelectedSession()");
                    //SectionController.LoadSectionValues(); 
                }
                else
                {
                    Debug.LogError($"Temporary blocked : GetSelectedSection.SectionName={GetSelectedSection.SectionName}" +
                        " GetSelectedSection.SectionName == SectionName.Global || GetSelectedSection.SectionName == SectionName.Local"); 
                }
            }
            else
                Debug.LogWarning($"The desired [{selectedSection}] Section was not found!");
        }

        private void ActivateSelectedSession(SectionObject selectedSection, bool activate = true)
        {
            //When the Section is Active the coresponded Button must not active
            selectedSection.SetValueMenuButtonInteractable(!activate);
            selectedSection.SetVisibleSectionBody(activate);
        }

        public void LinkToSectionActions(SectionName sectionName, ISectionControllerAction sectionController)
        {
            //SettingsSectionControllers.Add(sectionName, sectionController);
            //SectionControllers.Add((sectionName, typeof(SettingsSectionManager)), sectionController);
        }

        //private void LoadValuesSelectedSession()
        //{
        //    Debug.Log($"LoadValuesSelectedSession()");
        //    SectionControllers[(GetSelectedSection.SectionName, ActiveSectionManager.GetType())].LoadSectionValues();
        //}

        //public void ResetSectionValuesToDefault()
        //{
        //    //SettingsSectionControllers[GetLastActiveSection.SectionName].ResetSectionValuesToDefault();
        //    SectionControllers[(GetSelectedSection.SectionName, ActiveSectionManager.GetType())].ResetSectionValuesToDefault();
        //}
    }
}
