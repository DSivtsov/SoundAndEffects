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
    StartGame,
    QuitGame,
    ResetTopList,
    ResetDefault
}

public static class ButtonActions
{
    private static MainMenusSceneManager _menuSceneManager;
    //private static SectionManager _sectionManagerOptions;

    public static void LinkMenuSceneManager(MainMenusSceneManager menuSceneManager)
    {
        _menuSceneManager = menuSceneManager;
    }

    public static void ButtonPressed(this ButtonType buttonType)
    {
        if (_menuSceneManager)
        {
            switch (buttonType)
            {
                case ButtonType.StartGame:
                    _menuSceneManager.StartGame();
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
