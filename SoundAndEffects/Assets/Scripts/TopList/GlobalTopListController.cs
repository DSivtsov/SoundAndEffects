using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTopListController : TopListController
{
    private const int MaximumIterationForGetRemoteTopList = 500;
    [Header("RemoteTopList Options")]
    [SerializeField] private LootLockerController _lootLockerController;
    [SerializeField] private MainMenusSceneManager _mainMenusSceneManager;
    [SerializeField] private DisplayConnectingToServer _connectingToServer;

    protected new void Awake()
    {
        _topListElement = new TopListRemoteGroupElement();
        InitialLoadTopList();
    }

    public override void LoadAndShow(bool multiAsyncOperations = true)
    {
        if (_mainMenusSceneManager.IsConnectedToServer)
        {
            InitCharacterData = false;
            _topList = new List<PlayerData>();
            StartCoroutine(_lootLockerController.CoroutineGetScoreFromLeaderBoard(_topList, () => ShowTopList()));
        }
        else
        {
            CountFrame.DebugLogUpdate(this, $" : RemoteTopListController.LoadAndShow skipped");
        }

    }

    public void ShowTopList()
    {
        ActivateAndCheckTopList();
        if (InitCharacterData)
        {
            //The Sorting of TopList by Score did remotely on Server
            _topListElement.UpdateTopList(false);
            ////In case reload after save result doing only one operation simultenoius
            //if (multiAsyncOperations)
            //    _lootLockerController.FinishOneConnectionToServer(MultiOperation.LoadedTopList);
            //else
            //    _lootLockerController.CheckResultsServerOperations(resultOK: true);
        }
        else
        {
            //_lootLockerController.CheckResultsServerOperations(resultOK: false, ErrorConnecting.TopListNotLoaded);
            Debug.LogError($"{this} : Remote TopList was not Loaded");
        }
    }

    public override void AddCharacterResult(PlayerData newCharacterData)
    {
        if (_mainMenusSceneManager.IsConnectedToServer)
        {
            StartCoroutine(_lootLockerController.CoroutineSaveScoreToLeaderBoard(newCharacterData.GetScoreValue())); 
        }
        else
        {
            CountFrame.DebugLogUpdate(this, $" : RemoteTopListController.AddCharacterResult skipped");
        }
    }

    //private IEnumerator CoroutineSaveScoreToLeaderBoard(PlayerData newCharacterData)
    //{
    //    yield return _lootLockerController.SendScoreToLeaderBoard(newCharacterData.GetScoreValue());
    //    //if (_lootLockerController.NewResultWasSaved)
    //    //{
    //    //    LoadAndShow(multiAsyncOperations: false);
    //    //}
    //    //else
    //    //    _lootLockerController.CheckResultsServerOperations(resultOK: false, ErrorConnecting.NewResultNotSaved);
    //}
}