using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Menu.Elements;

public class ValueableSlider<T> : MonoBehaviour, IElement<T> where T : struct, IConvertible
{
    UnityEngine.UI.Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<UnityEngine.UI.Slider>();
        _slider.wholeNumbers = true;
    }

    private void OnEnable()
    {
        if (typeof(T) == typeof(int))
            {
                _slider.onValueChanged.AddListener((floatValue) =>
                 {
                     
                     T value = (T)(object)(int)Math.Round(floatValue);
                     onNewValue.Invoke(value);
                 }); 
            }
    }

    public event Action<T> onNewValue;

    public void SetValue(T value)
    {
        Type.GetTypeCode(value.GetType());
        switch (value)
        {
            case int intValue:
                _slider.value = intValue;
                break;
            case float floatValue:
                _slider.value = floatValue;
                break;
            default:
                Debug.LogWarning($"Wrong T Type - is not int or float");
                break;
        }
    }

    public void InitElement()
    {
        _slider = GetComponent<UnityEngine.UI.Slider>();
        _slider.wholeNumbers = true;
    }
}
