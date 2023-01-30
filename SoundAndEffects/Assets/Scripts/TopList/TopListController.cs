using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GMTools.Manager;

public enum TopListSource
{
    Demo,
    Local,
    //Remote
}

abstract public class TopListController : MonoBehaviour
{
    [SerializeField] protected Transform _rootRecords;
    //[SerializeField] protected bool _loadAndShowAtStart = true;

    protected List<PlayerData> _topList;
    protected TopListElementBase _topListElement;

    public bool InitCharacterData { get; protected set; }

    protected void Awake()
    {
        _topListElement = new TopListGroupElement();
        InitialLoadTopList();
    }

    protected void InitialLoadTopList()
    {
        InitCharacterData = false;
        _topListElement.CreateTableTopList(_rootRecords);
    }

    abstract public void LoadAndShow(bool multiAsyncOperations = true);

    abstract public void AddCharacterResult(PlayerData newCharacterData);

    protected void ActivateAndCheckTopList(bool activate = true)
    {
        if (activate && _topList.Count != 0)
        {
            _topListElement.SetUsedTopList(_topList);
            InitCharacterData = true;
        }
        else
        {
            InitCharacterData = false;
        }
    }
}