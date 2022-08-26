using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GMTools.Menu;

public class SectionGameSettingsController : MonoBehaviour, ISectionControllerAction, IOptionChanged
{
    [SerializeField] private SettingsSectionManager _sectionManagerOptions;
    [SerializeField] private SectionObject _gameSection;
    [SerializeField] private GameSettingsControllerSO _gameSettingsControllerSO;

    private DropdownOption<SectionGameSettingsController> _gameComplexityField;
    private ToggleGroupEnumController<PlayMode, SectionGameSettingsController> _playModeToggleGroup;
    private MainMenusSceneManager _mainMenusSceneManager;
    public bool IsGameOptionsChanged { get; private set; }

    private void Awake()
    {
        _gameComplexityField = new DropdownOption<SectionGameSettingsController>("GameComplexity", _gameSection.TransformSection, this) ;
        _playModeToggleGroup = new ToggleGroupEnumController<PlayMode, SectionGameSettingsController>("PlayMode", _gameSection.TransformSection, this);
        IsGameOptionsChanged = false;
        _mainMenusSceneManager = FindObjectOfType<MainMenusSceneManager>();
    }

    private void Start()
    {
        //Link the current section controller with Name of this Section in the Dictionary from the class derived from the SectionManager class
        _sectionManagerOptions.LinkToSectionActions(_gameSection.NameSection, this);
        //StartCoroutine(InitComplexityOptionCoroutine());
        //StartCoroutine(InitPlayModeToggleGroupCoroutine());
    }
    /// <summary>
    /// InitComplexityOptionCoroutine() postpone the initialization till the all Scenes will loaded
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitComplexityOptionCoroutine()
    {
        int count = 0;
        while (!_mainMenusSceneManager.GetStatusLoadingScenes())
        {
            count++;
            yield return null;
        }
        Debug.Log($"InitComplexityOptionCoroutine : Count ={count}");
        (List<string> values, UnityAction<int> actionOnValueChanged, int initialValue) = _mainMenusSceneManager.GetParametersToInitGameComplexityOption();
        _gameComplexityField.InitOption(values, actionOnValueChanged, initialValue);
    }

    private IEnumerator InitPlayModeToggleGroupCoroutine()
    {
        int count = 0;
        while (!(_playModeToggleGroup.Initialized && _gameSettingsControllerSO.GameSettingsInitialized))
        {
            //Debug.Log($"_playModeToggleGroup={_playModeToggleGroup.Initialized } GameSettingsInitialized={_gameSettingsController.GameSettingsInitialized}");
            count++;
            yield return null;
        }
        Debug.Log($"InitPlayModeToggleGroupCoroutine : Count ={count}");
        _playModeToggleGroup.Init((value) => Debug.Log($"SectionGameSettingsController : value={value}"), _gameSettingsControllerSO._currentGameSettings.UsedPlayMode);
    }

    //public void SetValues(PlayMode usedPlayMode, GameSettings current)
    //{
    //    Debug.Log($"current={current}");
    //    _playModeToggleGroup.SetValueWithoutNotify(current.UsedPlayMode);
    //    //_playModeToggleGroup.ChangeValue(usedPlayMode);
    //    //(List<string> values, UnityAction<int> actionOnValueChanged, int initialValue) = _mainMenusSceneManager.GetParametersToInitGameComplexityOption();
    //    //_playModeMyToggleGroup.InitField(actionOnValueChanged, _currentGameSettings.PlayMode);
    //    //_playModeToggleGroup.InitToggleGroup((test) => Debug.Log(test), YesNo.No);
    //    //_gameComplexityOption.InitOption(values, actionOnValueChanged, initialValue);
    //}
    public void OptionsChanged(bool isChanged)
    {
        if (!IsGameOptionsChanged && isChanged)
        {
            IsGameOptionsChanged = true;
            Debug.Log("_sectionManagerOptions.ActivateResetButton(true)");
            _sectionManagerOptions.ActivateResetButton(true);
            return;
        }
        if (!isChanged && IsGameOptionsChanged)
        {
            IsGameOptionsChanged = false;
            _sectionManagerOptions.ActivateResetButton(false);
            return;
        }
    }
    /// <summary>
    /// Actions on ResetDeafult pressing
    /// </summary>
    public void ResetSectionValuesToDefault()
    {
        _gameComplexityField.ResetDefaultValue();
        OptionsChanged(false);
    }

    public void LoadSectionValues()
    {
        //throw new System.NotImplementedException();
        Debug.LogWarning($"{this} : LoadSectionValues()");
        StartCoroutine(InitComplexityOptionCoroutine());
        StartCoroutine(InitPlayModeToggleGroupCoroutine());
    }
}