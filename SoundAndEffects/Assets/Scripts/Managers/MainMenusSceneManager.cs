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
    public void ActivateButtonStart(bool activate) => _buttonStart.interactable = activate;
    public void StartGame() => _mainManager?.FromMenusToStartGame(_playerDataController.Player.Name);
    public void ActivateMainMenusCamera(bool activate) => cameraMainMenus.SetActive(activate);
    public void SetStatusConnectionToServer(bool isConnected) => IsConnectedToServer = isConnected;

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

        _playerDataController.LoadLastPlayerAccount();

        if (_playerDataController.PlayerAccountInited)
        {
            if (_gameSettings.FieldPlayMode.GetCurrentValue() == PlayMode.Offline)
                _lootLockerController.SetDisplayOfflineModeActive();
            else
                CheckPlayModeAndStateConnectionToServer();
            ActivateButtonStart(true); 
        }
        else
            if (_gameSettings.FieldPlayMode.GetCurrentValue() == PlayMode.Online)
            _lootLockerController.SetDisplayOnineModeNotActive();

        //Introduction();
    }
    /// <summary>
    /// ManageConnectionToServer based on value PlayerMode and IsConnectedToServer
    /// </summary>
    public void CheckPlayModeAndStateConnectionToServer()
    {
        if (_playerDataController.PlayerAccountInited)
        {
            if (!IsConnectedToServer && _gameSettings.FieldPlayMode.GetCurrentValue() == PlayMode.Online)
                _lootLockerController.OpenSession();
            if (IsConnectedToServer && _gameSettings.FieldPlayMode.GetCurrentValue() == PlayMode.Offline)
                _lootLockerController.DisconnectedFromServer(); 
        }
    }

    public void CreateNewPlayer()
    {
        ActivateButtonStart(false);
        //After calling  CreateNewPlayerAccount() user can't skip creation New Player
        _playerDataController.CreateNewPlayerAccount(() =>
        {
            if (_gameSettings.FieldPlayMode.GetCurrentValue() == PlayMode.Online)
                CreateLootLockerRecordNewPlayerAccount();
            else
                _playerDataController.Player.SaveToRegistry();
            ActivateButtonStart(true);
        });
    }

    /// <summary>
    /// It will call in Online mode only
    /// </summary>
    /// <param name="playerName"></param>
    public void CreateLootLockerRecordNewPlayerAccount()
    {
        CountFrame.DebugLogUpdate(this, $"IsConnectedToServer[{IsConnectedToServer}]: Request to create NewPlayerLootLocker record");
        if (IsConnectedToServer)
            StartCoroutine(_lootLockerController.CoroutineCreateNewGuestIDRecord(isExistOpenSession: true));
        else
            StartCoroutine(_lootLockerController.CoroutineCreateNewGuestIDRecord(isExistOpenSession: false));
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
        if (!_gameSettings.FieldNotShowIntroductionText.GetCurrentValue())
        {
            _modalWindowsIntroduction.SetActionBeforeDeactivationModalWindow(() =>
            {
                _toggleBoolNShowIntro.SetNotShowIntroductionWindows();
                GameSettingsSOController.Instance.SaveCustomGameSettings();
            });
            _modalWindowsIntroduction.ActivateCanvasOverlayWindow();
        }
    }
}
