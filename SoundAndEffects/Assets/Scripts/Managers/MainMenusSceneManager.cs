using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Menu;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenusSceneManager : MonoBehaviour
{

    [SerializeField] private GameObject cameraMainMenus;
    [SerializeField] private PlayJukeBox playJukeBoxMainMenus;
    [SerializeField] private TopListController _topListController;
    [SerializeField] private TopListController _remoteTopListController;
    [SerializeField] private LootLockerController _lootLockerController;
    [SerializeField] private PlayerDataController _playerDataController;
    [SerializeField] private Button _buttonStart;
    [Header("Demo Options")]
    [SerializeField] private bool _useStartCanvas;
    [SerializeField] private CanvasName _startCanvasName;
    [SerializeField] private bool _startAlwaysActive;

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
        _buttonStart.interactable = false;
#if UNITY_EDITOR
        if (_startAlwaysActive)
        {
            _buttonStart.interactable = true;
        }
#endif
    }

    //public void SetPlayerName(string newPlayerName) => _playerDataController.PlayerName = newPlayerName;

    public void CreateNewPlayerLootLocker(string playerName)
    {
        StartCoroutine(_lootLockerController.CreateNewPlayerRecord(playerName));
        Debug.Log($"CreateNewPlayerLootLocker({playerName})");
    }

    private void Start()
    {
        _topListController.InitialLoadTopList();
        _remoteTopListController.InitialLoadTopList();
#if UNITY_EDITOR
        if (_useStartCanvas)
        {
            TempSwitchToCanvsa(_startCanvasName);
        }
#endif
    }

    public void ActivateButtonStart(bool activate) => _buttonStart.interactable = activate;

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

    public void StartGame(string playerName) => _mainManager?.FromMenusToStartGame(playerName);

    public void ResetTopList() => _topListController.ResetTopList();

    public void ActivateMusicMainMenus(bool activate = true)
    {
        if (activate)
        {
            playJukeBoxMainMenus.SetJukeBoxActive(true);
            playJukeBoxMainMenus.TurnOn(true);
        }
        else
        {
            playJukeBoxMainMenus.TurnOn(false);
            playJukeBoxMainMenus.SetJukeBoxActive(false);
        }
    }

    public void ActivateMainMenusCamera(bool activate)
    {
        cameraMainMenus.SetActive(activate);
    }

    public void AddNewCharacterData(PlayerData newCharacterData)
    {
        Debug.Log(newCharacterData);
        _topListController.AddNewCharacterData(newCharacterData);
        _remoteTopListController.AddNewCharacterData(newCharacterData);
        //_lootLockerController.StartSendScoreToLeaderBoard(newCharacterData);
    }
}
