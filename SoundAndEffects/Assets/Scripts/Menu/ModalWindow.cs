using System;
using UnityEngine;

public enum ModalWindowButtonType
{
    Ok = 1,
    Cancel = 2,
}

abstract public class ModalWindow : MonoBehaviour
{ 
    [SerializeField] private GameObject _modalWindow;
    [SerializeField] private ModalWindowButtonType _AcknButtonType;

    private Canvas _canvasOverlay;
    private Action _actionModalWindow;

    void Awake()
    {
        _canvasOverlay = GetComponent<Canvas>();
        foreach (ModalWindowButton btn in _modalWindow.GetComponentsInChildren<ModalWindowButton>(includeInactive: true))
        {
            btn.LinkToModalWindowsController(this);
        }
    }

    private bool IsTrue(ModalWindowButtonType btnType) => btnType == _AcknButtonType;

    public void ActivateCanvasOverlayWindow(bool activate = true)
    {
        _canvasOverlay.enabled = activate;
        _modalWindow.SetActive(activate);
    }

    public void ButtonWasPressed(ModalWindowButtonType btnType)
    {
        if (IsTrue(btnType))
        {
            _actionModalWindow();
            //Debug.Log($"ButtonWasPressed[{btnType}] menuName[{_modalWindow.name}] will Call action[{_actionModalWindow.Method.Name}]");
        }
        //else
        //    Debug.Log($"ButtonWasPressed[{btnType}] menuName[{_modalWindow.name}] actionPredicate.IsTrue(btnType)[{IsTrue(btnType)}]");
        ActivateCanvasOverlayWindow(activate: false);
    }

    public void SetActionWindowAcknowledgment(Action action) => this._actionModalWindow = action;

}