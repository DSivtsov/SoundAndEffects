using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TopListSingleElement : TopListElementBase
{
    private TextMeshProUGUI[] _textMeshRecords = new TextMeshProUGUI[MaxNumShowRecords];

    protected override Transform GetRecordTemplate() => _rootRecords.Find("GroupTemplate");

    // "01     01234567890123456                 9999 ft          999 999"
    //$"{x:00}     {str}                 {y:0000} {"ft"}          {z:000 000}"
    //protected override void UpdateRecords()
    //{
    //    for (int i = 0; i < MaxNumShowRecords; i++)
    //    {
    //        //_textMeshRecords[i].text = $"{i:00}     {str}                 {y:0000} {"ft"}          {z:000 000}";
    //        (string userName, int distance, int score) = _topList[i].GetValues();
    //        _textMeshRecords[i].text = $"{i:00}     {userName}                 {UnitSystem.Convert(distance):0000} {UnitSystem.Current}          {score:000 000}";
    //    }
    //}

    protected override void FillArrayText(int i, RectTransform newRectTransform)
    {
        _textMeshRecords[i] = newRectTransform.GetComponent<TextMeshProUGUI>();
    }

    protected override void SetValuesToRecord(int i)
    {
        (string userName, int distance, int score) = _topList[i].GetValues();
        _textMeshRecords[i].text = $"{(i+1):00}     {userName}                 {UnitSystem.Convert(distance):00000} {UnitSystem.Current}          {score:000 000}";
    }
}
