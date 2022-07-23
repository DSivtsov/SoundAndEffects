using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GMTools;
using UnityEngine.SceneManagement;

public enum SceneState
{
    Load,
    Menu,
    Game
}

public class GameMainManager : SingletonController<GameMainManager>
{
    private LoaderSceneManager _loaderSceneManager;
    private MenuSceneManager _menuSceneManager;
    private GameSceneManager _gameSceneManager;

    public event Action<SceneState>  changeScene;

    protected override void Awake()
    {
        base.Awake();

    }

    private void Update()
    {
        //if (_loaderSceneManager.GetStatusLoadingScenes() && !init)
        //{
        //    Debug.Log($"AllScenesLoaded && !init = {++count}");
        //    changeScene?.Invoke(SceneState.Load);
        //    init = true;
        //}
    }

    public void LinkMenuSceneManager(MenuSceneManager menuSceneManager) => _menuSceneManager = menuSceneManager;
    public void LinkLoaderSceneManager(LoaderSceneManager loaderSceneManager) => _loaderSceneManager = loaderSceneManager;
    public void LinkGameSceneManager(GameSceneManager gameSceneManager) => _gameSceneManager = gameSceneManager;

    public void StartNewGame()
    {
        _menuSceneManager.TurnOnMainMenuCanvas(false);
        _gameSceneManager.TurnOnGameCanvas(true);
        SetActiveScene(SceneName.Game);
    }

    private static void SetActiveScene(SceneName sceneName)
    {
        //Debug.Log(SceneManager.GetActiveScene().name);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)sceneName));
        //Debug.Log(SceneManager.GetActiveScene().name);
    }

    public void AllScenesLoaded()
    {
        _loaderSceneManager.TurnOnLoaderCanvas(false);
    }
}
