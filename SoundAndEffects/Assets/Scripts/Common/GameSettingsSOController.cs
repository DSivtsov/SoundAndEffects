using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Manager;
using GMTools;
using System.IO;

[Flags]
public enum GameSettingSOState
{
    UseCustomSaved = 0b01,
    ExistCustomSaved = 0b10,
    NotSavedChanges = 0b11,
}

public class GameSettingsSOController : SingletonController<GameSettingsSOController>
{
    [SerializeField] private GameSettingsSO _gameSettings;
    [SerializeField, ReadOnly] private string _nameFile = "GameSettingsSO.txt";
    [SerializeField, ReadOnly] private string _nameFileDefault = "GameSettingsSODefault.txt";

    public event Action UpdateElementFromFields;
    public event Action UpdateGameSettingsControlButtons;

    private bool _initGameSettings = false;

    //public bool UseCustomSavedSettings { get; private set; } = false;
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
    public void UpdateInitValues() => ExposeFieldBase.UpdateInitValue();

    public void InitGameSettings()
    {
        if (_gameSettings)
        {
            OdinSerializerCalls.SaveUnityObject(_gameSettings, _nameFileDefault);
            _initGameSettings = true;
            Load();
            _gameSettings.InitExposedFields(_flagGameSettingChanges);
            CountFrame.DebugLogUpdate(this, "InitGameSettings() finished");
        }
        else
            Debug.LogError($"{this} : InitGameSettings() : _gameSettings == null");
    }

    public void ShowData() => Debug.Log(_gameSettings);

    public void Load()
    {
        if (!_initGameSettings)
            return;
        if (File.Exists(_nameFile))
        {
            OdinSerializerCalls.LoadUnityObject(_gameSettings, _nameFile);
            //UseCustomSavedSettings = true;
            ExistCustomSavedSettings = true;
        }
        else
        {
            Debug.LogWarning($"{this} : Load() : {_nameFile} not exists "); 
        }
        UpdateElementFromFields?.Invoke();
        UpdateInitValues();
        _flagGameSettingChanges.SetNoChanges();
    }

    public void Save()
    {
        if (_initGameSettings)
        {
            OdinSerializerCalls.SaveUnityObject(_gameSettings, _nameFile);
            //UseCustomSavedSettings = true;
            ExistCustomSavedSettings = true;
            UpdateInitValues();
            _flagGameSettingChanges.SetNoChanges();
        }
        else
            Debug.LogWarning($"{this} : Save() : _initGameSettings == false");
    }


    public void LoadDefault()
    {
        if (_initGameSettings)
        {
            OdinSerializerCalls.LoadUnityObject(_gameSettings, _nameFileDefault);
            try
            {
                File.Delete(_nameFile);
            }
            catch (Exception)
            {
                Debug.LogWarning($"{this} : LoadDefault() : Can't delete {_nameFile} file");
            }
            ExistCustomSavedSettings = false;
            //UseCustomSavedSettings = false;
            UpdateElementFromFields?.Invoke();
            UpdateInitValues();
            _flagGameSettingChanges.SetNoChanges();
        }
        else
            Debug.LogWarning($"{this} : LoadDefault() : _initGameSettings == false");
    }
}
