using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GraveStoneControl : MonoBehaviour
{
    [SerializeField] private GameObject _graveStoneGroup;

    private TextMeshProUGUI _textAphorism;
    private TextMeshProUGUI _textScore;
    private TextMeshProUGUI _textDistance;
    private TMP_InputField _inputName;
    private void Awake()
    {
        _textAphorism = _graveStoneGroup.transform.Find("AphorismText").GetComponent<TextMeshProUGUI>();
        _textScore = _graveStoneGroup.transform.Find("GraveStoneText/Score").GetComponent<TextMeshProUGUI>();
        _textDistance = _graveStoneGroup.transform.Find("GraveStoneText/Distance").GetComponent<TextMeshProUGUI>();
        _inputName = _graveStoneGroup.transform.Find("GraveStoneText/Name/Input").GetComponent<TMP_InputField>();
        //ActivateField();
    }

    public void ActivategraveStoneGroupAndFocusInputField()
    {
        _graveStoneGroup.SetActive(true);
        Debug.Log(_textAphorism);
        Debug.Log(_textScore);
        Debug.Log(_textDistance);
        Debug.Log(_inputName);
        Debug.Log(_inputName.text);
        _inputName.ActivateInputField();
    }
    public void DeactivategraveStoneGroup()
    {
        _graveStoneGroup.SetActive(false);
    }
}
