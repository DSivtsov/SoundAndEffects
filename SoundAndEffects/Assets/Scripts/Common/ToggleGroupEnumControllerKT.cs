using System;
using UnityEngine;
using GMTools.Menu;

public class ToggleGroupEnumController<K, T> : IUIElement<K> where T : IOptionChanged where K : Enum
{
    private ToggleGroup<K> _toggleGroup;
    private T _optionController;
    private Action<K> _actionAtValueChanged;
    //protected event Action<T> onChangeValue;

    public bool Initialized
    {
        get
        {
            //if (_toggleGroup == null || !_toggleGroup.ElementIsInit)
            //{
            //    Debug.Log($"_toggleGroup == null[{_toggleGroup == null}] !_toggleGroup.ElementIsInitialized[{!_toggleGroup.ElementIsInit}]");
            //}
            return _toggleGroup != null && _toggleGroup.ElementIsInit;
        }
    }

    public bool Dirty { get; private set; }

    public ToggleGroupEnumController(string paramName, Transform parentTransformOptions, T optionController)
    {
        try
        {
            //Debug.Log($"parentTransformOptions.Find(paramName)={parentTransformOptions.Find(paramName)}");
            _toggleGroup = parentTransformOptions.Find(paramName).GetComponentInChildren<ToggleGroup<K>>();
            _optionController = optionController;
            if (!_toggleGroup) throw new Exception();
        }
        catch (Exception)
        {
            throw new NotImplementedException($"ToggleGroup<{typeof(K)}> : not found in the {paramName} GameObject at {parentTransformOptions.name} Transform");
        }
        //Debug.Log($"_toggleGroup.ElementIsInitialized={_toggleGroup.ElementIsInitialized}");
    }

    public void Init(Action<K> toggleGroupValueChanged, K initialEnumValue)
    {
        _toggleGroup.eventActivatedToggle += ValueChanged;
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
        _toggleGroup.SetValueWithoutNotify(value);
    }

    public K GetValue()
    {
        return _toggleGroup.GetValue();
    }
}