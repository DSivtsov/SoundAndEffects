using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModalWindowIntroduction : ModalWindow
{
    private Toggle _toggleDontShowAgain;
    private new void Awake()
    {
        base.Awake();
        _toggleDontShowAgain = _modalWindow.GetComponentInChildren<Toggle>();
        //Debug.Log($"_toggleDontShowAgain.name[{_toggleDontShowAgain.name}] isOn[{_toggleDontShowAgain.isOn}]");
    }

    public override void ButtonWasPressed(ModalWindowButtonType btnType)
    {
        if (IsTrue(btnType))
        {
            if (_toggleDontShowAgain.isOn)
            {
                Debug.Log($"_toggleDontShowAgain.name[{_toggleDontShowAgain.name}] isOn[{_toggleDontShowAgain.isOn}]");
                _actionModalWindow();
            }
        }
        ActivateCanvasOverlayWindow(activate: false);
    }
}
