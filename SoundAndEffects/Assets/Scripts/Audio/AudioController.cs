using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using GMTools;

public enum MixerVolume
{
    VolMaster,
    VolMusic,
    VolEffects
}
[Flags]
public enum StatusInitialization : byte
{
    NothingInited = 0b00,
    AudioContollerInited = 0b01,
    SettingsSectionManagerInited = 0b10,
    AllInited = 0b11,
}

public class AudioController : SingletonController<AudioController>
{
    [SerializeField] private GameSettingsSO _gameSettings;
    [SerializeField] private AudioMixer _mixerMain;
    [SerializeField] private PlayJukeBox _initialPlayJukeBox;

    private string[] _mixerVolumes;
    private PlayJukeBox[] _arrPlayJukeBoxes;
    private MainManager _mainManager;
    /// <summary>
    /// Before Finishing Initialization of AudioController and SettingsSectionsManager the switching current playing is disabled
    /// </summary>
    private bool _skipSwitchCurrentPlaying = true;
    private StatusInitialization _statusInitialization = StatusInitialization.NothingInited;
    protected override void Awake()
    {
        base.Awake();

        _mixerVolumes = Enum.GetNames(typeof(MixerVolume));
        _mainManager = MainManager.Instance;
    }
    private void Start()
    {
        StartCoroutine(InitSequenceTypeOptionCoroutine());
    }
    /// <summary>
    /// Set the initial volumes and sequence type based on game settings values
    /// </summary>
    public void InitAudioByValGameSettings()
    {
        SetMixerVolume(MixerVolume.VolMaster, _gameSettings.FieldMasterVolume.GetCurrentValue());
        SetMixerVolume(MixerVolume.VolMusic, _gameSettings.FieldMusicVolume.GetCurrentValue());
        SetMixerVolume(MixerVolume.VolEffects, _gameSettings.FieldEffectVolume.GetCurrentValue());
        _initialPlayJukeBox.SwitchJukeBoxSequenceType(_gameSettings.FieldSequenceType.GetCurrentValue());
    }

    public void SetMixerVolume(MixerVolume volume, float value)
    {
        _mixerMain.SetFloat(_mixerVolumes[(int)volume], value);
    }

    /// <summary>
    /// InitSequenceTypeOptionCoroutine() postpone the FillArrPlayJukeBox() and set SequenceType till the all Scenes with PlayJukeBox will be loaded
    /// </summary>
    /// <returns></returns>
    private IEnumerator InitSequenceTypeOptionCoroutine()
    {
        do
        {
            yield return null;
        } while (!_mainManager.GetStatusLoadingScenes());
        FillArrPlayJukeBox();
        SetSequenceType(_gameSettings.FieldSequenceType.GetCurrentValue());
        SetFinishingInitialization(StatusInitialization.AudioContollerInited);
    }

    /// <summary>
    /// Will be search for all MonoBehaviours with PlayJukeBox Base class  in all loasded Scenes
    /// </summary>
    public void FillArrPlayJukeBox()
    {
        _arrPlayJukeBoxes = UnityEngine.Object.FindObjectsOfType<PlayJukeBox>(includeInactive: true);
        //Debug.LogWarning($"FillArrPlayJukeBox() : _arrPlayJukeBoxes.Length={_arrPlayJukeBoxes.Length}");
        if (_arrPlayJukeBoxes.Length == 0)
            throw new Exception($"FillArrPlayJukeBox() : _arrPlayJukeBoxes.Length == 0");
    }

    /// <summary>
    /// Change SequenceType for PlayJukeBoxes in game
    /// </summary>
    /// <param name="sequenceType"></param>
    /// <param name="skipCurrentPlaying">don't set the value for _initialPlayJukeBox</param>
    public void SetSequenceType(SequenceType sequenceType)
    {
        for (int i = 0; i < _arrPlayJukeBoxes.Length; i++)
        {
            //to escape the restarting of currently playing music
            if (_skipSwitchCurrentPlaying && _arrPlayJukeBoxes[i] == _initialPlayJukeBox) continue;
            _arrPlayJukeBoxes[i].SwitchJukeBoxSequenceType(sequenceType);
        }
    }

    public void SetFinishingInitialization(StatusInitialization status)
    {
        _statusInitialization |= status;
        _skipSwitchCurrentPlaying = !(_statusInitialization == StatusInitialization.AllInited);
    }
}
