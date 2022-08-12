using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class TopListElementBase
{
    protected Transform _rootRecords;
    public const int MaxNumShowRecords = 10;
    protected List<PlayerData> _topList;

    private RectTransform[] _rectRecords = new RectTransform[MaxNumShowRecords];
    private Transform _recordTemplate;
    private RectTransform _rectTransformTemplate;
    private float _heightRecords;

    protected TopListElementBase()
    {
        TopListElementInited = false;
    }

    public bool TopListElementInited { get; private set; }

    /// <summary>
    /// Create TopList (all records TurnOff by default). Number records limitted by const MaxNumShowRecords
    /// </summary>
    /// <param name="rootRecords"></param>
    public void CreateTableTopList(Transform rootRecords)
    {
        if (!TopListElementInited)
        {
            _rootRecords = rootRecords;
            SetInstantiationParameters();
            InstantiateTopList();
            Debug.Log($"CreateTableTopList() : Table for {rootRecords.parent.name} created");
            TopListElementInited = true; 
        }
    }

    private void SetInstantiationParameters()
    {
        _recordTemplate = GetRecordTemplate();
        //Template Turn off always
        _recordTemplate.gameObject.SetActive(false);
        _rectTransformTemplate = _recordTemplate as RectTransform;
        _heightRecords = _rectTransformTemplate.sizeDelta.y;
    }

    private void InstantiateTopList(bool ActivateAllRecords = false)
    {
        for (int i = 0; i < MaxNumShowRecords; i++)
        {
            RectTransform newRectTransform = Object.Instantiate<RectTransform>(_rectTransformTemplate, _rootRecords);
            newRectTransform.anchoredPosition = new Vector2(0, -_heightRecords * i);
            //All records will be not active before Update by default
            newRectTransform.gameObject.SetActive(ActivateAllRecords);
            _rectRecords[i] = newRectTransform;
            FillArrayText(i, newRectTransform);
        }
    }
    /// <summary>
    /// ReSort, Update and Show records of TopList
    /// </summary>
    /// <param name="_autoSortByScore"></param>
    public void UpdateTopList(bool _autoSortByScore = true)
    {
        int showedNumberRecords = GetShowedNumberRecords();
        if (TopListElementInited)
        {
            if (_autoSortByScore)
                _topList.Sort();
            for (int i = 0; i < showedNumberRecords; i++)
            {
                SetValuesToRecord(i);
                _rectRecords[i].gameObject.SetActive(true);
            }
            //int numEmptyRecords = MaxNumShowRecords - showedNumberRecords;
            //All Empty records  TurnOff
            for (int i = showedNumberRecords; i < MaxNumShowRecords; i++)
            {
                _rectRecords[i].gameObject.SetActive(false);
            }
        }
        else
            Debug.LogError($"{this.GetType().Name} : Table for TopList not created");
    }

    public int GetShowedNumberRecords() => (_topList.Count > MaxNumShowRecords) ? MaxNumShowRecords : _topList.Count;
    /// <summary>
    /// Link the corespoding List<CharacterData> which will be a source data for methods of TopListElementBase class
    /// </summary>
    /// <param name="topList"></param>
    public void SetUsedTopList(List<PlayerData> topList) => _topList = topList;

    /// <summary>
    /// Get the Template Type which will be used to create the TopList
    /// </summary>
    /// <returns></returns>
    protected abstract Transform GetRecordTemplate();
    /// <summary>
    /// Fill the array (arries) which will store link to coresponding TextMeshProUGUI objects
    /// </summary>
    /// <param name="i"></param>
    /// <param name="newRectTransform"></param>
    protected abstract void FillArrayText(int i, RectTransform newRectTransform);

    /// <summary>
    /// The current TopList must be Sorted before calling this method, which set the position numbers according to order records in the array
    /// </summary>
    /// <param name="i"></param>
    protected abstract void SetValuesToRecord(int i);
}
