using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteTopListController : TopListController
{
    private const int MaximumIterationForGetRemoteTopList = 500;
    [Header("Remote Source")]
    [SerializeField] protected LootLockerController _lootLockerController;

    private new void Awake()
    {
        base.Awake();
        _topListElement = new TopListRemoteGroupElement();
    }

    protected override IEnumerator GetRemoteTopList(List<PlayerData> remoteTopList)
    {
        //CountFrame.DebugLogUpdate(this, $"GetRemoteTopList Begin");
        yield return _lootLockerController.CoroutineGetScoreFromLeaderBoard(remoteTopList);
        ActivateAndCheckTopList();
        CountFrame.DebugLogUpdate(this, $"GetRemoteTopList Finished _topList[{remoteTopList.Count}] InitCharacterData = true [{InitCharacterData == true}]");
    }

    protected override void UpdateAndShowTopList()
    {
        StartCoroutine(WainInitCharacterData());
        //CountFrame.DebugLogUpdate(this, $"UpdateAndShowTopListBase StartCoroutine(WainInitCharacterData()) started");
    }

    private IEnumerator WainInitCharacterData()
    {
        int count = 0;
        while (!InitCharacterData)
        {
            count++;
            if (count > MaximumIterationForGetRemoteTopList)
                break;
            else
                yield return null; 
        }
        
        if (InitCharacterData)
        {
            Debug.Log($"{this} : Remote TopList Loaded at {count} count");
            _topListElement.UpdateTopList(_autoSortByScore); 
        }
        else
            Debug.LogError($"{this} : Remote TopList can't Load at {count} count");
        CountFrame.DebugLogUpdate(this, $"UpdateAndShowTopList Finished");
    }

    protected override void LoadAndShow()
    {
        StartCoroutine(WaitIniSession());
        CountFrame.DebugLogUpdate(this, $"LoadAndShow StartCoroutine(WainIniSession()) started");
    }

    private IEnumerator WaitIniSession()
    {
        int count = 0;
        while (!_lootLockerController.GuestSessionInited)
        {
            count++;
            if (count > 200)
            {
                Debug.LogError($"{this} : LootLocker Session not inited");
                yield break;
            }
            else
                yield return null;
        }
        LoadTopList();
        InitUpdateAndShowTopList();
        CountFrame.DebugLogUpdate(this, $"LoadAndShow Finished");
    }

    public override void AddNewCharacterData(PlayerData newCharacterData)
    {
        //SaveTopList();
        StartCoroutine(WaitSaveScoreToLeaderBoard(newCharacterData));
        //InitCharacterData = false;
        //InitUpdateAndShowTopList();
    }

    private IEnumerator WaitSaveScoreToLeaderBoard(PlayerData newCharacterData)
    {
        yield return _lootLockerController.SendScoreToLeaderBoard(newCharacterData.GetScoreValue());
        InitCharacterData = false;
        CountFrame.DebugLogUpdate(this, $"SaveGameResult Finished");
        LoadAndShow();
    }
}
