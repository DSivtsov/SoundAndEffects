using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools;
using UnityEngine.Audio;

[Flags]
public enum GameSettingChangedBit
{
    NoChanges = 0b00,
    ComplexityGame = 0b01,
    PlayMode = 0b10,
    NotCopyToGlobal = 0b100,
    ByDefaultShowGlobalTopList = 0b1_000,
    SequenceType = 0b10_000,
    MasterVolume = 0b100_000,
    MusicVolume = 0b1_000_000,
    EffectVolume = 0b10_000_000,
}

[CreateAssetMenu(fileName = "GameSettingsSO", menuName = "SoundAndEffects/GameSettingsSO")]
public class GameSettingsSO : ScriptableObject
{
    [Header("Game Options")]
    [SerializeField] private ComplexitySO _complexityGame;
    [SerializeField] private PlayMode _usedPlayMode;
    [SerializeField] private bool _notCopyToGlobal;
    [SerializeField] private bool _defaultTopListGlobal;
    [Header("Audio Options")]
    [SerializeField] private SequenceType _musicSequenceType;
    [SerializeField] private int _masterVolume;
    [SerializeField] private int _effectVolume;
    [SerializeField] private int _musicVolume;

    public override string ToString() => JsonUtility.ToJson(this); 

    //public ComplexitySO ComplexityGame => _complexityGame;
    //public PlayMode UsedPlayMode => _usedPlayMode;
    public bool NotCopyToGlobal => _notCopyToGlobal;
    public bool DefaultTopListGlobalt => _defaultTopListGlobal;

    public ExposeField<ComplexitySO> FieldComplexityGame { get; private set; }
    public ExposeField<PlayMode> FieldPlayMode { get; private set; }
    public ExposeField<bool> FieldNotCopyToGlobal { get; private set; }
    public ExposeField<bool> FieldByDefaultShowGlobalTopList { get; private set; }
    public ExposeField<SequenceType> FieldSequenceType { get; private set; }
    public ExposeField<int> FieldMasterVolume { get; private set; }
    public ExposeField<int> FieldMusicVolume { get; private set; }
    public ExposeField<int> FieldEffectVolume { get; private set; }

    /// <summary>
    /// Init interface to game setting fields
    /// </summary>
    /// <param name="_flagGameSettingChanges"></param>
    public void InitExposedFields(FlagGameSettingChanged _flagGameSettingChanges, AudioContoller _audioContoller)
    {
        FieldComplexityGame = new ExposeField<ComplexitySO>(() => _complexityGame, (newValue) => _complexityGame = newValue, _flagGameSettingChanges, GameSettingChangedBit.ComplexityGame);
        FieldPlayMode = new ExposeField<PlayMode>(() => _usedPlayMode, (newValue) => _usedPlayMode = newValue, _flagGameSettingChanges, GameSettingChangedBit.PlayMode);
        FieldNotCopyToGlobal = new ExposeField<bool>(() => _notCopyToGlobal, (newValue) => _notCopyToGlobal = newValue, _flagGameSettingChanges, GameSettingChangedBit.NotCopyToGlobal);
        FieldByDefaultShowGlobalTopList = new ExposeField<bool>(() => _defaultTopListGlobal, (newValue) => _defaultTopListGlobal = newValue, _flagGameSettingChanges,
            GameSettingChangedBit.ByDefaultShowGlobalTopList);
        FieldSequenceType = new ExposeField<SequenceType>(() => _musicSequenceType, (newValue) => { _musicSequenceType = newValue; _audioContoller.SetSequenceType(newValue); },
            _flagGameSettingChanges, GameSettingChangedBit.SequenceType);
        FieldMasterVolume = new ExposeField<int>(() => _masterVolume, (newValue) => { _masterVolume = newValue; _audioContoller.SetMixerVolume(MixerVolume.VolMaster, newValue); },
            _flagGameSettingChanges, GameSettingChangedBit.MasterVolume);
        FieldMusicVolume = new ExposeField<int>(() => _musicVolume, (newValue) => { _musicVolume = newValue; _audioContoller.SetMixerVolume(MixerVolume.VolMusic, newValue); },
            _flagGameSettingChanges, GameSettingChangedBit.MasterVolume);
        FieldEffectVolume = new ExposeField<int>(() => _effectVolume, (newValue) => { _effectVolume = newValue; _audioContoller.SetMixerVolume(MixerVolume.VolEffects, newValue); },
            _flagGameSettingChanges, GameSettingChangedBit.MasterVolume);
    }
}