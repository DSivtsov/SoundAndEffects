using System;
using UnityEngine;
using UnityEngine.UI;

namespace GMTools.Menu
{
    public class SectionObject : MonoBehaviour
    {
        [SerializeField] private SectionName _sectionName;
        [SerializeField] private Transform _sectionBody;

        public SectionName SectionName => _sectionName;
        private SectionManager _sectionManager;
        private Button _menuButton;

        private void Awake()
        {
            //Debug.Log($"SectionObject : [{this}] Awake");
            _menuButton = GetComponent<Button>();
        }

        private void OnEnable() => _menuButton.onClick.AddListener(OnButtonClicked);

        private void OnDisable() => _menuButton.onClick.RemoveListener(OnButtonClicked);

        private void OnButtonClicked() => _sectionManager.SwitchSection(_sectionName);

        public void SetVisibleSectionBody(bool active) => _sectionBody.gameObject.SetActive(active);

        public void SetValueMenuButtonInteractable(bool value) => _menuButton.interactable = value;

        public void LinkToSectionManager(SectionManager sectionManager)
        {
            _sectionManager = sectionManager;
        }
    }
}