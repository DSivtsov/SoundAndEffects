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
    NotShowIntroduction = 0b100_000_000,
    NotShowCollisionAnimation = 0b1_000_000_000,
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
    [Header("Video Options")]
    [SerializeField] private bool _notShowIntroduction;
    [SerializeField] private bool _notShowCollisionAnimation;

    public override string ToString() => JsonUtility.ToJson(this);

    public ExposeField<ComplexitySO> FieldComplexityGame { get; private set; }
    public ExposeField<PlayMode> FieldPlayMode { get; private set; }
    public ExposeField<bool> FieldNotCopyToGlobal { get; private set; }
    public ExposeField<bool> FieldByDefaultShowGlobalTopList { get; private set; }
    public ExposeField<SequenceType> FieldSequenceType { get; private set; }
    public ExposeField<int> FieldMasterVolume { get; private set; }
    public ExposeField<int> FieldMusicVolume { get; private set; }
    public ExposeField<int> FieldEffectVolume { get; private set; }
    public ExposeField<bool> FieldNotShowIntroductionText { get; private set; }
    public ExposeField<bool> FieldNotShowCollisionAnimation { get; private set; }

    /// <summary>
    /// Init interface to game setting fields
    /// </summary>
    /// <param name="flagGameSettingChanges"></param>
    public void InitExposedFields(FlagGameSettingChanged flagGameSettingChanges)
    {
        FieldComplexityGame = new ExposeField<ComplexitySO>(() => _complexityGame, (newValue) => _complexityGame = newValue, flagGameSettingChanges,
            GameSettingChangedBit.ComplexityGame);
        FieldPlayMode = new ExposeField<PlayMode>(() => _usedPlayMode, (newValue) => _usedPlayMode = newValue, flagGameSettingChanges,
            GameSettingChangedBit.PlayMode);
        FieldNotCopyToGlobal = new ExposeField<bool>(() => _notCopyToGlobal, (newValue) => _notCopyToGlobal = newValue, flagGameSettingChanges,
            GameSettingChangedBit.NotCopyToGlobal);
        FieldByDefaultShowGlobalTopList = new ExposeField<bool>(() => _defaultTopListGlobal, (newValue) => _defaultTopListGlobal = newValue, flagGameSettingChanges,
            GameSettingChangedBit.ByDefaultShowGlobalTopList);
        FieldSequenceType = new ExposeField<SequenceType>(() => _musicSequenceType, (newValue) => _musicSequenceType = newValue, flagGameSettingChanges,
            GameSettingChangedBit.SequenceType);
        FieldMasterVolume = new ExposeField<int>(() => _masterVolume, (newValue) => _masterVolume = newValue, flagGameSettingChanges,
            GameSettingChangedBit.MasterVolume);
        FieldMusicVolume = new ExposeField<int>(() => _musicVolume, (newValue) => _musicVolume = newValue, flagGameSettingChanges,
            GameSettingChangedBit.MasterVolume);
        FieldEffectVolume = new ExposeField<int>(() => _effectVolume, (newValue) => _effectVolume = newValue, flagGameSettingChanges,
            GameSettingChangedBit.MasterVolume);
        FieldNotShowIntroductionText = new ExposeField<bool>(() => _notShowIntroduction, (newValue) => _notShowIntroduction = newValue, flagGameSettingChanges,
            GameSettingChangedBit.NotShowIntroduction);
        FieldNotShowCollisionAnimation = new ExposeField<bool>(() => _notShowCollisionAnimation, (newValue) => _notShowCollisionAnimation = newValue, flagGameSettingChanges,
            GameSettingChangedBit.NotShowCollisionAnimation);
    }
}