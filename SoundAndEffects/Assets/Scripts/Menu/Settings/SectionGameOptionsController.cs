using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Menu.Elements;
using GMTools.Menu;

public class SectionGameOptionsController : MonoBehaviour
{
    [SerializeField] private SettingsSectionManager _sectionManager;
    [SerializeField] private SectionObject _gameSection;
    [Header("Game Settings Parameters")]
    [SerializeField] private GameSettingsSO _gameSettings;
    [Header("UI Elements")]
    [SerializeField] private ToggleGroupEnum<PlayMode> _UIPlayMode;
    [SerializeField] private ToggleGroupBoolEnum<YesNoToggleEnum> _UINotCopyToGlobal;
    [SerializeField] private ToggleGroupBoolEnum<DefaultTopListToggleEnum> _UIDefaultTopListGlobal;
    [SerializeField] private DropdownListSO<ComplexitySO> _UIComplexityGame;

    private GameSettingsSOController _gameSettingsSOController;

    private void Awake()
    {
        _gameSettingsSOController = GameSettingsSOController.Instance;
    }
    private void Start()
    {
        LinkFieldsToElement();
    }
    private void OnEnable()
    {
        _gameSettingsSOController.UpdateElementFromFields += UpdateElementsValueFromFields;
    }
    private void OnDisable()
    {
        _gameSettingsSOController.UpdateElementFromFields -= UpdateElementsValueFromFields;
    }

    private void LinkFieldsToElement()
    {
        Debug.LogError($"{this} : LinkFieldsToElement()");
        LinkFieldToElementBase.Link(_gameSettings.FieldPlayMode, _UIPlayMode);
        LinkFieldToElementBase.Link(_gameSettings.FieldNotCopyToGlobal, _UINotCopyToGlobal);
        LinkFieldToElementBase.Link(_gameSettings.FieldByDefaultShowGlobalTopListl, _UIDefaultTopListGlobal);
        LinkFieldToElementBase.Link(_gameSettings.FieldComplexityGame, _UIComplexityGame);

        Debug.LogWarning($"{this} : LoadSectionValues()");
        UpdateElementsValueFromFields();
    }

    private void UpdateElementsValueFromFields() => LinkFieldToElementBase.UpdateElementsValues();
}