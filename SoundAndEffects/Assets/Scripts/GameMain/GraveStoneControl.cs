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
    private TMP_InputField _inputName;
    private CharacterDataController _characterDataCtrl;

    //private const UnitSystemDistance _currentDisplayUnitSystemDistance = UnitSystemDistance.ft;
    //private const float OneFootInMeter = 0.3048f;

    private void Awake()
    {
        _textAphorism = _graveStoneGroup.transform.Find("AphorismText").GetComponent<TextMeshProUGUI>();
        _textScore = _graveStoneGroup.transform.Find("GraveStoneText/Score").GetComponent<TextMeshProUGUI>();
        _textDistance = _graveStoneGroup.transform.Find("GraveStoneText/Distance").GetComponent<TextMeshProUGUI>();
        _inputName = _graveStoneGroup.transform.Find("GraveStoneText/Name/Input").GetComponent<TMP_InputField>();
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
        _inputName.text = _nameCurrentPlayer;
        Debug.LogError("Time Solution : InputField was made readOnly = true");
        _inputName.readOnly = true;
        _inputName.ActivateInputField();
        //Debug.Log("ActivategraveStoneGroupAndFocusInputField()");
    }
    public void DeactivategraveStoneGroup()
    {
        _graveStoneGroup.SetActive(false);
    }

    public string GetUserName() => _inputName.text;
}