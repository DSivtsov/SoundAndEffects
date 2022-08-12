using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GMTools.Menu;

public interface IButtonAction
{
    public void ResetDefault();
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
    Options,
    EndScreen,
    Help
}

public enum ButtonType
{
    StartGame,
    QuitGame,
    ResetTopList,
    ResetDefault,
    NewPlayer
}

public static class ButtonActions
{
    private static MainMenusSceneManager _menuSceneManager;
    private static PlayerDataController _playerDataController;

    public static void LinkMenuSceneManager(MainMenusSceneManager menuSceneManager)
    {
        _menuSceneManager = menuSceneManager;
    }

    public static void LinkPlayerDataController(PlayerDataController playerDataController)
    {
        _playerDataController = playerDataController;
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
                    ((SectionManagerOptions)SectionManager.ActiveSectionManager).ResetDefault();
                    //Debug.Log("End ButtonType.ResetDefault");
                    break;
                case ButtonType.NewPlayer:
                    _playerDataController.CreateNewPlayer();
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
