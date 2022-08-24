using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Manager;

public class GameSettingsController : MonoBehaviour
{
    [SerializeField] private SectionGameSettingsController _sectionGameSettingsController;
    [SerializeField] private GameSettings _defaultGameSettings = new GameSettings();
    [NonSerialized] public GameSettings _currentGameSettings = new GameSettings();
    private StoreObjectT<GameSettings> _storeObjectT;
    private StoreGame _storeGame;
    public bool GameSettingsInitialized { get; private set; }

    private void Awake()
    {
        GameSettingsInitialized = false;
        _storeObjectT = GetComponent<StoreObjectT<GameSettings>>();
        _storeGame = _storeObjectT.GetStoreGameObject();
    }

    //public void Test()
    //{
    //    Debug.Log($"_defaultGameSettings.PlayMode={_defaultGameSettings.UsedPlayMode} {(int)_defaultGameSettings.UsedPlayMode}");
    //    Debug.Log($"_currentGameSettings.PlayMode={_currentGameSettings.UsedPlayMode} {(int)_currentGameSettings.UsedPlayMode}");
    //    //Debug.Log($"_currentGameSettings={_currentGameSettings}");
    //    _sectionGameSettingsController.SetValues(_currentGameSettings.UsedPlayMode, _currentGameSettings);
    //}

    public void SaveGameSettings()
    {
        _storeObjectT.SetObjectsToSave(new GameSettings[] { _currentGameSettings });
        _storeGame.QuickSave();
    }

    public void InitGameSettings()
    {
        IOError error = _storeGame.QuickLoad();
        GameSettings[] loadedGameSetting = _storeObjectT.GetLoadedObjects();
        if (error == IOError.NoError && loadedGameSetting.Length != 0)
        {
            _currentGameSettings = _storeObjectT.GetLoadedObjects()[0];
            Debug.Log(_currentGameSettings);
            //Debug.Log(_currentGameSettings.ComplexityGame);
        }
        else
        {
            ErrorLoadTopList(error);
            _currentGameSettings = _defaultGameSettings;
            Debug.Log(_currentGameSettings);
        }
        GameSettingsInitialized = true;
    }

    private void ErrorLoadTopList(IOError error)
    {
        switch (error)
        {
            case IOError.FileNotFound:
                Debug.LogWarning($"{this} : Couldn't load the {_storeGame.GetNameFile()} file, will use the default settings");
                break;
            case IOError.WrongFormat:
                Debug.LogError($"{this} : QuickLoad() - Format of the [{_storeGame.GetNameFile()}] file is wrong. Restore interrupted");
                break;
        }
    }
}
