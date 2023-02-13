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
    [SerializeField] private ModalWindowButtonType _AcknButtonType;

    private Canvas _canvasOverlay;
    protected Action _actionModalWindow;

    protected void Awake()
    {
        _canvasOverlay = GetComponent<Canvas>();
        foreach (ModalWindowButton btn in _modalWindow.GetComponentsInChildren<ModalWindowButton>(includeInactive: true))
        {
            btn.LinkToModalWindowsController(this);
        }
    }

    protected bool IsTrue(ModalWindowButtonType btnType) => btnType == _AcknButtonType;

    public void ActivateCanvasOverlayWindow(bool activate = true)
    {
        _canvasOverlay.enabled = activate;
        _modalWindow.SetActive(activate);
    }

    public virtual void ButtonWasPressed(ModalWindowButtonType btnType)
    {
        if (IsTrue(btnType))
        {
            _actionModalWindow();
        }
        ActivateCanvasOverlayWindow(activate: false);
    }

    public void SetActionWindowAcknowledgment(Action action) => this._actionModalWindow = action;

}