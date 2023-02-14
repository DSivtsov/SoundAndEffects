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

public class AudioContoller : SingletonController<AudioContoller>
{
    [SerializeField] private GameSettingsSO _gameSettings;
    [SerializeField] private AudioMixer _mixerMain;
    [SerializeField] private PlayJukeBox _initialPlayJukeBox;

    private string[] _mixerVolumes;
    private PlayJukeBox[] _arrPlayJukeBoxes;
    private MainManager _mainManager;

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
        SetSequenceType(_gameSettings.FieldSequenceType.GetCurrentValue(), true);
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
    /// <param name="skipInitial">don't set the value for _initialPlayJukeBox</param>
    public void SetSequenceType(SequenceType sequenceType, bool skipInitial = false)
    {
        for (int i = 0; i < _arrPlayJukeBoxes.Length; i++)
        {
            //to escape the restarting of currently playing music
            if (skipInitial && _arrPlayJukeBoxes[i] == _initialPlayJukeBox) continue;
            _arrPlayJukeBoxes[i].SwitchJukeBoxSequenceType(sequenceType);
        }
    }
}
