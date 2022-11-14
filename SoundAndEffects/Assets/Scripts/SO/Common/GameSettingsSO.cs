using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools;

[Flags]
public enum GameSettingChangedBit
{
    NoChanges = 0b00,
    ComplexityGame = 0b01,
    PlayMode = 0b10,
    NotCopyToGlobal = 0b100,
    ByDefaultShowGlobalTopList = 0b1000,
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
    [Range(-80,20)]
    [SerializeField] private int _masterVolume;
    [Range(-80, 20)]
    [SerializeField] private int _musicVolume;
    [Range(-80, 20)]
    [SerializeField] private int _effectVolume;
    [SerializeField] private SequenceType _musicSequenceType;

    public override string ToString() => JsonUtility.ToJson(this); 

    public ComplexitySO ComplexityGame => _complexityGame;
    public PlayMode UsedPlayMode => _usedPlayMode;
    public bool NotCopyToGlobal => _notCopyToGlobal;
    public bool DefaultTopListGlobalt => _defaultTopListGlobal;

    public ExposeField<ComplexitySO> FieldComplexityGame { get; private set; }
    public ExposeField<PlayMode> FieldPlayMode { get; private set; }
    public ExposeField<bool> FieldNotCopyToGlobal { get; private set; }
    public ExposeField<bool> FieldByDefaultShowGlobalTopListl { get; private set; }

    public void InitExposedFields(FlagGameSettingChanged _flagGameSettingChanges)
    {
        FieldComplexityGame = new ExposeField<ComplexitySO>(() => _complexityGame, (newValue) => _complexityGame = newValue, _flagGameSettingChanges, GameSettingChangedBit.ComplexityGame);
        FieldPlayMode = new ExposeField<PlayMode>(() => _usedPlayMode, (newValue) => _usedPlayMode = newValue, _flagGameSettingChanges, GameSettingChangedBit.PlayMode);
        FieldNotCopyToGlobal = new ExposeField<bool>(() => _notCopyToGlobal, (newValue) => _notCopyToGlobal = newValue, _flagGameSettingChanges, GameSettingChangedBit.NotCopyToGlobal);
        FieldByDefaultShowGlobalTopListl = new ExposeField<bool>(() => _defaultTopListGlobal, (newValue) => _defaultTopListGlobal = newValue, _flagGameSettingChanges,
            GameSettingChangedBit.ByDefaultShowGlobalTopList);
    }

}