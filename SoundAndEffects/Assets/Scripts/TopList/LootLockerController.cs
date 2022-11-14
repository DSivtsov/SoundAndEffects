using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using System;
using System.Linq;

[Flags]
public enum MultiOperation: byte
{
    NothingLoaded = 0b00,
    LoadedTopList = 0b01,
    LoadedPlayerName = 0b10,
    LoadedAll = 0b11,
}
[Flags]
public enum ErrorConnecting : byte
{
    NoErrors = 0b00,
    GuestSessionNotStarted = 0b01,
    PlayerNameNotLoaded = 0b10,
    TopListNotLoaded = 0b100,
    NewNameNotSaved = 0b1000,
    NewResultNotSaved = 0b1000,
    PlayerNameIsEmpty = 0b10000
}
[Serializable]
public enum PlayMode// : byte
{
    //NoInitialized = 0,
    Online = 1,
    Offline = 2
}

public class LootLockerController : MonoBehaviour
{
    [SerializeField] private ConnectingToServer _connectingToServer;
    [SerializeField] private PlayerDataController _playerDataController;
    [SerializeField] private GameSettingsSO _gameSettings;

    /// <summary>
    /// The standart key PlayerPref used by LootLocker
    /// </summary>
    private const string _guestPlayerIDKey = "LootLockerGuestPlayerID";
    // Computer\HKEY_CURRENT_USER\SOFTWARE\DefaultCompany\SoundAndEffects for Build
    // CComputer\HKEY_CURRENT_USER\SOFTWARE\Unity\UnityEditor\DefaultCompany\SoundAndEffects for Player in Editor
    //private const int _leaderboardID = 5374;
    /// <summary>
    /// The key of the leaderboard used
    /// </summary>
    private const string _leaderboardKey = "CityRunnerGlobalTopList";

    private string _playerIdentifierLootLocker;
    private string _sessionTokenLootLocker;
    private int _playerIDLootLocker;
    public bool GuestSessionInited { get; private set;}
    public bool GlobalListLoaded { get; private set; }
    private string _playerNameLootLocker;
    private Coroutine _coroutineProcessConnecting;
    private MultiOperation _statusFinishedConnecting;
    private ErrorConnecting _statusErrorConnecting;
    private bool _newNameWasSaved;
    public bool NewResultWasSaved { get; private set; }
    public PlayMode CurrentPlayMode { get; private set; }
    //public PlayMode SelectedPlayMode { get; private set; }

    private void Awake()
    {
//#if UNITY_EDITOR
//        if (_gameSettings.UsedPlayMode == PlayMode.Online)
//            SelectedPlayMode = PlayMode.Online;
//        else
//            SelectedPlayMode = PlayMode.Offline;
//#else
//        SelectedPlayMode = PlayMode.Online;
//#endif
        GuestSessionInited = false;
        if (_gameSettings.UsedPlayMode == PlayMode.Offline)
        {
            CurrentPlayMode = PlayMode.Offline;
            _connectingToServer.OfflineMode();
            CountFrame.DebugLogUpdate(this, $" Start() : _currentPlayMode set to Offline");
        }
        else
            CurrentPlayMode = PlayMode.Online;
    }

    void Start()
    {
        if (CurrentPlayMode == PlayMode.Offline)
        {
            _playerDataController.GetPlayerNameFromLocalStorage();
        }
        else
        {
            UseExistedPlayer();
        }
    }

    private void UseExistedPlayer()
    {
        if (PlayerPrefs.HasKey(_guestPlayerIDKey))
        {
            BeginConnectingToServer();
            _playerIdentifierLootLocker = PlayerPrefs.GetString(_guestPlayerIDKey);
            StartCoroutine(InitExistenPlayerRecord());
        }
        else
        {
            CountFrame.DebugLogUpdate(this, $"PlayerPrefs doesn't have the Key(_guestPlayerIDKey)");
            //throw new NotImplementedException("PlayerPrefs doesn't have the Key(_guestPlayerIDKey)");
        }
    }

    private void BeginConnectingToServer()
    {
        _statusErrorConnecting = ErrorConnecting.NoErrors;
        _statusFinishedConnecting = MultiOperation.NothingLoaded;
        _coroutineProcessConnecting = StartCoroutine(_connectingToServer.CoroutineProcessConnecting());
    }

    private IEnumerator InitExistenPlayerRecord()
    {
        yield return InitGuestSession(useExistedPlayerRecord: true);
        if (GuestSessionInited)
        {
            yield return GetPlayerName();
            //All players will have a nickname
            if (_playerNameLootLocker != null)
            {
                if ( _playerNameLootLocker.Length != 0)
                {
                    FinishOneConnectionToServer(MultiOperation.LoadedPlayerName);
                    _playerDataController.SetPlayerNameAndUpdateMenuScene(_playerNameLootLocker);  
                }
                else
                    FinalizeAllServerOperations(resultOK: false, ErrorConnecting.PlayerNameIsEmpty);
            }
            else
                FinalizeAllServerOperations(resultOK: false, ErrorConnecting.PlayerNameNotLoaded);
        }
        else
            FinalizeAllServerOperations(resultOK: false, ErrorConnecting.GuestSessionNotStarted);
    }

    public void FinalizeAllServerOperations(bool resultOK, ErrorConnecting error = ErrorConnecting.NoErrors)
    {
        if (resultOK)
            CurrentPlayMode = PlayMode.Online;
        else
        {
            _statusErrorConnecting |= error;
            CurrentPlayMode = PlayMode.Offline;
            _connectingToServer.OfflineMode();
        }
        StopCoroutine(_coroutineProcessConnecting);
        _connectingToServer.Result(resultOK);

    }

    public void FinishOneConnectionToServer(MultiOperation result)
    {
        if (_statusFinishedConnecting.Equals(MultiOperation.NothingLoaded))
        {
            _statusFinishedConnecting = result;
            StartCoroutine(InitWaitingFinishingConnectionsToServer()); 
        }
        else
            _statusFinishedConnecting |= result;
    }

    private IEnumerator InitWaitingFinishingConnectionsToServer()
    {
        while (_connectingToServer.Connecting && !_statusFinishedConnecting.HasFlag(MultiOperation.LoadedAll))
        {
            yield return null;
        }
        FinalizeAllServerOperations(resultOK: true);
    }
   
    private IEnumerator InitGuestSession(bool useExistedPlayerRecord = true)
    {
        bool notGetResponse = true;
        Action<LootLockerGuestSessionResponse> onComplete = (response) =>
        {
            //Debug.Log($"Current state notGetResponse == true [{notGetResponse == true}] before StartGuestSession");
            if (response.success)
            {
                _sessionTokenLootLocker = response.session_token;
                _playerIDLootLocker = response.player_id;
                GuestSessionInited = true;
                //Debug.Log($"{this} : Successfully started LootLocker GuestSession with {_sessionTokenLootLocker} token");
            }
            else
            {
                Debug.Log($"{this} : Error starting LootLocker GuestSession");
            }
            notGetResponse = false;
        };
        if (useExistedPlayerRecord)
            LootLockerSDKManager.StartGuestSession(_playerIdentifierLootLocker, (response) => onComplete(response));
        else
            LootLockerSDKManager.StartGuestSession((response) => onComplete(response));
        yield return new WaitWhile(() => notGetResponse);
    }

    private IEnumerator GetPlayerName()
    {
        bool notGetResponse = true;
        //used also for check the result of request
        _playerNameLootLocker = null;
        LootLockerSDKManager.GetPlayerName((response) =>
        {
            if (response.success)
            {
                _playerNameLootLocker = response.name;
                //Debug.Log($"{this} : Successfully get [{_playerNameLootLocker}] Player name for {_playerIDLootLocker} playerIDLootLocker");
            }
            else
            {
                Debug.LogWarning($"{this} : Error getting player name");
            }
            notGetResponse = false;
        });
        yield return new WaitWhile(() => notGetResponse);
    }

    public IEnumerator SendScoreToLeaderBoard(int score)
    {
        BeginConnectingToServer();

        NewResultWasSaved = false;
        bool notGetResponse = true;
        LootLockerSDKManager.SubmitScore(_playerIDLootLocker.ToString(), score, _leaderboardKey, (response) =>
        {
            if (response.success)
            {
                //Debug.Log($"{this} : Successful : Was writen [{score}] score for Player [{_playerIDLootLocker}]");
                NewResultWasSaved = true;
            }
            else
            {
                Debug.LogError($"{this} : failed: " + response.Error);
            }
            notGetResponse = false;
        });
        yield return new WaitWhile(() => notGetResponse);
    }

    public IEnumerator SetPlayerName(string newName)
    {
        _newNameWasSaved = false;
        bool notGetResponse = true;
        LootLockerSDKManager.SetPlayerName(newName, (response) =>
        {
            if (response.success)
            {
                //Debug.Log($"{this} : Successfully set {response.name} player name");
                _newNameWasSaved = true;
            }
            else
            {
                Debug.LogError($"{this} : Error setting player name");
            }
            notGetResponse = false;
        });
        yield return new WaitWhile(() => notGetResponse);
    }

    public IEnumerator CoroutineGetScoreFromLeaderBoard(List<PlayerData> playerDatas, int count = 10, int after = 0)
    {
        GlobalListLoaded = false;
        bool notGetResponse = true;
        LootLockerLeaderboardMember[] table = default(LootLockerLeaderboardMember[]);
        LootLockerSDKManager.GetScoreList(_leaderboardKey, count, after, (response) =>
        {
            if (response.statusCode == 200)
            {
                table = response.items;
                //Debug.Log($"{this} : Successfuly get {table.Length} records from LootLocker");
            }
            else
            {
                Debug.LogError($"{this} : failed: " + response.Error);
            }
            notGetResponse = false;
        });
        yield return new WaitWhile(() => notGetResponse);
        for (int i = 0; i < table.Length; i++)
        {
            //Debug.Log($"i[{i + 1}] player.id={table[i].player.id} player.name={table[i].player.name} score={table[i].score} player.public_uid={table[i].player.public_uid}");
            playerDatas.Add(new PlayerData(table[i].player.name, 0, table[i].score));
        }
        GlobalListLoaded = true;
        //CountFrame.DebugLogUpdate(this, $"Finished the GetScoreFromLeaderBoard()");
    }

    public void CreateNewPlayerRecord(string playerName)
    {
        BeginConnectingToServer();
        StartCoroutine(CoroutineCreateNewPlayerRecord(playerName));
    }
    public IEnumerator CoroutineCreateNewPlayerRecord(string playerName)
    {
        //BackUp and Remove the previous Player Record
        if (PlayerPrefs.HasKey(_guestPlayerIDKey))
        {
            BackUpOldPlayerRecord();
            if (_sessionTokenLootLocker != null)
            {
                if (GuestSessionInited)
                {
                    yield return StopCurrentSession();
                    //Because the result of StopCurrentSession not so important it will not checking
                }
            }
            else
                Debug.LogWarning($"{this} : Exist the _guestPlayerIDKey key, but active session is absent");
            GuestSessionInited = false;
            _sessionTokenLootLocker = null;
            _playerIDLootLocker = 0;
            PlayerPrefs.DeleteKey(_guestPlayerIDKey);
        }
        yield return InitGuestSession(useExistedPlayerRecord: false);
        if (GuestSessionInited)
        {
            yield return SetPlayerName(playerName);
            if (_newNameWasSaved)
            {
                FinalizeAllServerOperations(resultOK: true);
            }
            else
                FinalizeAllServerOperations(resultOK: false, ErrorConnecting.NewNameNotSaved);
        }
        else
        {
            FinalizeAllServerOperations(resultOK: false, ErrorConnecting.GuestSessionNotStarted);
        }
    }

    private void BackUpOldPlayerRecord() => Debug.LogError($"Not Realized BackUpOldPlayer ({_playerIdentifierLootLocker})");

    private IEnumerator StopCurrentSession()
    {
        bool notGetResponse = true;
        LootLockerSDKManager.EndSession(_sessionTokenLootLocker, (response) =>
        {
            if (response.success)
            {
                //Debug.Log($"{this} : Successfully {_sessionTokenLootLocker} EndSession");
            }
            else
            {
                Debug.LogWarning($"{this} : StopCurrentSession failed: " + response.Error);
            }
            notGetResponse = false;
        });
        yield return new WaitWhile(() => notGetResponse);
    }
}

