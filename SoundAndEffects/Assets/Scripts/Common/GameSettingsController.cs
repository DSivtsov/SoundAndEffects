using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Manager;
using GMTools;
using System.IO;

public class GameSettingsController : MonoBehaviour
{
    [SerializeField] private GameSettingsSO _gameSettings;
    [SerializeField, ReadOnly] private string _nameFile = "GameSettingsSO.txt";
    [SerializeField, ReadOnly] private string _nameFileDefault = "GameSettingsSODefault.txt";
    public GameSettingsSO GameSettings => _gameSettings;
    private bool _initGameSettings = false;
    public void Load()
    {
        if (!_initGameSettings)
            return;
        if (File.Exists(_nameFile))
            OdinSerializerCalls.LoadUnityObject(_gameSettings, _nameFile);
        else
            Debug.LogWarning($"{this} : Load() : {_nameFile} not exists ");
    }

    public void Save()
    {
        if (_initGameSettings)
            OdinSerializerCalls.SaveUnityObject(_gameSettings, _nameFile);
        else
            Debug.LogWarning($"{this} : Save() : _initGameSettings == false");
    }

    public void InitGameSettings()
    {
        if (_gameSettings)
        {
            OdinSerializerCalls.SaveUnityObject(_gameSettings, _nameFileDefault);
            _initGameSettings = true;
            Load();
            CountFrame.DebugLogUpdate(this, "InitGameSettings() fnished");
        }
        else
            Debug.LogError($"{this} : InitGameSettings() : _gameSettings == null");
    }

    public void Reset()
    {
        if (_initGameSettings)
            OdinSerializerCalls.LoadUnityObject(_gameSettings, _nameFileDefault);
        else
            Debug.LogWarning($"{this} : Reset() : _initGameSettings == false");
    }
}
