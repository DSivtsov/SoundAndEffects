using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalWindowOK : ModalWindow
{
    protected override void ActionAfterButtonWasPressed()
    {
        if (_buttonWasPressed == ModalWindowButtonType.Ok)
        {
            _actionBeforeDeactivateModalWindow();
        }
        base.ActionAfterButtonWasPressed();
    }
}
