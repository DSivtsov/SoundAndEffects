using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GMTools.Menu;


public enum SectionName
{
    Game,
    Audio,
    Video
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
    START_GAME,
    KILL_PLAYER,
    TOP_LIST
}

public static class ButtonActions
{
    private static MenuSceneManager _menuSceneManager;

    public static void LinkMenuSceneManager(MenuSceneManager menuSceneManager)
    {
        _menuSceneManager = menuSceneManager;
    }

    public static void ButtonPressed(this ButtonType buttonType)
    {
        if (_menuSceneManager)
        {
            switch (buttonType)
            {
                case ButtonType.START_GAME:
                    GameMainManager.Instance.StartNewGame();
                    break;
                case ButtonType.KILL_PLAYER:
                    CanvasManager.Instance.SwitchCanvas(CanvasName.EndScreen);
                    break;
                //case ButtonType.TOP_LIST:
                //    Debug.Log("ButtonType.TOP_LIST");
                //    break;
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
