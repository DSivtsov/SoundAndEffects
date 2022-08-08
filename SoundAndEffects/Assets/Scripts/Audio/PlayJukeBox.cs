using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using GMTools;
using System.Collections;

/// <summary>
/// To initializtion and control the JukeBox (middle and right mouse button)
/// </summary>
public class PlayJukeBox : MonoBehaviour
{
    #region SerializedFields
    [SerializeField] protected JukeBoxSO _jukeBox;
    [SerializeField] private AudioMixerGroup _mixerGroup;
    [SerializeField] private AudioMixerGroup _mixerGroup2;
    [SerializeField] private bool _playAtAwake;
    [Range(0, 255)] [SerializeField] private int _sourcePriority;
    #endregion

    #region NonSerializedFields
    public bool JukeBoxPlaying { get; private set; } = false;
    private bool initAudioSources = false;
    private AudioSource[] _audioSources = new AudioSource[2];
    private AudioControls _audioControls;
    private bool _notSkipAudioControls;
    #endregion

    private void Awake()
    {
        if (AudioControls.Instance)
        {
            _audioControls = AudioControls.Instance;
            _notSkipAudioControls = true;
        }
        else
        {
            Debug.LogError($"{this} not linked to AudioControls");
            _notSkipAudioControls = false; 
        }
    }

    private void OnEnable()
    {
        if (_notSkipAudioControls)
        {
            _audioControls.MusicTurnOn += TurnOnButtonPressed;
            _audioControls.MusicSwitchToNextClip += SwitchToNextClipButtonPressed; 
        }
    }

    private void OnDisable()
    {
        if (_notSkipAudioControls)
        {
            _audioControls.MusicTurnOn -= TurnOnButtonPressed;
            _audioControls.MusicSwitchToNextClip -= SwitchToNextClipButtonPressed; 
        }
    }

    private void Start()
    {
        if (_playAtAwake)
        {
            TurnOn();
        }
    }

    private void Update()
    {
        if (JukeBoxPlaying && _jukeBox.IsTimeStartPreparing())
        {
            _jukeBox.PlayScheduledNextClip();
        }
    }

    private void SwitchToNextClipButtonPressed()
    {
        if (JukeBoxPlaying)
            _jukeBox.SwitchToNextClip();
    }

    private void TurnOnButtonPressed()
    {
        if (JukeBoxPlaying)
            TurnOn(false);
        else
            TurnOn(true);
    }

    public virtual SequenceType GetInitialSeqenceType() => _jukeBox.CurrentSeqenceType;

    /// <summary>
    /// Check and inti the SO JukeBox and init AudioSources
    /// </summary>
    private void CheckInitJukeBox()
    {
        if (_jukeBox.ClipsArrayEmpty())
        {
            Debug.LogError($"[{gameObject.name}] AudioEvent disabled because SO with Audio clips is Empty");
            JukeBoxPlaying = false;
        }
        else
        {
            if (!initAudioSources)
            {
                for (int i = 0; i < 2; i++)
                {
                    GameObject child = new GameObject($"Soundtrack{i}");
                    child.transform.parent = gameObject.transform;
                    _audioSources[i] = child.AddComponent<AudioSource>();
                    _audioSources[i].priority = _sourcePriority;
                }
                _audioSources[0].outputAudioMixerGroup = _mixerGroup;
                _audioSources[1].outputAudioMixerGroup = _mixerGroup2;
                initAudioSources = true;
            }
            _jukeBox.InitJukeBox(_audioSources);
            JukeBoxPlaying = true;
        }
    }
    /// <summary>
    /// Try turnOn or turnOff JukeBoxMusic. The seed reinitialized every time
    /// </summary>
    public void TurnOn(bool turnOn = true)
    {
        ///CountFrame.DebugLogUpdate(this, $"MusicPlaying[{JukeBoxPlaying}] turnOn = true[{turnOn}]");
        if (JukeBoxPlaying)
        {
            if (!turnOn)
            {
                for (int i = 0; i < 2; i++)
                {
                    _audioSources[i].Stop();
                }
                JukeBoxPlaying = false;
            }
        }
        else
        {
            if (turnOn)
            {
                CheckInitJukeBox();
                if (JukeBoxPlaying)
                    _jukeBox.PlayClipNextInit();
            }
        }
    }

    public void SwitchJukeBoxSequenceType(SequenceType newSequenceType)
    {
        //Debug.Log($"{this} : PlayJukeBox.SwitchJukeBoxSequenceType()");
        bool initialMusicState = JukeBoxPlaying;
        if (initialMusicState)
            TurnOn(false);
        foreach (JukeBoxSO item in GetUsedSequenceType())
        {
            item.SetNewSequenceType(newSequenceType);
        }
        if (initialMusicState)
            TurnOn(true);
    }

    protected virtual IEnumerable<JukeBoxSO> GetUsedSequenceType()
    {
        //Debug.Log($"{this} : PlayJukeBox.GetUsedSequenceType");
        yield return _jukeBox;
    }

}

