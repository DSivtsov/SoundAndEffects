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

public enum ErrorConnecting : byte
{
    NoErrors = 0b00,
    GuestSessionNotStarted = 0b01,
    PlayerNameNotLoaded = 0b10,
    TopListNotLoaded = 0b100,
    NewNameNotSaved = 0b1000,
    NewResultNotSaved = 0b1000
}

public class LootLockerController : MonoBehaviour
{
    [SerializeField] private ConnectingToServer _connectingToServer;
    [SerializeField] private PlayerDataController _playerDataController;
    [Header("Demo Options")]
    [SerializeField] private bool _PlayOnline;
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

    private void Awake()
    {
        GuestSessionInited = false;
    }

    void Start()
    {
#if UNITY_EDITOR
        if (!_PlayOnline)
        {
            UseExistedPlayer();
        }
#else
        UseExistedPlayer();
#endif
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
            throw new NotImplementedException("PlayerPrefs doesn't have the Key(_guestPlayerIDKey)");
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
                FinishOneConnectionToServer(MultiOperation.LoadedPlayerName);
                _playerDataController.SetPlayerName(_playerNameLootLocker); 
            }
            else
                FinalizeAllServerOperations(resultOK: false, ErrorConnecting.PlayerNameNotLoaded);
        }
        else
            FinalizeAllServerOperations(resultOK: false, ErrorConnecting.GuestSessionNotStarted);
    }

    public void FinalizeAllServerOperations(bool resultOK, ErrorConnecting error = ErrorConnecting.NoErrors)
    {
        if (!resultOK)
        {
            _statusErrorConnecting |= error;
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
                Debug.Log($"{this} : Successfully started LootLocker GuestSession with {_sessionTokenLootLocker} token");
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
        //CountFrame.DebugLogUpdate(this, $"Finished the InitGuestSession()");
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
                Debug.Log($"{this} : Successfully get player name for {_playerIDLootLocker} playerIDLootLocker");
                _playerNameLootLocker = response.name;
            }
            else
            {
                Debug.LogWarning($"{this} : Error getting player name");
            }
            notGetResponse = false;
        });
        yield return new WaitWhile(() => notGetResponse);
        //CountFrame.DebugLogUpdate(this, $"Finished the GetPlayerName()");
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
                Debug.Log($"{this} : Successful : Was writen [{score}] score for Player [{_playerIDLootLocker}]");
                NewResultWasSaved = true;
            }
            else
            {
                Debug.LogError($"{this} : failed: " + response.Error);
            }
            notGetResponse = false;
        });
        yield return new WaitWhile(() => notGetResponse);
        //CountFrame.DebugLogUpdate(this, $"Finished the SendScoreToLeaderBoard()");
    }

    public IEnumerator SetPlayerName(string newName)
    {
        _newNameWasSaved = false;
        bool notGetResponse = true;
        LootLockerSDKManager.SetPlayerName(newName, (response) =>
        {
            if (response.success)
            {
                Debug.Log($"{this} : Successfully set {response.name} player name");
                _newNameWasSaved = true;
            }
            else
            {
                Debug.LogError($"{this} : Error setting player name");
            }
            notGetResponse = false;
        });
        yield return new WaitWhile(() => notGetResponse);
        //CountFrame.DebugLogUpdate(this, $"Finished the SetPlayerName() _newNameWasSaved[{_newNameWasSaved}]");
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
                Debug.Log($"{this} : Successfuly get {table.Length} records from LootLocker");
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
        CountFrame.DebugLogUpdate(this, $"Finished the GetScoreFromLeaderBoard()");
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
                Debug.Log($"{this} : Successfully {_sessionTokenLootLocker} EndSession");
            }
            else
            {
                Debug.LogWarning($"{this} : StopCurrentSession failed: " + response.Error);
            }
            notGetResponse = false;
        });
        yield return new WaitWhile(() => notGetResponse);
        //CountFrame.DebugLogUpdate(this, $"Finished the StopCurrentSession()");
    }
}

