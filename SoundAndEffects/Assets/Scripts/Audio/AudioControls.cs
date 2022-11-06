#define OWNCONTROL  //use own input controller of InputSystem temporary solution for simplicity
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using GMTools;
using System.Collections;

public class AudioControls : SingletonController<AudioControls>
{
    #region NonSerializedFields
    public event Action MusicSwitchToNextClip;
    public event Action MusicTurnOnOFF;
    #endregion

    private void Start()
    {
#if OWNCONTROL
        //Audio control use separated controller from Assets/Controls/MyControls.inputactions
        Debug.LogWarning($"Was Activated the own Audio control in [{this}] at [{this.gameObject.scene.name}] Scene");
#endif
    }

    private void Update()
    {
#if OWNCONTROL
        if (Mouse.current.rightButton.wasPressedThisFrame)
            MusicSwitchToNextClip.Invoke();

        if (Mouse.current.middleButton.wasPressedThisFrame)
            MusicTurnOnOFF.Invoke();
#endif
    }
}
