using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GMTools.Menu;

public class GameOptionController : MonoBehaviour, IButtonAction, IOptionChanged
{
    [SerializeField] private SectionManagerOptions _sectionManagerOptions;
    //[SerializeField] private SectionName _sectionName;
    //[SerializeField] private Transform _gameGroupOptions;
    [SerializeField] private SectionObject _gameSection;

    private DropdownOption<GameOptionController> _gameComplexityOption;
    private MainMenusSceneManager _mainMenusSceneManager;

    public bool IsGameOptionsChanged { get; private set; }

    private void Awake()
    {
        _gameComplexityOption = new DropdownOption<GameOptionController>("GameComplexity", _gameSection.TransformSection, this) ;
        IsGameOptionsChanged = false;
        _mainMenusSceneManager = FindObjectOfType<MainMenusSceneManager>();
    }

    private void Start()
    {
        _sectionManagerOptions.LinkToButtonActions(_gameSection.NameSection, this);
        StartCoroutine(InitComplexityOptionCoroutine());
    }
    /// <summary>
    /// InitComplexityOptionCoroutine() postpone the initialization till the all Scenes will loaded
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitComplexityOptionCoroutine()
    {
        do
        {
            yield return null;
        } while (!_mainMenusSceneManager.GetStatusLoadingScenes());
        (List<string> values, UnityAction<int> actionOnValueChanged, int initialValue) = _mainMenusSceneManager.GetParametersToInitGameComplexityOption();
        _gameComplexityOption.InitOption(values, actionOnValueChanged, initialValue);
    }

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
    public void ResetDefault()
    {
        _gameComplexityOption.ResetDefaultValue();
        OptionsChanged(false);
    }
}