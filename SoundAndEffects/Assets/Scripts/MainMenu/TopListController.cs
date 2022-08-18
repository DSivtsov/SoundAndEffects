using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Manager;

public enum TopListSource
{
    Demo,
    Local,
    Remote
}

public class TopListController : MonoBehaviour
{
    [SerializeField] private Transform _rootRecords;
    [SerializeField] private StoreGame _storeGame;
    [SerializeField] private TopListSource _usedTopListSource = TopListSource.Local;
    [Header("Demo Options TopList")]
    /// <summary>
    /// Use the IComparable<T> from CharacterData
    /// </summary>
    [SerializeField] protected bool _autoSortByScore = true;
    [SerializeField] bool _loadAndShowAtStart = true;

    protected List<PlayerData> _topList;
    protected TopListElementBase _topListElement;
    private StoreObjectT<PlayerData> _storeObjectT;

    public bool InitCharacterData { get; protected set; }

    protected void Awake()
    {
        _topListElement = new TopListGroupElement();
        InitCharacterData = false;
        _storeObjectT = GetComponent<StoreObjectT<PlayerData>>();
    }

    public void InitialLoadTopList()
    {
        _topListElement.CreateTableTopList(_rootRecords);
        if (_loadAndShowAtStart)
        {
            LoadAndShow();
        }
    }

    protected virtual void LoadAndShow(bool multiAsyncOperations = true)
    {
        LoadTopList();
        UpdateAndShowTopList();
    }

    /// <summary>
    /// Save to storage data from the _toplist 
    /// </summary>
    public void SaveTopList()
    {
        _storeObjectT.SetObjectsToSave(_topList.ToArray());
        _storeGame.QuickSave();
    }

    public void ResetTopList()
    {
        Debug.Log("ButtonType.ResetTopList");
        _topList.Clear();
        SaveTopList();
        UpdateAndShowTopList();
        ActivateAndCheckTopList(false);
    }

    public virtual void AddCharacterResult(PlayerData newCharacterData)
    {
        //Because Initialy the TopList can be Empty and not Active
        if (_topList == null || _topList.Count == 0)
            InitTopLis();
        _topList.Add(newCharacterData);
        SaveTopList();
        UpdateAndShowTopList();
    }

    private void InitTopLis()
    {
        _topList = new List<PlayerData>();
        _topListElement.SetUsedTopList(_topList);
        InitCharacterData = true;
    }

    /// <summary>
    /// ReSort, Update and Show records of _toplist
    /// </summary>
    public void UpdateAndShowTopList()
    {
        Debug.Log("TopListController : UpdateAndShowTopList()");
        if (_topListElement != null)
        {
            if (InitCharacterData)
                _topListElement.UpdateTopList(_autoSortByScore);
            else
                Debug.LogWarning($"{this.GetType().Name} : TopList is Empty");
        }
        else
            Debug.LogError("TopListElementBase == null");
    }

    /// <summary>
    /// Fill the _toplist from Local storage or from Demo data
    /// </summary>
    public void LoadTopList()
    {
        Debug.Log("TopListController : LoadTopList()");
        InitCharacterData = false;
        switch (_usedTopListSource)
        {
            case TopListSource.Demo:
                LoadDemoData(TopListElementBase.MaxNumShowRecords);
                break;
            case TopListSource.Local:
                _storeGame.QuickLoad();
                _topList = new List<PlayerData>(_storeObjectT.GetLoadedObjects());
                break;
        }
        ActivateAndCheckTopList();
    }

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

    /// <summary>
    /// Fill the _toplist from random generated a Demo data 
    /// </summary>
    private void LoadDemoData(int numRecords)
    {
        System.Random random = new System.Random();
        Debug.Log("CreateDemoData()");
        _topList = new List<PlayerData>(TopListElementBase.MaxNumShowRecords);
        int distance;
        int score;
        string timstr;
        for (int i = 0; i < numRecords; i++)
        {
            //Test Data for testing interface
            //distance = random.Next(1, 600);
            //score = random.Next(1,999999);

            //Test Data for testing Game Logic
            distance = random.Next(1, 300);
            score = random.Next(1, 75);
            timstr = (i % 2 == 1) ? "abhgjhgjhgjhgjhgj".ToUpper() : "abhgjhgjhgjhgjhgj";
            if (i == 0)
                timstr = "01234567890123456";
            _topList.Add(new PlayerData(timstr, distance, score));
        }
    }
}
