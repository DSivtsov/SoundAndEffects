using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoaderSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject loaderCameraObj;
    [SerializeField] private LoaderManager _loaderManager;
    [Tooltip("To show the Loading screen in loading")]
    [SerializeField] private bool _makePauseBeforeStartLoad = true;
    [SerializeField] private float _pauseTime = 1f;

    private MainManager _gameMainManager;
    private void Awake()
    {
        _gameMainManager = MainManager.Instance;
        if (_gameMainManager)
        {
            _gameMainManager.LinkLoaderSceneManager(this);
        }
        else
            Debug.LogError($"{this} not linked to GameMainManager");
        ActivateLoaderCamera(true);
        //loaderCanvas.SetActive(true);
    }

    private IEnumerator Start()
    {
        if (_makePauseBeforeStartLoad)
        {
            yield return new WaitForSeconds(_pauseTime); 
        }
        _loaderManager.LoadScenes();
    }

    public bool AllScenesLoaded => _loaderManager.AllScenesLoaded;

    public void ActivateLoaderCamera(bool activate)
    {
        loaderCameraObj.SetActive(activate);
    }
}
