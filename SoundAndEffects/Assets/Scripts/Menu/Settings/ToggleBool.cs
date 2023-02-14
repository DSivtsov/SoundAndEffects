using GMTools.Menu.Elements;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleBool : MonoBehaviour, IElement<bool>
{
    protected Toggle _toggle;
    private bool _toggleIsInit = false;

    public event Action<bool> onNewValue;

    private void Awake()
    {
        InitElement();
    }

    public void InitElement()
    {
        if (!_toggleIsInit)
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener((toggleIsOn) => onNewValue?.Invoke(toggleIsOn));
            _toggleIsInit = true;
        }
    }

    public void SetValue(bool value)
    {
        _toggle.SetIsOnWithoutNotify(value);
    }
}
