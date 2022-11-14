using System;
using UnityEngine;
using GMTools.Menu;
/// <summary>
/// Controller for toggle group UIElements based on ToggleGroupEnum<T>
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="T"></typeparam>
//public class ToggleGroupEnumController<K, T> : IUIElement<K> where T : IOptionChanged where K : Enum
public class ToggleGroupEnumController<K, T> where T : IOptionChanged where K : Enum
{
    private ToggleGroupEnum<K> _toggleGroup;
    private T _optionController;
    private Action<K> _actionAtValueChanged;

    public bool Initialized
    {
        get
        {
            return _toggleGroup != null && _toggleGroup.ToggleGroupIsInit;
        }
    }

    public bool Dirty { get; private set; }

    public ToggleGroupEnumController(string paramName, Transform parentTransformOptions, T optionController)
    {
        try
        {
            _toggleGroup = parentTransformOptions.Find(paramName).GetComponentInChildren<ToggleGroupEnum<K>>();
            _optionController = optionController;
            if (!_toggleGroup) throw new Exception();
        }
        catch (Exception)
        {
            throw new NotImplementedException($"ToggleGroup<{typeof(K)}> : not found in the {paramName} GameObject at {parentTransformOptions.name} Transform");
        }
    }

    public void Init(Action<K> toggleGroupValueChanged, K initialEnumValue)
    {
        _toggleGroup.onNewValue += ValueChanged;
        _actionAtValueChanged = toggleGroupValueChanged;
        SetValueWithoutNotify(initialEnumValue);
    }

    private void ValueChanged(K value)
    {
        _actionAtValueChanged.Invoke(value);
        _optionController.OptionsChanged(true);
        Dirty = true;
    }

    public void SetValueWithoutNotify(K value)
    {
        _toggleGroup.SetValue(value);
    }

    //public K GetValue()
    //{
    //    return _toggleGroup.GetValue();
    //}
}