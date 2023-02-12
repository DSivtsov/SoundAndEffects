using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Menu;

public class CanvasManagerCityRunner : CanvasManager
{
    [Space]
    [SerializeField] private MainMenusSceneManager _mainMenusSceneManager;
    protected override void SwitchCanvasCallSpecificActions(CanvasObject prevCanvasObject, CanvasObject nextCanvasObject)
    {
        if (prevCanvasObject?.CanvasName == CanvasName.Settings && nextCanvasObject?.CanvasName == CanvasName.MainMenu)
        {
            _mainMenusSceneManager.ManageConnectionToServer();
        }
    }
}
