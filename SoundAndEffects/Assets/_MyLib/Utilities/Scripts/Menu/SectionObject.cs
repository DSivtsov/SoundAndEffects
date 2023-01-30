using System;
using UnityEngine;
using UnityEngine.UI;

namespace GMTools.Menu
{
    public class SectionObject : MonoBehaviour
    {
        [SerializeField] private SectionName _sectionName;
        [SerializeField] private Transform _sectionBody;
        
        private SectionManager _sectionManager;
        private Button _menuButton;

        public SectionName SectionName => _sectionName;

        private void Awake()
        {
            _menuButton = GetComponent<Button>();
            _menuButton.onClick.AddListener(() => _sectionManager.SwitchToSection(_sectionName));
        }

        public void SetVisibleSectionBody(bool active) => _sectionBody.gameObject.SetActive(active);

        public void SetValueMenuButtonInteractable(bool value) => _menuButton.interactable = value;

        public void LinkToSectionManager(SectionManager sectionManager) => _sectionManager = sectionManager;
    }
}

