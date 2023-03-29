using System.Collections;
using UnityEngine;
using GMTools;
using UnityEngine.SceneManagement;

public enum SceneState
{
    Load,
    Menu,
    Game
}

public class MainManager : SingletonController<MainManager>
{
    private LoaderSceneManager _loaderSceneManager;
    private MainMenusSceneManager _menuSceneManager;
    private GameSceneManager _gameSceneManager;

    private int _overrideCharacterHealth;
    /*
     * The Start order:
     *  Turn Loader camera
     *  Load GameSettings and Set Environment variables
     *  Turn on Music
     *  Load Scenes
     */
    private void Start()
    {
        _loaderSceneManager.ActivateLoaderCamera(true);
        GameSettingsSOController.Instance.InitGameSettings();
        _loaderSceneManager.ActivateMusicLoaderMenus(true);
        StartCoroutine(_loaderSceneManager.StartLoadScenes());
    }

    public void OverrideCharacterHealth(int overrideCharacterHealth) => _overrideCharacterHealth = overrideCharacterHealth;
    public void LinkMenuSceneManager(MainMenusSceneManager menuSceneManager) => _menuSceneManager = menuSceneManager;
    public void LinkLoaderSceneManager(LoaderSceneManager loaderSceneManager) => _loaderSceneManager = loaderSceneManager;
    public void LinkGameSceneManager(GameSceneManager gameSceneManager) => _gameSceneManager = gameSceneManager;

    public bool GetStatusLoadingScenes() => _loaderSceneManager.AllScenesLoaded;

    private static void SetActiveScene(SceneName sceneName) => SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)sceneName));

    public void AllScenesLoaded()
    {
        CountFrame.DebugLogUpdate(this, $"AllScenesLoaded()");
        FromLoaderToMenus();
        _menuSceneManager.Introduction();
    }

    public void FromLoaderToMenus()
    {
        _menuSceneManager.ActivateMainMenusCamera(true);
        SwitchMusicTo(SceneName.Menus);
        SetActiveScene(SceneName.Menus);
        _gameSceneManager.ActivateGameCamera(false);
        _loaderSceneManager.ActivateLoaderCamera(false);
    }

    public void FromMenusToStartGame(string playerName)
    {
        CountFrame.DebugLogUpdate(this, $"FromMenusToStartGame()");
        SetActiveScene(SceneName.Game);
        _gameSceneManager.StartNewGame(playerName, _overrideCharacterHealth);
        _gameSceneManager.ActivateGameCamera(true);
        _menuSceneManager.ActivateMainMenusCamera(false);
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
                _loaderSceneManager.ActivateMusicLoaderMenus();
                _gameSceneManager.ActivateGameMusic(false);
                break;
            case SceneName.Game:
                _gameSceneManager.ActivateGameMusic();
                _loaderSceneManager.ActivateMusicLoaderMenus(false);
                break;
            default:
                Debug.LogError("{this} SwitchMusicTo() can't switch to {scene} Scene");
                break;
        }
    }

    public void AddNewCharacterData(PlayerData newCharacterData)
    {
        _menuSceneManager.AddNewCharacterData(newCharacterData);
    }
}
