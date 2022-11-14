using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GMTools.Menu;
using System;

public interface ISectionControllerAction
{
    public void ResetSectionValuesToDefault();
    public void LoadSectionValues();
}

public enum SectionName
{
    Game,
    Audio,
    Video,
    Global,
    Local
}

public enum CanvasName
{
    MainMenu,
    TopList,
    Settings,
    EndScreen,
    Help
}

public enum ButtonType
{
    StartGame = 0,
    QuitGame = 1,
    ResetTopList = 2,
    ResetDefault = 3,
    NewPlayer = 4,
    LoadSavedSettings = 5,
    LoadDefSettings = 6,
    SaveNewSettings = 7,
}

public static class ButtonActions
{
    private static MainMenusSceneManager _menuSceneManager;
    private static PlayerDataController _playerDataController;
    private static GameSettingsSOController _gameSettingsSOController;

    public static void LinkMenuSceneManager(MainMenusSceneManager menuSceneManager)
    {
        _menuSceneManager = menuSceneManager;
    }

    public static void LinkPlayerDataController(PlayerDataController playerDataController)
    {
        _playerDataController = playerDataController;
    }
    public static void LinkGameSettingsSOController(GameSettingsSOController gameSettingsSOController)
    {
        _gameSettingsSOController = gameSettingsSOController;
    }
    public static void ButtonPressed(this ButtonType buttonType)
    {
        if (_menuSceneManager)
        {
            switch (buttonType)
            {
                case ButtonType.StartGame:
                    _menuSceneManager.StartGame(_playerDataController.PlayerName);
                    break;
                case ButtonType.QuitGame:
#if UNITY_EDITOR
                    Debug.LogError("Application.Quit()");
#else
                    Application.Quit();
#endif
                    //CanvasManager.Instance.SwitchCanvas(CanvasName.EndScreen);
                    break;
                case ButtonType.ResetTopList:
                    _menuSceneManager.ResetTopList();
                    break;
                case ButtonType.ResetDefault:
                    SectionManager.SectionController.ResetSectionValuesToDefault();
                    //Debug.Log("End ButtonType.ResetDefault");
                    break;
                case ButtonType.NewPlayer:
                    _playerDataController.CreateNewPlayer();
                    break;
                case ButtonType.LoadDefSettings:
                    _gameSettingsSOController.LoadDefault();
                    break;
                case ButtonType.LoadSavedSettings:
                    _gameSettingsSOController.Load();
                    break;
                case ButtonType.SaveNewSettings:
                    _gameSettingsSOController.Save();
                    break;
                default:
                    Debug.LogError($"ButtonPressed for [{buttonType}] button not set");
                    break;
            }
        }
        else
        {
            Debug.LogError("InitButtonActions not initialized");
        }
    }


}
