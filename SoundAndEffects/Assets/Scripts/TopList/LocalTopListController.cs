using System.Collections.Generic;
using UnityEngine;
using GMTools.Manager;

public class LocalTopListController : TopListController
{
    [Header("LocalTopList Options")]
    [SerializeField] private TopListSource _usedTopListSource = TopListSource.Local;
    [SerializeField] private StoreObjectController _storeObjectController;
    ///// <summary>
    ///// Use the IComparable<T> from CharacterData
    ///// </summary>
    [SerializeField] protected bool _autoSortByScore = true;
    
    private StoredPlainClass<PlayerData> _storedPlainPlayerData;

    private void Start()
    {
        _storedPlainPlayerData = (StoredPlainClass<PlayerData>)_storeObjectController.GetStoredObject();
    }

    protected override void LoadAndShow(bool multiAsyncOperations = true)
    {
        CountFrame.DebugLogUpdate(this, $"LoadAndShow() started");
        LoadTopList();
        UpdateAndShowTopList();
    }

    /// <summary>
    /// Save to storage data from the _toplist 
    /// </summary>
    public void SaveTopList()
    {
        _storedPlainPlayerData.SetObjectsToSave(_topList.ToArray());
        IOError error = _storeObjectController.Save();
        if (error != IOError.NoError)
            Debug.LogWarning($"{this} : [{_storeObjectController.GetNameFile()} Save breaked");
    }

    public void ResetTopList()
    {
        //Debug.Log("ButtonType.ResetTopList");
        _topList.Clear();
        SaveTopList();
        UpdateAndShowTopList();
        ActivateAndCheckTopList(false);
    }

    public override void AddCharacterResult(PlayerData newCharacterData)
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
        //Debug.LogWarning($"{this} : InitTopLis() created new TopList");
        _topList = new List<PlayerData>();
        _topListElement.SetUsedTopList(_topList);
        InitCharacterData = true;
    }

    /// <summary>
    /// ReSort, Update and Show records of _toplist
    /// </summary>
    public void UpdateAndShowTopList()
    {
        if (_topListElement != null)
        {
            if (InitCharacterData)
                _topListElement.UpdateTopList(_autoSortByScore);
            //else
            //    Debug.LogWarning($"{this.GetType().Name} : TopList is Empty");
        }
        else
            Debug.LogError($"{this} : TopListElementBase == null");
    }

    /// <summary>
    /// Fill the _toplist from Local storage or from Demo data
    /// </summary>
    public void LoadTopList()
    {
        InitCharacterData = false;
        switch (_usedTopListSource)
        {
            case TopListSource.Demo:
                LoadDemoData(TopListElementBase.MaxNumShowRecords);
                break;
            case TopListSource.Local:
                IOError error = _storeObjectController.Load();
                if (error == IOError.NoError)
                {
                    PlayerData[] loaded = _storedPlainPlayerData.GetLoadedObjects();
                    if (loaded == null || loaded.Length == 0)
                    {
                        InitTopLis(); 
                    }
                    else
                        _topList = new List<PlayerData>(loaded);
                }
                else
                {
                    //Debug.LogWarning($"{this} : [{_storeObjectController.GetNameFile()} Load breaked");
                    InitTopLis();
                }
                break;
        }
        ActivateAndCheckTopList();
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
