using UnityEngine;
using GMTools.Menu.Elements;
using GMTools.Menu;

public class SectionGameOptionsController : MonoBehaviour
{
    [SerializeField] private SettingsSectionManager _sectionManager;
    [SerializeField] private SectionObject _gameSection;
    [SerializeField] private GameSettingsSO _gameSettings;
    [Header("UI Elements")]
    [SerializeField] private ToggleGroupEnum<PlayMode> _UIPlayMode;
    [SerializeField] private ToggleGroupBoolEnum<YesNoToggleEnum> _UINotCopyToGlobal;
    [SerializeField] private ToggleGroupBoolEnum<DefaultTopListToggleEnum> _UIDefaultTopListGlobal;
    [SerializeField] private DropdownListSO<ComplexitySO> _UIComplexityGame;
    

    private void Awake()
    {
        if (GameSettingsSOController.Instance.GameSettingsInited)
        {
            LinkFieldsToElement();
        }
        else
            Debug.LogError($"{this} : Attempt to Use before GameSettingsInited was Inited");
    }

    private void LinkFieldsToElement()
    {
        LinkFieldToElementBase.Link(_gameSettings.FieldPlayMode, _UIPlayMode);
        LinkFieldToElementBase.Link(_gameSettings.FieldNotCopyToGlobal, _UINotCopyToGlobal);
        LinkFieldToElementBase.Link(_gameSettings.FieldByDefaultShowGlobalTopList, _UIDefaultTopListGlobal);
        LinkFieldToElementBase.Link(_gameSettings.FieldComplexityGame, _UIComplexityGame);
    }
}