using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GMTools.Manager;

public enum TopListSource
{
    Demo,
    Local,
    Remote
}

abstract public class TopListController : MonoBehaviour
{
    [SerializeField] protected Transform _rootRecords;
    //[SerializeField] protected TopListSource _usedTopListSource = TopListSource.Local;
    //[SerializeField] private StoreClassPlayerData _storePlayerData;

    //[Header("Demo Options TopList")]
    /// <summary>
    /// Use the IComparable<T> from CharacterData
    /// </summary>
    //[SerializeField] protected bool _autoSortByScore = true;
    [SerializeField] protected bool _loadAndShowAtStart = true;

    protected List<PlayerData> _topList;
    protected TopListElementBase _topListElement;
    //private StoreObjectController _storeObjectController;

    public bool InitCharacterData { get; protected set; }

    protected void Awake()
    {
        _topListElement = new TopListGroupElement();
        InitCharacterData = false;
    }

    //private void Start()
    //{
    //    _storeObjectController = _storePlayerData.GetStoreObjectController();
    //}

    public void InitialLoadTopList()
    {
        _topListElement.CreateTableTopList(_rootRecords);
        if (_loadAndShowAtStart)
        {
            LoadAndShow();
        }
    }

    abstract protected void LoadAndShow(bool multiAsyncOperations = true);
    //protected virtual void LoadAndShow(bool multiAsyncOperations = true)
    //{
    //    LoadTopList();
    //    UpdateAndShowTopList();
    //}

    //abstract public void SaveTopList();
    ///// <summary>
    ///// Save to storage data from the _toplist 
    ///// </summary>
    //public void SaveTopList()
    //{
    //    _storePlayerData.SetObjectsToSave(_topList.ToArray());
    //    _storeObjectController.Save();
    //}

    //public void ResetTopList()
    //{
    //    Debug.Log("ButtonType.ResetTopList");
    //    _topList.Clear();
    //    SaveTopList();
    //    UpdateAndShowTopList();
    //    ActivateAndCheckTopList(false);
    //}

    abstract public void AddCharacterResult(PlayerData newCharacterData);
    //public virtual void AddCharacterResult(PlayerData newCharacterData)
    //{
    //    //Because Initialy the TopList can be Empty and not Active
    //    if (_topList == null || _topList.Count == 0)
    //        InitTopLis();
    //    _topList.Add(newCharacterData);
    //    SaveTopList();
    //    UpdateAndShowTopList();
    //}

    //private void InitTopLis()
    //{
    //    _topList = new List<PlayerData>();
    //    _topListElement.SetUsedTopList(_topList);
    //    InitCharacterData = true;
    //}

    /// <summary>
    /// ReSort, Update and Show records of _toplist
    /// </summary>
    //public void UpdateAndShowTopList()
    //{
    //    Debug.Log("TopListController : UpdateAndShowTopList()");
    //    if (_topListElement != null)
    //    {
    //        if (InitCharacterData)
    //            _topListElement.UpdateTopList(_autoSortByScore);
    //        else
    //            Debug.LogWarning($"{this.GetType().Name} : TopList is Empty");
    //    }
    //    else
    //        Debug.LogError("TopListElementBase == null");
    //}

    //abstract public void LoadTopList();

    /// <summary>
    /// Fill the _toplist from Local storage or from Demo data
    /// </summary>
    //public void LoadTopList()
    //{
    //    Debug.Log("TopListController : LoadTopList()");
    //    InitCharacterData = false;
    //    switch (_usedTopListSource)
    //    {
    //        case TopListSource.Demo:
    //            LoadDemoData(TopListElementBase.MaxNumShowRecords);
    //            break;
    //        case TopListSource.Local:
    //            IOError error = _storeObjectController.Load();
    //            if (error == IOError.NoError)
    //            {
    //                Debug.Log($"GetLoadedObjects().Length=[{_storePlayerData}]");
    //                _topList = new List<PlayerData>(_storePlayerData.GetLoadedObjects());
    //            }
    //            else
    //            {
    //                ErrorLoadTopList(error);
    //            }
    //            break;
    //    }
    //    ActivateAndCheckTopList();
    //}

    //private void ErrorLoadTopList(IOError error)
    //{
    //    switch (error)
    //    {
    //        case IOError.FileNotFound:
    //            Debug.LogWarning($"{this} : [{_storeObjectController.GetNameFile()}] file which stores the local TopList, not found will be created new at first Save");
    //            break;
    //        case IOError.WrongFormat:
    //            Debug.LogError($"{this} : QuickLoad() - Format of the [{_storeObjectController.GetNameFile()}] file is wrong. Restore interrupted");
    //            break;
    //    }
    //}

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
    //private void LoadDemoData(int numRecords)
    //{
    //    System.Random random = new System.Random();
    //    Debug.Log("CreateDemoData()");
    //    _topList = new List<PlayerData>(TopListElementBase.MaxNumShowRecords);
    //    int distance;
    //    int score;
    //    string timstr;
    //    for (int i = 0; i < numRecords; i++)
    //    {
    //        //Test Data for testing interface
    //        //distance = random.Next(1, 600);
    //        //score = random.Next(1,999999);

    //        //Test Data for testing Game Logic
    //        distance = random.Next(1, 300);
    //        score = random.Next(1, 75);
    //        timstr = (i % 2 == 1) ? "abhgjhgjhgjhgjhgj".ToUpper() : "abhgjhgjhgjhgjhgj";
    //        if (i == 0)
    //            timstr = "01234567890123456";
    //        _topList.Add(new PlayerData(timstr, distance, score));
    //    }
    //}
}