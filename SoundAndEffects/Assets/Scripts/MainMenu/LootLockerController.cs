using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using System;
using System.Linq;

public class LootLockerController : MonoBehaviour
{
    //[SerializeField] private MainMenusSceneManager _mainMenusSceneManager;
    [SerializeField] private PlayerDataController _playerDataController;
    [Header("Demo Options")]
    [SerializeField] private bool _NotUseExistedPlayer;

    private const string _guestPlayerIDKey = "LootLockerGuestPlayerID";
    private const int _leaderboardID = 5374;
    private const string _leaderboardKey = "CityRunnerGlobalTopList";

    private string _playerIdentifierLootLocker;
    private string _sessionTokenLootLocker;
    private int _playerIDLootLocker;
    public bool GuestSessionInited { get; private set;}
    private string _playerNameLootLocker;

    private void Awake()
    {
        GuestSessionInited = false;
    }

    void Start()
    {
#if UNITY_EDITOR
        if (!_NotUseExistedPlayer)
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
            _playerIdentifierLootLocker = PlayerPrefs.GetString(_guestPlayerIDKey);
            StartCoroutine(InitExistenPlayerRecord());
        }
    }

    private IEnumerator InitExistenPlayerRecord()
    {
        yield return InitGuestSession();
        if (GuestSessionInited)
        {
            yield return GetPlayerName();
            Debug.LogError($"_playerNameLootLocker[{_playerNameLootLocker}] _playerNameLootLocker==null[{_playerNameLootLocker==null}] Length[{_playerNameLootLocker.Length}]");
            if (_playerNameLootLocker == null || _playerNameLootLocker.Length == 0)
            {
                Debug.LogWarning($"{this} : Current player name for {_playerIDLootLocker} playerIDLootLocker is Empty will be used playerID");
                _playerNameLootLocker = _playerIDLootLocker.ToString();
            }
            _playerDataController.SetPlayerName(_playerNameLootLocker);
        }
    }

    public IEnumerator CreateNewPlayerRecord(string playerName)
    {
        //BackUp and Remove the previous Player Record
        if (PlayerPrefs.HasKey(_guestPlayerIDKey))
        {
            BackUpOldPlayerRecord();
            if (_sessionTokenLootLocker != null)
            {
                yield return StopCurrentSession(); 
            }
            else
                Debug.LogWarning($"{this} : Exist the _guestPlayerIDKey key, but active session is absent");
            PlayerPrefs.DeleteKey(_guestPlayerIDKey);
        }
        yield return InitGuestSession(useExistedPlayerRecord: false);
        if (GuestSessionInited)
        {
            yield return SetPlayerName(playerName);
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
                _sessionTokenLootLocker = null;
                _playerIDLootLocker = 0;
                GuestSessionInited = false;
                Debug.Log($"{this} : Successfully {_sessionTokenLootLocker} EndSession");
            }
            else
            {
                Debug.LogWarning($"{this} : failed: " + response.Error);
            }
            notGetResponse = false;
        });
        yield return new WaitWhile(() => notGetResponse);
        CountFrame.DebugLogUpdate(this, $"Finished the StopCurrentSession()");
    }

    //Action<LootLockerGuestSessionResponse, LootLockerController> onComplete = (response, obj) =>
    //{
    //    Debug.Log($"Current state obj.notGetResponse == true [{obj.notGetResponse == true}] before StartGuestSession");
    //    if (response.success)
    //    {
    //        obj._sessionTokenLootLocker = response.session_token;
    //        obj._playerIDLootLocker = response.player_id;
    //        obj._guestSessionInited = true;
    //        Debug.Log($"{obj} : Successfully started LootLocker GuestSession with {obj._sessionTokenLootLocker} token");
    //    }
    //    else
    //    {
    //        Debug.Log($"{obj} : Error starting LootLocker GuestSession");
    //    }
    //    obj.notGetResponse = false;
    //};

    //private IEnumerator InitGuestSession(bool useExistedPlayerRecord = true)
    //{
    //    bool notGetResponse = true;
    //    if (useExistedPlayerRecord)
    //        LootLockerSDKManager.StartGuestSession(_playerIdentifierLootLocker, (response) => onComplete(response, this)); 
    //    else
    //        LootLockerSDKManager.StartGuestSession((response) => onComplete(response, this));
    //    yield return new WaitWhile(() => notGetResponse);
    //    CountFrame.DebugLogUpdate(this, $"Finished the InitGuestSession()");
    //}

    private IEnumerator InitGuestSession(bool useExistedPlayerRecord = true)
    {
        bool notGetResponse = true;
        if (useExistedPlayerRecord)
            LootLockerSDKManager.StartGuestSession(_playerIdentifierLootLocker, (response) =>
            {
                //Debug.Log($"Current state obj.notGetResponse == true [{notGetResponse == true}] before StartGuestSession");
                if (response.success)
                {
                    _sessionTokenLootLocker = response.session_token;
                    _playerIDLootLocker = response.player_id;
                    //_guestSessionInited = true;
                    Debug.Log($"InitGuestSession : Successfully started LootLocker GuestSession with {_sessionTokenLootLocker} token");
                }
                else
                {
                    Debug.LogError($"InitGuestSession : Error starting LootLocker GuestSession");
                }
                notGetResponse = false;
            });
        else
            LootLockerSDKManager.StartGuestSession((response) =>
            {
                Debug.Log($"Current state obj.notGetResponse == true [{notGetResponse == true}] before StartGuestSession");
                if (response.success)
                {
                    _sessionTokenLootLocker = response.session_token;
                    _playerIDLootLocker = response.player_id;
                    //_guestSessionInited = true;
                    Debug.Log($"InitGuestSession : Successfully started LootLocker GuestSession with {_sessionTokenLootLocker} token");
                }
                else
                {
                    Debug.LogError($"InitGuestSession : Error starting LootLocker GuestSession Error [{response.Error}]");
                }
                notGetResponse = false;
            });
        yield return new WaitWhile(() => notGetResponse);
        GuestSessionInited = true;
        CountFrame.DebugLogUpdate(this, $"Finished the InitGuestSession()");
    }

    private IEnumerator GetPlayerName()
    {
        bool notGetResponse = true;
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
        CountFrame.DebugLogUpdate(this, $"Finished the GetPlayerName()");
    }

    public void StartSendScoreToLeaderBoard(PlayerData newCharacterData) => StartCoroutine(SendScoreToLeaderBoard(newCharacterData.GetScoreValue()));
    public void StartSendScoreToLeaderBoard(int score) => StartCoroutine(SendScoreToLeaderBoard(score));

    public IEnumerator SendScoreToLeaderBoard(int score)
    {
        bool notGetResponse = true;
        //LootLockerSDKManager.SubmitScore(_playerIDLootLocker.ToString(), score, _leaderboardID, (response) =>
        LootLockerSDKManager.SubmitScore(_playerIDLootLocker.ToString(), score, _leaderboardKey, (response) =>
        {
            if (response.success)
            {
                Debug.Log($"{this} : Successful : Was writen [{score}] score for Player [{_playerIDLootLocker}]");
            }
            else
            {
                Debug.LogError($"{this} : failed: " + response.Error);
            }
            notGetResponse = false;
        });
        yield return new WaitWhile(() => notGetResponse);
        CountFrame.DebugLogUpdate(this, $"Finished the SendScoreToLeaderBoard()");
    }

    public IEnumerator SetPlayerName(string newName)
    {
        bool notGetResponse = true;
        LootLockerSDKManager.SetPlayerName(newName, (response) =>
        {
            if (response.success)
            {
                Debug.Log($"{this} : Successfully set {response.name} player name");
                
            }
            else
            {
                Debug.LogError($"{this} : Error setting player name");
            }
        });
        yield return new WaitWhile(() => notGetResponse);
        CountFrame.DebugLogUpdate(this, $"Finished the SetPlayerName()");
    }

    public bool GlobalListLoaded { get; private set; }

    public IEnumerator CoroutineGetScoreFromLeaderBoard(List<PlayerData> playerDatas, int count = 10, int after = 0)
    {
        GlobalListLoaded = false;
        bool notGetResponse = true;
        LootLockerLeaderboardMember[] table = default(LootLockerLeaderboardMember[]);
        //LootLockerSDKManager.GetScoreList(_leaderboardID, count, after, (response) =>
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
        CountFrame.DebugLogUpdate(this, $"Finished the GetScoreToLeaderBoard()");
    }
}

