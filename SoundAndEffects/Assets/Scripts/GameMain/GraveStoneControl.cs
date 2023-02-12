using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GraveStoneControl : MonoBehaviour
{
    [SerializeField] private GameObject _graveStoneGroup;

    private TextMeshProUGUI _textAphorism;
    private TextMeshProUGUI _textScore;
    private TextMeshProUGUI _textDistance;
    private TextMeshProUGUI _textName;
    private CharacterDataController _characterDataCtrl;

    //private const UnitSystemDistance _currentDisplayUnitSystemDistance = UnitSystemDistance.ft;
    //private const float OneFootInMeter = 0.3048f;

    private void Awake()
    {
        _textAphorism = _graveStoneGroup.transform.Find("AphorismText").GetComponent<TextMeshProUGUI>();
        _textScore = _graveStoneGroup.transform.Find("GraveStoneText/Score").GetComponent<TextMeshProUGUI>();
        _textDistance = _graveStoneGroup.transform.Find("GraveStoneText/Distance").GetComponent<TextMeshProUGUI>();
        _textName = _graveStoneGroup.transform.Find("GraveStoneText/Name").GetComponent<TextMeshProUGUI>();
        _characterDataCtrl = SingletonGame.Instance.GetCharacterDataCtrl();
    }

    //private int ConvertToDisplayUnitSystemDistance(int distanceInMeters) =>
    //    (_currentDisplayUnitSystemDistance == UnitSystemDistance.ft) ? Mathf.RoundToInt( distanceInMeters / OneFootInMeter) : distanceInMeters;

    public void ActivateGraveStoneGroupAndFocusInputField(string _nameCurrentPlayer)
    {
        //Template "Distance: 9999 ft"
        _textDistance.text = $"Distance: {UnitSystem.Convert(_characterDataCtrl.SummaryDistance)} {UnitSystem.Current}";
        //_textAphorism.text = "That which we call a rose by any other name would smell as sweet";
        _textAphorism.text = AphorismText.GetStrRandomAphorismText();
        //Template "Score: 999 999"
        _textScore.text = $"Score: {_characterDataCtrl.SummaryScores:000 000}";
        _graveStoneGroup.SetActive(true);
        _textName.text = _nameCurrentPlayer;
    }
    public void DeactivategraveStoneGroup()
    {
        _graveStoneGroup.SetActive(false);
    }

    public string GetUserName() => _textName.text;
}