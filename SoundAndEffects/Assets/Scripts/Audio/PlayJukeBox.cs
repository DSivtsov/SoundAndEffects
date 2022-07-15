#define OWNCONTROL  //use own input controller of InputSystem
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using GMTools;

/// <summary>
/// To initializtion and control the JukeBox (middle and right mouse button)
/// </summary>
public class PlayJukeBox : MonoBehaviour
{
    #region SerializedFields
    [SerializeField] private JukeBoxSO _jukeBox;
    [SerializeField] private AudioMixerGroup _mixerGroup;
    [SerializeField] private AudioMixerGroup _mixerGroup2;
    [SerializeField] private bool _playAtAwake;
    [Range(0, 255)] [SerializeField] private int _sourcePriority;
    #endregion

    #region NonSerializedFields
    public bool MusicState { get; private set; } = false;
    private bool initAudioSources = false;
    private AudioSource[] audioSources = new AudioSource[2];
    #endregion

    private void Start()
    {
        if (_playAtAwake)
        {
            TurnOn();
        }
#if OWNCONTROL
        Debug.LogWarning($"Was Activated the own control in [{this}]");
#endif
    }

    private void Update()
    {
#if OWNCONTROL
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (MusicState)
                _jukeBox.SwitchToNextClip();
        }

        if (Mouse.current.middleButton.wasPressedThisFrame)
        {
            if (MusicState)
                TurnOn(false);
            else
                TurnOn(true);
        } 
#endif
        if (MusicState && _jukeBox.IsTimeStartPreparing())
        {
            _jukeBox.PlayScheduledNextClip();
        }
    }

    /// <summary>
    /// Check and inti the SO JukeBox and init AudioSources
    /// </summary>
    private void CheckInitJukeBox()
    {
        if (_jukeBox.ClipsArrayEmpty())
        {
            Debug.LogError($"[{gameObject.name}] AudioEvent disabled because SO with Audio clips is Empty");
            MusicState = false;
        }
        else
        {
            if (!initAudioSources)
            {
                for (int i = 0; i < 2; i++)
                {
                    GameObject child = new GameObject($"Soundtrack{i}");
                    child.transform.parent = gameObject.transform;
                    audioSources[i] = child.AddComponent<AudioSource>();
                    audioSources[i].priority = _sourcePriority;
                }
                audioSources[0].outputAudioMixerGroup = _mixerGroup;
                audioSources[1].outputAudioMixerGroup = _mixerGroup2;
                initAudioSources = true;
            }
            //_jukeBox.InitJukeBox(audioSources, _sequenceType, initValue == 0 ? null : (int?)initValue);
            _jukeBox.InitJukeBox(audioSources);
            MusicState = true;
        }
    }
    /// <summary>
    /// Try turnOn or turnOff JukeBoxMusic. The seed reinitialized every time
    /// </summary>
    public void TurnOn(bool turnOn = true)
    {
        if (!MusicState)
        {
            if (turnOn)
            {
                CheckInitJukeBox();
                if (MusicState)
                    _jukeBox.PlayClipNextInit();  
            }
        }
        else
        {
            if (!turnOn)
            {
                for (int i = 0; i < 2; i++)
                {
                    audioSources[i].Stop();
                }
                MusicState = false; 
            }
        }
    }
}
