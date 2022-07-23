using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject gameCamera;
    [SerializeField] private GameObject gameCanvas;

    private GameMainManager _gameMainManager;
    private void Awake()
    {
        _gameMainManager = GameMainManager.Instance;
        if (_gameMainManager)
        {
            _gameMainManager.LinkGameSceneManager(this);
        }
        else
            Debug.LogError($"{this} not linked to GameMainManager");
    }

    public void TurnOnGameCanvas(bool value)
    {
        gameCanvas.SetActive(value);
        //ActivateGameScene();
    }

    public void ManDied()
    {
        Debug.LogWarning("ManDied()");
    }

    public bool TryDecreaseLifes()
    {
        if (true)
        {
            // Character lost the one life and continue the game
            return true;
        }
        else
        {
            //Character starts to die 
            return false;
        }
    }

    //public void ActivateGameScene()
    //{
    //    Debug.Log(SceneManager.GetActiveScene().name);
    //    SceneManager.SetActiveScene(gameObject.scene);
    //    Debug.Log(SceneManager.GetActiveScene().name);
    //}
}
