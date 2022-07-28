using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GMTools;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public enum SceneState
{
    Load,
    Menu,
    Game
}

public class GameMainManager : SingletonController<GameMainManager>
{
    private LoaderSceneManager _loaderSceneManager;
    private MainMenusSceneManager _menuSceneManager;
    private GameSceneManager _gameSceneManager;

    protected override void Awake()
    {
        base.Awake();
    }

    public void LinkMenuSceneManager(MainMenusSceneManager menuSceneManager) => _menuSceneManager = menuSceneManager;
    public void LinkLoaderSceneManager(LoaderSceneManager loaderSceneManager) => _loaderSceneManager = loaderSceneManager;
    public void LinkGameSceneManager(GameSceneManager gameSceneManager) => _gameSceneManager = gameSceneManager;

    public void FromMenusToStartGame()
    {
        CountFrame.DebugLogUpdate(this,$"FromMenusToStartGame()");
        _gameSceneManager.ActivateGameCamera(true);
        SetActiveScene(SceneName.Game);
        //_gameSceneManager.SelectGameObjectFromScene();
        _gameSceneManager.StartNewGame();
        _menuSceneManager.ActivateMainMenusCamera(false);
        //_menuSceneManager.TurnOffMusicMenus();
    }

    private static void SetActiveScene(SceneName sceneName) => SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)sceneName));

    public void AllScenesLoaded()
    {
        CountFrame.DebugLogUpdate(this, $"AllScenesLoaded()");
        FromLoaderToMenus();
    }

    public void FromLoaderToMenus()
    {
        _menuSceneManager.ActivateMainMenusCamera(true);
        SetActiveScene(SceneName.Menus);
        _gameSceneManager.ActivateGameCamera(false);
        _loaderSceneManager.ActivateLoaderCamera(false);
    }

    public void FromGameToMenus()
    {
        _menuSceneManager.ActivateMainMenusCamera(true);
        SwitchMusicTo(SceneName.Menus);
        SetActiveScene(SceneName.Menus);
        _gameSceneManager.ActivateGameCamera(false);
    }

    public void SwitchMusicTo(SceneName scene)
    {
        switch (scene)
        {
            case SceneName.Menus:
                _menuSceneManager.TurnOnMusicMenus();
                _gameSceneManager.TurnOnMusic(false);
                break;
            case SceneName.Game:
                _gameSceneManager.TurnOnMusic();
                _menuSceneManager.TurnOnMusicMenus(false);
                break;
            default:
                Debug.LogError("{this} SwitchMusicTo() can't switch to {scene} Scene");
                break;
        }

    }

    public void AddAndSaveNewCharacterData(CharacterData newCharacterData)
    {
        Debug.Log("AddAndSaveNewCharacterData()");
    }
}
