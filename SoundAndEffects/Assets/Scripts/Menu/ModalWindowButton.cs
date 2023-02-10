using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ModalWindowButton : MonoBehaviour
{
    [SerializeField] ModalWindowButtonType _btnType;

    private Button _button;
    private ModalWindow _modalWindow;

    void Awake() => _button = GetComponent<Button>();

    private void OnEnable() => _button.onClick.AddListener(ButtonPressed());
    private void OnDisable() => _button.onClick.RemoveListener(ButtonPressed());

    public void LinkToModalWindowsController(ModalWindow modalWindow)
    {
        _modalWindow = modalWindow;
    }

    private UnityAction ButtonPressed() => () => _modalWindow.ButtonWasPressed(_btnType);
}
