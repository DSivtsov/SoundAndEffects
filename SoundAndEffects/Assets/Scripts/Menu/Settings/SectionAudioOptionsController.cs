using UnityEngine;
using UnityEngine.Audio;
using GMTools.Menu.Elements;
using GMTools;
using GMTools.Menu;

public class SectionAudioOptionsController : MonoBehaviour
{
    [SerializeField] private SettingsSectionManager _sectionManagerOptions;
    [SerializeField] private SectionObject _audioSection;
    [Header("Audio Options Parameters")]
    [SerializeField] private GameSettingsSO _gameSettings;
    [SerializeField] private AudioMixer _mixerMain;
    [SerializeField] private JukeBoxSO _jukeBoxSO;
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
        //Debug.LogError($"{this} : LinkFieldsToElement()");
        LinkFieldToElementBase.Link(_gameSettings.FieldMasterVolume, _UIMasterVolume);
        LinkFieldToElementBase.Link(_gameSettings.FieldMusicVolume, _UIMusicVolume);
        LinkFieldToElementBase.Link(_gameSettings.FieldEffectVolume, _UIEffectsVolume);
        LinkFieldToElementBase.Link(_gameSettings.FieldSequenceType, _UISequenceType);

        //Debug.LogWarning($"{this} : LoadSectionValues()");
        LinkFieldToElementBase.UpdateElementsValues();
    }
}
