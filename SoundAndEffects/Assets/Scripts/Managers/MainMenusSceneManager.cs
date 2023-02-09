using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Menu;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MainMenusSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject cameraMainMenus;
    [SerializeField] private LocalTopListController _localTopListController;
    [SerializeField] private GlobalTopListController _remoteTopListController;
    [SerializeField] private LootLockerController _lootLockerController;
    [SerializeField] private PlayerDataController _playerDataController;
    [SerializeField] private Button _buttonStart;
    [SerializeField] private GameSettingsSO _gameSettings;

    private MainManager _mainManager;
    public bool IsConnectedToServer { get; private set; } = false;

    private void Awake()
    {
        CountFrame.DebugLogUpdate(this, $"Awake()");
        _mainManager = MainManager.Instance;
        ButtonActions.LinkMenuSceneManager(this);
        if (_mainManager)
        {
            _mainManager.LinkMenuSceneManager(this);
            //Camera will manage by GameMainManager
            ActivateMainMenusCamera(false);
        }
        else
        {
            Debug.LogError($"{this} not linked to GameMainManager");
            ActivateMainMenusCamera(true);
        }
        _buttonStart.interactable = false;
    }

    private void Start()
    {
        _localTopListController.LoadAndShow();
        _playerDataController.InitPlayerAccount();
        CheckPlayerMode();
        if (_gameSettings.FieldPlayMode.GetCurrentValue() == PlayMode.Offline)
            _lootLockerController.SetDisplayOfflineModeActive();
    }
    /// <summary>
    /// Check PlayerMode and if it switched to Online then load remote data
    /// </summary>
    public void CheckPlayerMode()
    {
        if (_playerDataController.Player != null)
        {
            //Debug.Log($"{this}: IsConnectedToServer[{IsConnectedToServer}] FieldPlayMode[{_gameSettings.FieldPlayMode.GetCurrentValue()}]");
            if (!IsConnectedToServer && _gameSettings.FieldPlayMode.GetCurrentValue() == PlayMode.Online)
                 _lootLockerController.OpenSession();
            if (IsConnectedToServer && _gameSettings.FieldPlayMode.GetCurrentValue() == PlayMode.Offline)
                _lootLockerController.DisconnectedFromServer();
        }
        else
            Debug.LogWarning($"{this} : _player == null");
    }

    public void ActivateButtonStart(bool activate) => _buttonStart.interactable = activate;

    public void StartGame(string playerName) => _mainManager?.FromMenusToStartGame(playerName);

    public void ResetTopList() => _localTopListController.ResetTopList();

    /// <summary>
    /// If Online mode is active will be create a new Player in the LootLocker
    /// </summary>
    /// <param name="playerName"></param>
    public void CreateNewPlayerLootLocker()
    {
        if (IsConnectedToServer)
        {
            StartCoroutine(_lootLockerController.CoroutineCreateNewGuestIDRecord());
        }
        else
        {
            Debug.LogWarning($"{this}: Not Create NewPlayerLootLocker record in Offline mode");
        }
    }

    public void TryReconnect() => CheckPlayerMode();

    public void ActivateMainMenusCamera(bool activate)
    {
        cameraMainMenus.SetActive(activate);
    }

    public void AddNewCharacterData(PlayerData newCharacterData)
    {
        Debug.Log(newCharacterData);
        _localTopListController.AddCharacterResult(newCharacterData);
        _remoteTopListController.AddCharacterResult(newCharacterData);
    }

    public void SetStatusConnectionToServer(bool isConnected) => IsConnectedToServer = isConnected;

    public void TemporaryDisableMouse(bool setDiabled = true)
    {
        if (setDiabled)
        {
            InputSystem.DisableDevice(Mouse.current); 
        }
        else
        {
            InputSystem.EnableDevice(Mouse.current);
        }
    }
}
