using GMTools.Menu.Elements;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ToggleBool : MonoBehaviour, IElement<bool>
{
    private Toggle _toggle;
    private bool _toggleIsInit = false;

    public event Action<bool> onNewValue;

    private void Awake()
    {
        if (!_toggleIsInit)
        {
            InitElement();
            _toggleIsInit = true;
        }
    }

    public void InitElement()
    {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener((toggleIsOn) => onNewValue?.Invoke(toggleIsOn));
    }

    public void SetValue(bool value)
    {
        _toggle.SetIsOnWithoutNotify(value);
    }
}
