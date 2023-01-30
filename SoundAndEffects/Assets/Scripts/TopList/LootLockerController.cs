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
    ScoreFromLeaderBoardNotGet = 0b100,
    NewNameNotSaved = 0b1000,
    ScoreToLeaderBoardNotSent = 0b1000,
    PlayerNameIsEmpty = 0b10_000,
    TimeOut = 0b100_000,
    SessionNotStoped = 0b1_000_000,
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
    [SerializeField] private DisplayConnectingToServer _connectingToServer;
    [SerializeField] private PlayerDataController _playerDataController;
    [SerializeField] private GameSettingsSO _gameSettings;
    [SerializeField] private MainMenusSceneManager _mainMenusSceneManager;

    /// <summary>
    /// The key of the leaderboard used
    /// </summary>
    private const string _leaderboardKey = "CityRunnerGlobalTopList";
    private string _sessionTokenLootLocker;
    private int _playerIDLootLocker;

    /// <summary>
    /// Maximum time for one server operaion, for internal detection the TimeOut error
    /// </summary>
    private const float MaximumTimePeriod = 10f;
    private bool _connecting;
    private float _startTime;
    private Coroutine _coroutineProcessCountDown;

    public bool GuestSessionInited { get; private set;} = false;
    public bool GlobalListLoaded { get; private set; }

    private void Awake() => ClearSessionData();

    public void SetDisplayOfflineModeActive() => _connectingToServer.DisplayOfflineModeActive();

    public void OpenSession()
    {
        if (_playerDataController.ExistGuestPlayerID)
        {
            StartCoroutine(OpenSessionUseExistenRecord());
        }
        else
        {
            CountFrame.DebugLogUpdate(this, $"PlayerPrefs doesn't have the Key(_guestPlayerIDKey)");
            StartCoroutine(CoroutineCreateNewGuestIDRecord());
        }
    }

    public void DisconnectedFromServer()
    {
        if (_playerDataController.ExistGuestPlayerID)
        {
            StartProcessConnecting();
            StartCoroutine(StopCurrentSession(onlyEndSession: true)); 
        }
        else
            CountFrame.DebugLogUpdate(this, $"DisconnectedFromServer() but absent the Key(_guestPlayerIDKey)");
    }

    private IEnumerator OpenSessionUseExistenRecord()
    {
        StartProcessConnecting();
        yield return OpenGuestSession(useExistedPlayerRecord: true);
    }
   
    private IEnumerator OpenGuestSession(bool useExistedPlayerRecord = true)
    {
        string player_identifier = null;
        bool notGetResponse = true;
        Action<LootLockerGuestSessionResponse> onComplete = (response) =>
        {
            if (response.success)
            {
                _sessionTokenLootLocker = response.session_token;
                _playerIDLootLocker = response.player_id;
                player_identifier = response.player_identifier;
                GuestSessionInited = true;
            }
            notGetResponse = false;
        };
        if (useExistedPlayerRecord)
            LootLockerSDKManager.StartGuestSession(_playerDataController.Player.GuestPlayerID, (response) => onComplete(response));
        else
            LootLockerSDKManager.StartGuestSession((response) => onComplete(response)); 
        yield return new WaitWhile(() => notGetResponse && _connecting);
        if (GuestSessionInited)
        {
            if (useExistedPlayerRecord)
                CheckResultsServerOperations();
            else
            {
                //In this Case the process is not finished yet
                _playerDataController.Player.SetGuestPlayerID(player_identifier);
                Debug.LogWarning($"SetGuestPlayerID(response.player_identifier)[{player_identifier}]");
            }
        }
        else
        {
            Debug.Log($"{this} : ErrorConnecting.GuestSessionNotStarted");
            CheckResultsServerOperations(ErrorConnecting.GuestSessionNotStarted);
        }
    }

    public IEnumerator CoroutineSaveScoreToLeaderBoard(int score)
    {
        if (GuestSessionInited)
        {
            StartProcessConnecting();
            bool NewResultWasSaved = false;
            bool notGetResponse = true;
            LootLockerSDKManager.SubmitScore(_playerIDLootLocker.ToString(), score, _leaderboardKey, (response) =>
            {
                if (response.success)
                    NewResultWasSaved = true;
                notGetResponse = false;
            });
            yield return new WaitWhile(() => notGetResponse && _connecting);
            if (NewResultWasSaved)
                CheckResultsServerOperations();
            else
                CheckResultsServerOperations(ErrorConnecting.ScoreToLeaderBoardNotSent);
        }
        else
            CountFrame.DebugLogUpdate(this, $"CoroutineSaveScoreToLeaderBoard Canceled - GuestSession not inited");
    }

    public IEnumerator CoroutineGetScoreFromLeaderBoard(List<PlayerData> playerDatas, Action callbackShowTopList, int count = 10, int after = 0)
    {
        if (GuestSessionInited)
        {
            StartProcessConnecting();
            GlobalListLoaded = false;
            bool notGetResponse = true;
            LootLockerLeaderboardMember[] table = default(LootLockerLeaderboardMember[]);
            LootLockerSDKManager.GetScoreList(_leaderboardKey, count, after, (response) =>
            {
                if (response.statusCode == 200)
                {
                    table = response.items;
                }
                notGetResponse = false;
            });
            yield return new WaitWhile(() => notGetResponse && _connecting);
            if (table != null)
            {
                CheckResultsServerOperations();
                for (int i = 0; i < table.Length; i++)
                {
                    //Debug.Log($"i[{i + 1}] player.id={table[i].player.id} player.name={table[i].player.name} score={table[i].score} player.public_uid={table[i].player.public_uid}");
                    playerDatas.Add(new PlayerData(table[i].player.name, 0, table[i].score));
                }
                GlobalListLoaded = true;
                callbackShowTopList(); 
            }
            else
            {
                //!!!In this case will show previous state of GlobalTopList
                CheckResultsServerOperations(ErrorConnecting.ScoreFromLeaderBoardNotGet);
                Debug.LogWarning("TIME SOLUTION");
                ClearSessionData();
            }
        }
        else
            CountFrame.DebugLogUpdate(this, $"CoroutineGetScoreFromLeaderBoard Canceled - GuestSession not inited");
    }

    public IEnumerator CoroutineCreateNewGuestIDRecord()
    {
        StartProcessConnecting();
        if (_playerDataController.ExistGuestPlayerID)
        {
            yield return StopCurrentSession();
            if (!GuestSessionInited)
            {
                CountFrame.DebugLogUpdate(this, $"CoroutineCreateNewGuestIDRecord Canceled - StopCurrentSession not finished successfully");
                yield break;
            }
            //BackUp and Remove the current Player Record
            _playerDataController.Player.BackUpCurrentPlayerRecord();
            _playerDataController.Player.DeleteCurrentGuestPlayerID();
        }
        yield return OpenGuestSession(useExistedPlayerRecord: false);
        if (GuestSessionInited)
        {
            yield return SetPlayerNameForPlayerRecord();
        }
        else
        {
            CountFrame.DebugLogUpdate(this, $"CoroutineCreateNewGuestIDRecord Canceled - GuestSession for New Player not inited");
        }
    }

    public IEnumerator SetPlayerNameForPlayerRecord()
    {
        bool NewNameSaved = false;
        bool notGetResponse = true;
        LootLockerSDKManager.SetPlayerName(_playerDataController.Player.Name, (response) =>
        {
            if (response.success)
            {
                NewNameSaved = true;
            }
            notGetResponse = false;
        });
        yield return new WaitWhile(() => notGetResponse && _connecting);
        if (NewNameSaved)
        {
            CheckResultsServerOperations();
        }
        else
        {
            CheckResultsServerOperations(ErrorConnecting.NewNameNotSaved);
            CreationPlayerNotFinished();
        }
    }

    private void CreationPlayerNotFinished()
    {
        //Very Important Case: Record was created in LootLocker, but not set the coresponded Player Name
        Debug.LogError("!!!Called _playerDataController.Player.DeleteCurrentGuestPlayerID()");
        _playerDataController.Player.DeleteCurrentGuestPlayerID();
    }

    private IEnumerator StopCurrentSession(bool onlyEndSession = false)
    {
        if (GuestSessionInited && _sessionTokenLootLocker != null)
        {
            string responseError = null;
            bool sessionStoped = false;
            bool notGetResponse = true;
            LootLockerSDKManager.EndSession(_sessionTokenLootLocker, (response) =>
            {
                if (response.success)
                    sessionStoped = true;
                else
                    responseError = response.Error;
                notGetResponse = false;
            });
            yield return new WaitWhile(() => notGetResponse && _connecting);
            if (sessionStoped)
            {
                ClearSessionData();
                if (onlyEndSession)
                {
                    StopCoroutine(_coroutineProcessCountDown);
                    _mainMenusSceneManager.SetStatusConnectionToServer(isConnected: false);
                    _connectingToServer.SetResultsGoingToOffLineMode(); 
                }
            }
            else
            {
                CheckResultsServerOperations(ErrorConnecting.SessionNotStoped);
                Debug.LogWarning($"{this} : StopCurrentSession failed: " + responseError);
            }
        }
        else
            Debug.LogWarning($"{this} : Exist the _guestPlayerIDKey key, but active session is absent");
    }

    private void ClearSessionData()
    {
        GuestSessionInited = false;
        _sessionTokenLootLocker = null;
        _playerIDLootLocker = 0;
    }

    public void CheckResultsServerOperations(ErrorConnecting finalStatus = ErrorConnecting.NoErrors)
    {
        StopCoroutine(_coroutineProcessCountDown);

        //To combine ErrorConnecting.TimeOut with other Error if it occurs
        // To have possibility to finish w/o error if good response will be in the one frame with Time.out
        // awating must check the server response and _connecting status, but result check on value of the server response 
        // here if _connecting = false the error in response in other case the ErrorConnecting.TimeOut error
        if (finalStatus != ErrorConnecting.NoErrors)
            finalStatus = (_connecting) ? finalStatus : ErrorConnecting.TimeOut | finalStatus;

        CountFrame.DebugLogUpdate(this, $"finalStatus Connecting=[{finalStatus}]");
        bool statusConnectionNoErrors = finalStatus == ErrorConnecting.NoErrors;
        if (!statusConnectionNoErrors)
            ClearSessionData();
        _mainMenusSceneManager.SetStatusConnectionToServer(isConnected: statusConnectionNoErrors);
        _connectingToServer.DisplayResult(resultOK: statusConnectionNoErrors, _startTime);
    }

    private void StartProcessConnecting()
    {
        _connectingToServer.StartAnimateProcessConnecting();
        _connecting = true;
        _startTime = Time.time;
        _coroutineProcessCountDown = StartCoroutine(CoroutineProcessCountDown());
    }

    private IEnumerator CoroutineProcessCountDown(float currentMaximumPeriod = MaximumTimePeriod)
    {
        do
        {
            yield return null;
            if (Time.time - _startTime > currentMaximumPeriod)
            {
                CountFrame.DebugLogUpdate(this, $"CoroutineAnimateProcessConnecting(): canceled connection because time is more than {currentMaximumPeriod}");
                _connecting = false;
            }
        } while (_connecting);
    }
}

