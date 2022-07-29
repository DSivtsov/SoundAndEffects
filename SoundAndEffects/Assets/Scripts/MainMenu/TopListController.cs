using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Manager;

public class TopListController : MonoBehaviour
{
    [SerializeField] private Transform _rootRecords;
    [SerializeField] StoreGame _storeGame;
    [Header("Demo Options TopList")]
    [SerializeField] bool _useDemoData = false;
    /// <summary>
    /// Use the IComparable<T> from CharacterData
    /// </summary>
    [SerializeField] bool _autoSortByScore = true;
    [SerializeField] bool _loadAndShowAtStart = true;

    private List<CharacterData> _topList;
    private TopListElementBase topListElement;
    private StoreObjectT<CharacterData> _storeObjectT;

    public bool InitCharacterData { get; private set; }

    public bool UseDemoData => _useDemoData;

    private void Awake()
    {
        topListElement = new TopListGroupElement();
        InitCharacterData = false;
        _storeObjectT = GetComponent<StoreObjectT<CharacterData>>();
        Init();
    }

    private void Init()
    {
        _topList = new List<CharacterData>();
        topListElement.SetUsedTopList(_topList);
        InitCharacterData = true;
    }

    public void InitialLoadTopList()
    {
        topListElement.CreateTableTopList(_rootRecords);
        if (_loadAndShowAtStart)
        {
            LoadTopList();
            UpdateAndShowTopList(); 
        }
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

    public void AddNewCharacterData(CharacterData newCharacterData)
    {
        Debug.Log("AddAndSaveNewCharacterData()");
        _topList.Add(newCharacterData);
        //Because Initialy the TopList can be Empty and not Active
        ActivateAndCheckTopList();
        Debug.Log(newCharacterData);
        SaveTopList();
        UpdateAndShowTopList();
    }

    /// <summary>
    /// ReSort, Update and Show records of _toplist
    /// </summary>
    public void UpdateAndShowTopList()
    {
        Debug.Log("TopListController : UpdateAndShowTopList()");
        if (topListElement != null)
        {
            if (InitCharacterData)
                topListElement.UpdateTopList(_autoSortByScore);
            else
                Debug.LogWarning($"{this.GetType().Name} : TopList is Empty");
        }
        else
            Debug.LogError("TopListElementBase == null");
    }

    /// <summary>
    /// Fill the _toplist from storage or from Demo data
    /// </summary>
    public void LoadTopList()
    {
        Debug.Log("TopListController : LoadTopList()");
        if (_useDemoData)
        {
            LoadDemoData(TopListElementBase.MaxNumShowRecords);
        }
        else
        {
            _storeGame.QuickLoad();
            _topList = new List<CharacterData>(_storeObjectT.GetLoadedObjects());
        }
        ActivateAndCheckTopList();
        //topListElement.SetUsedTopList(_topList);
        //InitCharacterData = true;
    }

    private void ActivateAndCheckTopList(bool activate = true)
    {
        if (activate && _topList.Count != 0)
        {
            topListElement.SetUsedTopList(_topList);
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
        _topList = new List<CharacterData>(TopListElementBase.MaxNumShowRecords);
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
            _topList.Add(new CharacterData(timstr, distance, score));
        }
    }
}
