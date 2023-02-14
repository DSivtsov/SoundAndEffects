using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class MainMenusSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject cameraMainMenus;
    [SerializeField] private LocalTopListController _localTopListController;
    [SerializeField] private GlobalTopListController _remoteTopListController;
    [SerializeField] private LootLockerController _lootLockerController;
    [SerializeField] private PlayerDataController _playerDataController;
    [SerializeField] private ModalWindowOK _modalWindowsResetTopList;
    [SerializeField] private ModalWindowCloseWithToggle _modalWindowsIntroduction;
    [SerializeField] private Button _buttonStart;
    [SerializeField] private GameSettingsSO _gameSettings;
    [SerializeField] private ToggleBoolNShowIntro _toggleBoolNShowIntro;

    private MainManager _mainManager;
    public bool IsConnectedToServer { get; private set; } = false;

    private bool _notInitedManageConnectionToServer;

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
        ActivateButtonStart(false);
    }

    private void Start()
    {
        _localTopListController.LoadAndShow();
        _notInitedManageConnectionToServer = true;
        if (_playerDataController.InitPlayerAccount())
        {
            ManageConnectionToServer();
            ActivateButtonStart(true);
        }
        if (!_gameSettings.FieldNotShowIntroductionText.GetCurrentValue())
        {
            Introduction();
        }
    }
    /// <summary>
    /// ManageConnectionToServer based on value PlayerMode and IsConnectedToServer
    /// </summary>
    public void ManageConnectionToServer()
    {
        if (_notInitedManageConnectionToServer && _gameSettings.FieldPlayMode.GetCurrentValue() == PlayMode.Offline)
        {
            _lootLockerController.SetDisplayOfflineModeActive();
        }
        else
        {
                if (!IsConnectedToServer && _gameSettings.FieldPlayMode.GetCurrentValue() == PlayMode.Online)
                    _lootLockerController.OpenSession();
                if (IsConnectedToServer && _gameSettings.FieldPlayMode.GetCurrentValue() == PlayMode.Offline)
                    _lootLockerController.DisconnectedFromServer();
        }
        _notInitedManageConnectionToServer = false;
    }

    public void CreateNewPlayer()
    {
        ActivateButtonStart(false);
        _playerDataController.ActivateCreateNewPlayer(() =>
        {
            CreateNewPlayerLootLocker();
            ActivateButtonStart(true);
        });
    }

    public void ActivateButtonStart(bool activate) => _buttonStart.interactable = activate;

    public void StartGame() => _mainManager?.FromMenusToStartGame(_playerDataController.Player.Name);

    /// <summary>
    /// If Online mode is active will try to create a new Player in the LootLocker, in Offline creation will be skipped until switching to Online
    /// </summary>
    /// <param name="playerName"></param>
    public void CreateNewPlayerLootLocker()
    {
        if (IsConnectedToServer)
        {
            //In case if Session is open will call CreateNewGuestIDRecord directly
            StartCoroutine(_lootLockerController.CoroutineCreateNewGuestIDRecord());
        }
        else
        {
            //In case Online mode is active will try to openSession and CreateNewGuestIDRecord
            CountFrame.DebugLogUpdate(this, $"IsConnectedToServer[{IsConnectedToServer}]: Request to create NewPlayerLootLocker record");
            ManageConnectionToServer();
        }
    }

    public void TryReconnect() => ManageConnectionToServer();

    public void ActivateMainMenusCamera(bool activate)
    {
        cameraMainMenus.SetActive(activate);
    }

    public void AddNewCharacterData(PlayerData newCharacterData)
    {
        CountFrame.DebugLogUpdate(this, $"{newCharacterData}]");
        _localTopListController.AddCharacterResult(newCharacterData);
        if (!_gameSettings.FieldNotCopyToGlobal.GetCurrentValue())
        {
            _remoteTopListController.AddCharacterResult(newCharacterData); 
        }
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

    public void ResetTopList()
    {
        _modalWindowsResetTopList.SetActionBeforeDeactivationModalWindow(() => _localTopListController.ResetTopList());
        _modalWindowsResetTopList.ActivateCanvasOverlayWindow();
    }

    public void Introduction()
    {
        _modalWindowsIntroduction.SetActionBeforeDeactivationModalWindow(() =>
            {
                _toggleBoolNShowIntro.SetNotShowIntroductionWindows();
                GameSettingsSOController.Instance.SaveCustomGameSettings();
            });
        _modalWindowsIntroduction.ActivateCanvasOverlayWindow();
    }
}
