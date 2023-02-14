using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalWindowCloseWithToggle : ModalWindow
{
    private Toggle _toggleDontShowAgain;
    private new void Awake()
    {
        base.Awake();
        _toggleDontShowAgain = _modalWindow.GetComponentInChildren<Toggle>();
    }

    protected override void ActionAfterButtonWasPressed()
    {
        if (_toggleDontShowAgain.isOn)
        {
            _actionBeforeDeactivateModalWindow();
        }
        base.ActionAfterButtonWasPressed();
    }
}
