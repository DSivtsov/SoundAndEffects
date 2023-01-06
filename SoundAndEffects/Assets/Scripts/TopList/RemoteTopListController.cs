using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteTopListController : TopListController
{
    private const int MaximumIterationForGetRemoteTopList = 500;
    [Header("RemoteTopList Options")]
    [SerializeField] private LootLockerController _lootLockerController;
    [SerializeField] private ConnectingToServer _connectingToServer;

    private new void Awake()
    {
        base.Awake();
        _topListElement = new TopListRemoteGroupElement();
    }

    protected override void LoadAndShow(bool multiAsyncOperations = true)
    {
        if (_lootLockerController.CurrentPlayMode != PlayMode.Offline)
        {
            StartCoroutine(CoroutineLoadAndShow(multiAsyncOperations));
        }
        else
            CountFrame.DebugLogUpdate(this, $"LoadAndShow skipped");
    }

    private IEnumerator CoroutineLoadAndShow(bool multiAsyncOperations = true)
    {
        //CountFrame.DebugLogUpdate(this, $"LoadAndShow : StartCoroutine(CoroutineLoadAndShow(multiAsyncOperations))");
        while (!_lootLockerController.GuestSessionInited && _connectingToServer.Connecting)
        {
            yield return null;
        }
        if (_lootLockerController.GuestSessionInited)
        {
            //Debug.Log("RemoteTopListController : LoadTopList()");
            InitCharacterData = false;
            _topList = new List<PlayerData>();
            yield return _lootLockerController.CoroutineGetScoreFromLeaderBoard(_topList);
            ActivateAndCheckTopList();
            if (InitCharacterData)
            {
                //Debug.Log($"{this} : Remote TopList Loaded");
                //The Sorting of TopList by Score did remotely
                _topListElement.UpdateTopList(false);
                //In case reload after save result doing only one operation simultenoius
                if (multiAsyncOperations)
                    _lootLockerController.FinishOneConnectionToServer(MultiOperation.LoadedTopList);
                else
                    _lootLockerController.FinalizeAllServerOperations(resultOK: true);
            }
            else
            {
                _lootLockerController.FinalizeAllServerOperations(resultOK: false, ErrorConnecting.TopListNotLoaded);
                Debug.LogError($"{this} : Remote TopList was not Loaded"); 
            }
            //CountFrame.DebugLogUpdate(this, $"CoroutineLoadAndShow Finished");
        }
        else
            CountFrame.DebugLogUpdate(this, $"LoadAndShow Canceled - GuestSession not inited");
    }

    public override void AddCharacterResult(PlayerData newCharacterData)
    {
        if (_lootLockerController.CurrentPlayMode == PlayMode.Online)
        {
            StartCoroutine(CoroutineSaveScoreToLeaderBoard(newCharacterData)); 
        }
        else
        {
            CountFrame.DebugLogUpdate(this, $" : AddCharacterResult skipped");
        }
    }

    private IEnumerator CoroutineSaveScoreToLeaderBoard(PlayerData newCharacterData)
    {
        yield return _lootLockerController.SendScoreToLeaderBoard(newCharacterData.GetScoreValue());
        if (_lootLockerController.NewResultWasSaved)
        {
            LoadAndShow(multiAsyncOperations: false);
        }
        else
            _lootLockerController.FinalizeAllServerOperations(resultOK: false, ErrorConnecting.NewResultNotSaved);
    }
}