using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement

public class MenuSceneManager : MonoBehaviour
{

    [SerializeField] private GameObject menuCamera;
    [SerializeField] private GameObject menuCanvas;

    private GameMainManager _gameMainManager;
    private void Awake()
    {
        _gameMainManager = GameMainManager.Instance;
        if (_gameMainManager)
        {
            ButtonActions.LinkMenuSceneManager(this);
            _gameMainManager.LinkMenuSceneManager(this); 
        }
        else
            Debug.LogError($"{this} not linked to GameMainManager");
    }

    private void OnEnable()
    {
        _gameMainManager.changeScene += GameSceneManager_changeScene;
    }

    private void OnDisable()
    {
        _gameMainManager.changeScene -= GameSceneManager_changeScene;
    }

    private void GameSceneManager_changeScene(SceneState sceneState)
    {
        Debug.Log($"{this.gameObject.scene.name} - {sceneState}");
    }

    public void TurnOnMainMenuCanvas(bool value)
    {
        menuCamera.SetActive(value);
        menuCanvas.SetActive(value);
    }

    //public void StartNewGame()
    //{
    //    TurnOnSceneMainMenu(false);
    //    SingletonController.Instance.gameMenu.TurnOnGameCanvas(true);
    //}
}
