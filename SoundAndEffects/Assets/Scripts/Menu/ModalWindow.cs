using System;
using UnityEngine;

public enum ModalWindowButtonType
{
    Ok = 1,
    Cancel = 2,
    Close = 3,
}

abstract public class ModalWindow : MonoBehaviour
{ 
    [SerializeField] protected GameObject _modalWindow;

    private Canvas _canvasOverlay;
    protected Action _actionBeforeDeactivateModalWindow;
    protected ModalWindowButtonType _buttonWasPressed;

    protected void Awake()
    {
        _canvasOverlay = GetComponent<Canvas>();
        foreach (ModalWindowButton btn in _modalWindow.GetComponentsInChildren<ModalWindowButton>(includeInactive: true))
            btn.LinkToModalWindowsController(this);
    }

    public void ActivateCanvasOverlayWindow(bool activate = true)
    {
        _canvasOverlay.enabled = activate;
        _modalWindow.SetActive(activate);
    }

    public void ButtonWasPressed(ModalWindowButtonType btnType)
    {
        _buttonWasPressed = btnType;
        ActionAfterButtonWasPressed();
    }
    /// <summary>
    /// Base ActionAfterButtonWasPressed() deactivate ModalWindow only, special action (_actionBeforeDeactivateModalWindow) must be call before this
    /// </summary>
    protected virtual void ActionAfterButtonWasPressed() => ActivateCanvasOverlayWindow(activate: false);

    public void SetActionBeforeDeactivationModalWindow(Action action) => this._actionBeforeDeactivateModalWindow = action;
}