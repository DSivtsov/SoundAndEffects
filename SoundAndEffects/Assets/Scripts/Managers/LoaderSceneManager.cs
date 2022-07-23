using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject loaderCameraObj;
    [SerializeField] private GameObject loaderCanvas;
    [SerializeField] private LoaderManager _loaderManager;

    private GameMainManager _gameMainManager;
    private void Awake()
    {
        _gameMainManager = GameMainManager.Instance;
        if (_gameMainManager)
        {
            _gameMainManager.LinkLoaderSceneManager(this); ;
        }
        else
            Debug.LogError($"{this} not linked to GameMainManager");
    }

    public bool GetStatusLoadingScenes() => _loaderManager.AllScenesLoaded;

    public void TurnOnLoaderCanvas(bool value)
    {
        loaderCameraObj.SetActive(value);
        loaderCanvas.SetActive(value);
    }
}
