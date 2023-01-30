using UnityEngine;
using GMTools.Menu.Elements;
using GMTools.Menu;

public class SectionGameOptionsController : MonoBehaviour
{
    [SerializeField] private SettingsSectionManager _sectionManager;
    [SerializeField] private SectionObject _gameSection;
    [Header("Game Settings Parameters")]
    [SerializeField] private GameSettingsSO _gameSettings;
    [Header("FieldsOnlinePlaymode")]
    [SerializeField] private GameObject _titleOnlineOptions;
    [SerializeField] private GameObject _notCopyToGlobal;
    [SerializeField] private GameObject _defaultTopListGlobal;
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
            _gameSettings.ChangedFieldPlaymode += ChangeFieldsOnlinePlaymode;
            ChangeFieldsOnlinePlaymode(_gameSettings.FieldPlayMode.GetCurrentValue()); 
        }
        else
            Debug.LogError($"{this} : Attempt to Use before GameSettingsInited was Inited");
    }

    private void ChangeFieldsOnlinePlaymode(PlayMode currentMode)
    {
        bool playModeIsOnline = currentMode == PlayMode.Online;
        _notCopyToGlobal.SetActive(playModeIsOnline);
        _defaultTopListGlobal.SetActive(playModeIsOnline);
        _titleOnlineOptions.SetActive(playModeIsOnline);
    }

    private void LinkFieldsToElement()
    {
        //Debug.LogError($"{this} : LinkFieldsToElement()");
        LinkFieldToElementBase.Link(_gameSettings.FieldPlayMode, _UIPlayMode);
        LinkFieldToElementBase.Link(_gameSettings.FieldNotCopyToGlobal, _UINotCopyToGlobal);
        LinkFieldToElementBase.Link(_gameSettings.FieldByDefaultShowGlobalTopList, _UIDefaultTopListGlobal);
        LinkFieldToElementBase.Link(_gameSettings.FieldComplexityGame, _UIComplexityGame);

        //Debug.LogWarning($"{this} : LoadSectionValues()");
        LinkFieldToElementBase.UpdateElementsValues();
    }
}