using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Menu;
using UnityEngine.Events;

public class MainMenusSceneManager : MonoBehaviour
{

    [SerializeField] private GameObject cameraMainMenus;
    [SerializeField] private PlayJukeBox playJukeBoxMainMenus;
    [SerializeField] private TopListController _topListController;

    private GameMainManager _mainManager;
    public Func<bool> FuncGetStatusLoadingScenes;

    public Func<(List<string> values, UnityAction<int> actionOnValueChanged, int initialValue)> FuncGetParametersToInitGameComplexityOption;

    private void Awake()
    {
        CountFrame.DebugLogUpdate(this, $"Awake()");
        _mainManager = GameMainManager.Instance;
        //Not extensively tested
        ButtonActions.LinkMenuSceneManager(this);
        if (_mainManager)
        {
            //ButtonActions.LinkMenuSceneManager(this);
            _mainManager.LinkMenuSceneManager(this);
            //Camera will manage by GameMainManager
            ActivateMainMenusCamera(false);
            FuncGetStatusLoadingScenes = _mainManager.GetStatusLoadingScenes;
            FuncGetParametersToInitGameComplexityOption = _mainManager.GetParametersToInitGameComplexityOption;
        }
        else
        {
            Debug.LogError($"{this} not linked to GameMainManager");
            ActivateMainMenusCamera(true);
            StartCoroutine(EmulatorGetStatusLoadingScenes());
            FuncGetParametersToInitGameComplexityOption = EmulatorGetParametersToInitGameComplexityOption;
        }
    }

    private void Start()
    {
        _topListController.InitialLoadTopList();
        //TempSwitchToCanvsa(CanvasName.Options);
    }

    public (List<string> values, UnityAction<int> actionOnValueChanged, int initialValue) GetParametersToInitGameComplexityOption() => FuncGetParametersToInitGameComplexityOption();

    public bool GetStatusLoadingScenes() => FuncGetStatusLoadingScenes();

    //The GetStatusLoadingScenes() = true will be little postponed
    private IEnumerator EmulatorGetStatusLoadingScenes()
    {
        FuncGetStatusLoadingScenes = () => { return false; };
        yield return new WaitForSeconds(.5f);
        FuncGetStatusLoadingScenes = () => { return true; };
    }

    public (List<string> values, UnityAction<int> actionOnValueChanged, int initialValue) EmulatorGetParametersToInitGameComplexityOption()
        => (new List<string>() { "E", "N", "H" }, (int value) => Debug.Log($"new GameComplexity value = {value}"), 0);
    //{
    //    return (new List<string>() {"E","N","H"}, (int value) => Debug.Log($"new GameComplexity value = {value}"), 0);
    //}

    /// <summary>
    /// Method used for spped Testing of some Menu sections
    /// </summary>
    /// <param name="values"></param>
    /// <param name="actionOnValueChanged"></param>
    /// <param name="initialValue"></param>
    private static void TempSwitchToCanvsa(CanvasName canvasName)
    {
        Debug.LogError("Temporary Switched on TopList");
        FindObjectOfType<CanvasManager>().SwitchCanvas(canvasName);
    }

    public void StartGame() => _mainManager?.FromMenusToStartGame();

    public void ResetTopList() => _topListController.ResetTopList();

    public void TurnOnMusicMenus(bool turnOn = true)
    {
        if (turnOn)
        {
            playJukeBoxMainMenus.TurnOn(true);

        }
        else
        {
            playJukeBoxMainMenus.TurnOn(false);
        }
    }

    public void ActivateMainMenusCamera(bool activate)
    {
        cameraMainMenus.SetActive(activate);
    }

    public void AddNewCharacterData(CharacterData newCharacterData)
    {
        _topListController.AddNewCharacterData(newCharacterData);
    }
}
