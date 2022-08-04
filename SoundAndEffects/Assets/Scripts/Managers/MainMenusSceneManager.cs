using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Menu;

public class MainMenusSceneManager : MonoBehaviour
{

    [SerializeField] private GameObject cameraMainMenus;
    [SerializeField] private PlayJukeBox playJukeBoxMainMenus;
    [SerializeField] private TopListController _topListController;

    private GameMainManager _mainManager;
    public Func<bool> FuncGetStatusLoadingScenes;

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
        }
        else
        {
            Debug.LogError($"{this} not linked to GameMainManager");
            ActivateMainMenusCamera(true);
            StartCoroutine(EmulatorGetStatusLoadingScenes());
        }
    }

    private void Start()
    {
        _topListController.InitialLoadTopList();
        //TempSwitchToCanvsa(CanvasName.Options);
    }

    public bool GetStatusLoadingScenes() => FuncGetStatusLoadingScenes();

    //The GetStatusLoadingScenes() = true will be little postponed
    private IEnumerator EmulatorGetStatusLoadingScenes()
    {
        FuncGetStatusLoadingScenes = () => { return false; };
        yield return new WaitForSeconds(.5f);
        FuncGetStatusLoadingScenes = () => { return true; };
    }

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
