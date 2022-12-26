using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Manager;
using GMTools;
using System.IO;
using UnityEngine.Audio;

[Flags]
public enum GameSettingSOState
{
    UseCustomSaved = 0b01,
    ExistCustomSaved = 0b10,
    NotSavedChanges = 0b11,
}
/*
 * Always load gamesettings from stored files
 * Load Custom game setting if it exists or Load Default game setting in other case
 * If absent the default game settings files it will create from GameSettingsSO
 * The values in AudioMixer always override by game settings
 */
public class GameSettingsSOController : SingletonController<GameSettingsSOController>
{
    [SerializeField] private GameSettingsSO _gameSettings;
    [SerializeField] private AudioContoller _audioContoller;
    [SerializeField, ReadOnly] private string _nameFile = "GameSettingsSO.txt";
    [SerializeField, ReadOnly] private string _nameFileDefault = "GameSettingsSODefault.txt";

    public event Action UpdateElementFromFields;
    public event Action UpdateGameSettingsControlButtons;

    public bool GameSettingsInited { get; private set; } = false;

    public bool ExistCustomSavedSettings { get; private set; } = false;
    public bool ExistNotSavedChanges { get; private set; } = false;

    private FlagGameSettingChanged _flagGameSettingChanges = new FlagGameSettingChanged();

    protected override void Awake()
    {
        base.Awake();

        ButtonActions.LinkGameSettingsSOController(this);
        _flagGameSettingChanges.ExistChangesAfterFlagUpdated += (bool flagExistNotSavedChanges) =>
        {
            ExistNotSavedChanges = flagExistNotSavedChanges;
            UpdateGameSettingsControlButtons?.Invoke();
        };
    }
    public void UpdateInitValues()
    {
        ExposeFieldBase.UpdateInitValue();
        _flagGameSettingChanges.SetNoChanges();
    }

    public void InitGameSettings()
    {
        if (_gameSettings && _audioContoller)
        {
            SaveDefaultGameSettings();
            _gameSettings.InitExposedFields(_flagGameSettingChanges, _audioContoller);
            if (File.Exists(_nameFile))
            {
                LoadCustomGameSettings(); 
            }
            else
            {
                LoadDefaultGameSettings();
            }
            _audioContoller.InitAudioByValGameSettings();
            GameSettingsInited = true;
            CountFrame.DebugLogUpdate(this, "InitGameSettings() finished");
        }
        else
            Debug.LogError($"{this} : InitGameSettings() : _gameSettings == null or _audioContoller == null");
    }

    private void SaveDefaultGameSettings()
    {
        if (!File.Exists(_nameFileDefault))
        {
            OdinSerializerCalls.SaveUnityObject(_gameSettings, _nameFileDefault);
        }
    }

    public void ShowData() => Debug.Log(_gameSettings);

    public void LoadCustomGameSettings()
    {
        OdinSerializerCalls.LoadUnityObject(_gameSettings, _nameFile);
        ExistCustomSavedSettings = true;
        UpdateElementFromFields?.Invoke();
        UpdateInitValues();
    }

    public void SaveCustomGameSettings()
    {
        OdinSerializerCalls.SaveUnityObject(_gameSettings, _nameFile);
        ExistCustomSavedSettings = true;
        UpdateInitValues();
    }

    public void LoadDefaultGameSettings()
    {
        OdinSerializerCalls.LoadUnityObject(_gameSettings, _nameFileDefault);
        if (File.Exists(_nameFile)) File.Delete(_nameFile);
        ExistCustomSavedSettings = false;
        UpdateElementFromFields?.Invoke();
        UpdateInitValues();
    }
}
