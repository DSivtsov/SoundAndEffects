using UnityEngine;
using UnityEngine.Audio;
using GMTools.Menu.Elements;
using GMTools;
using GMTools.Menu;

public class SectionAudioOptionsController : MonoBehaviour
{
    [SerializeField] private SettingsSectionManager _sectionManagerOptions;
    [SerializeField] private SectionObject _audioSection;
    [SerializeField] private GameSettingsSO _gameSettings;
    [Header("UI Elements")]
    [SerializeField] private DropdownListEnum<SequenceType> _UISequenceType;
    [SerializeField] private ValueableSliderInt _UIMasterVolume;
    [SerializeField] private ValueableSliderInt _UIMusicVolume;
    [SerializeField] private ValueableSliderInt _UIEffectsVolume;

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
        LinkFieldToElementBase.Link(_gameSettings.FieldMasterVolume, _UIMasterVolume);
        LinkFieldToElementBase.Link(_gameSettings.FieldMusicVolume, _UIMusicVolume);
        LinkFieldToElementBase.Link(_gameSettings.FieldEffectVolume, _UIEffectsVolume);
        LinkFieldToElementBase.Link(_gameSettings.FieldSequenceType, _UISequenceType);
    }
}
