using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

/// <summary>
/// The Interface which must be implemented by a Class which will be controller for class DropdownOption<T> 
/// </summary>
public interface IOptionChanged
{
    public void OptionsChanged(bool isChanged);
}

/// <summary>
/// The common class for use the DropdownOption (TMP_Dropdown) in Sections of Menus
/// </summary>
/// <typeparam name="T">The class which will be controller for this Options</typeparam>
public class DropdownOption<T> where T : IOptionChanged
{
    private TMP_Dropdown _dropdownOption;
    private T _optionController;
    private UnityAction<int> _actionAtFieldValueChanged;
    /// <summary>
    /// Initial value will be used at ResetDefault
    /// </summary>
    private int _initialValue;

    public DropdownOption(string paramName, Transform parentTransformOptions, T optionController)
    {
        _dropdownOption = parentTransformOptions.Find(paramName).GetComponent<TMP_Dropdown>();
        _optionController = optionController;
        Debug.Log($"{typeof(T)}.ctor : Created");
    }

    public void InitOption(List<string> dropdownOptions, UnityAction<int> fieldValueChanged, int initialValue)
    {
        _initialValue = initialValue;
        _dropdownOption.ClearOptions();
        _dropdownOption.AddOptions(dropdownOptions);
        _dropdownOption.value = _initialValue;
        _actionAtFieldValueChanged = fieldValueChanged;
        //_dropdownOption.onValueChanged.AddListener(fieldValueChanged);
        _dropdownOption.onValueChanged.AddListener(FieldSetValue);
    }

    private void FieldSetValue(int value)
    {
        //_dropdownOption.value = value;
        _actionAtFieldValueChanged.Invoke(value);
        _optionController.OptionsChanged(true);
    }

    //Not checked Inited because it can be called w/o _inited = true
    public void ResetDefaultValue() => FieldSetValue(_initialValue);
}