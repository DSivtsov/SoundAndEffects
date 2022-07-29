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

    private GameMainManager _gameMainManager;

    private void Awake()
    {
        CountFrame.DebugLogUpdate(this, $"Awake()");
        _gameMainManager = GameMainManager.Instance;
        if (_gameMainManager)
        {
            ButtonActions.LinkMenuSceneManager(this);
            _gameMainManager.LinkMenuSceneManager(this);
            //Camera will manage by GameMainManager
            ActivateMainMenusCamera(false);
        }
        else
        {
            Debug.LogError($"{this} not linked to GameMainManager");
            ActivateMainMenusCamera(true);
        }
        _topListController.InitialLoadTopList();
        //Debug.LogError("Temporary Switched on TopList");
        //FindObjectOfType<CanvasManager>().SwitchCanvas(CanvasName.TopList);
    }

    public void TurnOnMusicMenus(bool turnOn = true)
    {
        playJukeBoxMainMenus.TurnOn(turnOn);
    }

    public void ActivateMainMenusCamera(bool activate)
    {
        cameraMainMenus.SetActive(activate);
    }
}
