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
    //public bool MenusMainManagerLinked { get; private set; }

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
        }
        else
        {
            Debug.LogError($"{this} not linked to GameMainManager");
            ActivateMainMenusCamera(true);
        }
    }

    private void Start()
    {
        _topListController.InitialLoadTopList();
        TempSwitchToCanvsa(CanvasName.Options);
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
        playJukeBoxMainMenus.TurnOn(turnOn);
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
