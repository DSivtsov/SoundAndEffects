using UnityEngine;
using UnityEngine.Audio;
using GMTools.Menu.Elements;
using GMTools;
using GMTools.Menu;

public class SectionVideoOptionsController : MonoBehaviour
{
    [SerializeField] private SettingsSectionManager _sectionManagerOptions;
    [SerializeField] private SectionObject _audioSection;
    [Header("Video Options Parameters")]
    [SerializeField] private GameSettingsSO _gameSettings;
    [Header("UI Elements")]
    [SerializeField] private ToggleBool _UINotShowIntroductionText;
    [SerializeField] private ToggleBool _UINotShowCollisionAnimation;

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
        LinkFieldToElementBase.Link(_gameSettings.FieldNotShowCollisionAnimation, _UINotShowCollisionAnimation);
        LinkFieldToElementBase.Link(_gameSettings.FieldNotShowIntroductionText, _UINotShowIntroductionText);


        //Debug.LogWarning($"{this} : LoadSectionValues()");
        LinkFieldToElementBase.UpdateElementsValues();
    }
}

