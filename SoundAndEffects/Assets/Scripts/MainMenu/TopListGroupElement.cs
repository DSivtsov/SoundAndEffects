using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TopListGroupElement : TopListElementBase
{
    private TextMeshProUGUI[] _strPosNameTextMeshRecords = new TextMeshProUGUI[MaxNumShowRecords];
    private TextMeshProUGUI[] _numDistanceTextMeshRecords = new TextMeshProUGUI[MaxNumShowRecords];
    private TextMeshProUGUI[] _numScoreMeshRecords = new TextMeshProUGUI[MaxNumShowRecords];

    protected override Transform GetRecordTemplate() => _rootRecords.Find("GroupTemplate");


    protected override void FillArrayText(int i, RectTransform newRectTransform)
    {
        _strPosNameTextMeshRecords[i] = newRectTransform.Find("PosNameTemplate").GetComponent<TextMeshProUGUI>();
        _numDistanceTextMeshRecords[i] = newRectTransform.Find("DistanceTemplate").GetComponent<TextMeshProUGUI>();
        _numScoreMeshRecords[i] = newRectTransform.Find("ScoreTemplate").GetComponent<TextMeshProUGUI>();
    }

    protected override void SetValuesToRecord(int i)
    {
        (string userName, int distance, int score) = _topList[i].GetValues();
        _strPosNameTextMeshRecords[i].text = $"{(i+1):00}     {userName}";
        //_numDistanceTextMeshRecords[i].text = string.Format("{0,4:D} {1,2}", UnitSystem.Convert(distance), UnitSystem.Current);
        _numDistanceTextMeshRecords[i].text = $"{UnitSystem.Convert(distance),5:D} {UnitSystem.Current,2}"; 
        _numScoreMeshRecords[i].text = $"{score:000 000}";
    }
}
