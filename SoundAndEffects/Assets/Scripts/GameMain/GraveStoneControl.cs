using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum UnitSystemDistance
{
    m,
    ft
}

public class GraveStoneControl : MonoBehaviour
{
    [SerializeField] private GameObject _graveStoneGroup;

    private TextMeshProUGUI _textAphorism;
    private TextMeshProUGUI _textScore;
    private TextMeshProUGUI _textDistance;
    private TMP_InputField _inputName;
    private CharacterData _characterData;

    private const UnitSystemDistance _currentDisplayUnitSystemDistance = UnitSystemDistance.ft;
    private const float OneFootInMeter = 0.3048f;

    private void Awake()
    {
        _textAphorism = _graveStoneGroup.transform.Find("AphorismText").GetComponent<TextMeshProUGUI>();
        _textScore = _graveStoneGroup.transform.Find("GraveStoneText/Score").GetComponent<TextMeshProUGUI>();
        _textDistance = _graveStoneGroup.transform.Find("GraveStoneText/Distance").GetComponent<TextMeshProUGUI>();
        _inputName = _graveStoneGroup.transform.Find("GraveStoneText/Name/Input").GetComponent<TMP_InputField>();
        _characterData = SingletonGame.Instance.GetCharacterData();
    }

    private int ConvertToDisplayUnitSystemDistance(int distanceInMeters) =>
        (_currentDisplayUnitSystemDistance == UnitSystemDistance.ft) ? Mathf.RoundToInt( distanceInMeters / OneFootInMeter) : distanceInMeters;

    public void ActivateGraveStoneGroupAndFocusInputField()
    {
        //Template "Distance: 9999 ft"
        _textDistance.text = $"Distance: {ConvertToDisplayUnitSystemDistance(_characterData.GetCurrentSummaryDistance())} {_currentDisplayUnitSystemDistance}";
        //_textAphorism.text = "That which we call a rose by any other name would smell as sweet";
        _textAphorism.text = AphorismText.GetStrRandomAphorismText();
        //Template "Score: 999 999"
        _textScore.text = $"Score: {_characterData.GetCurrentSummaryScore():000 000}";
        _graveStoneGroup.SetActive(true);
        _inputName.ActivateInputField();
        //Debug.Log("ActivategraveStoneGroupAndFocusInputField()");
    }
    public void DeactivategraveStoneGroup()
    {
        _graveStoneGroup.SetActive(false);
    }

    public void StoreUserResult()
    {
        Debug.Log($"StoreUserResult() : {_inputName.text}");
    }
}
