using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject _loaderCamera;
    [SerializeField] private LoaderScenes _loaderScenes;
    [SerializeField] private PlayJukeBox _playJukeBoxLoaderMenus;

    private const bool MakePauseBeforeStartLoad = true;

    private void Awake()
    {
        MainManager _gameMainManager = MainManager.Instance;
        if (_gameMainManager)
        {
            _gameMainManager.LinkLoaderSceneManager(this);
        }
        else
        {
            Debug.LogError($"{this} not linked to GameMainManager");
        }

    }

    //Postpone the start of loading
    //Before start loading show the loader screen and start music
    public IEnumerator StartLoadScenes()
    {
#if UNITY_EDITOR
        yield return _loaderScenes.UnLoadScenes();
#endif
        if (MakePauseBeforeStartLoad)
        {
            do
            {
                //CountFrame.DebugLogUpdate(this, $"IsJukeBoxPlaying [{_playJukeBoxLoaderMenus.GetIsJukeBoxPlaying()}]");
                yield return null;
            } while (!_playJukeBoxLoaderMenus.GetIsJukeBoxPlaying());
        }
        _loaderScenes.LoadScenes();
    }

    public bool AllScenesLoaded => _loaderScenes.AllScenesLoaded;

    public void ActivateLoaderCamera(bool activate)
    {
        _loaderCamera.SetActive(activate);
    }
    public void ActivateMusicLoaderMenus(bool activate = true)
    {
        if (activate)
        {
            _playJukeBoxLoaderMenus.SetJukeBoxActive(true);
            _playJukeBoxLoaderMenus.TurnOn(true);
        }
        else
        {
            _playJukeBoxLoaderMenus.TurnOn(false);
            _playJukeBoxLoaderMenus.SetJukeBoxActive(false);
        }
    }


}
