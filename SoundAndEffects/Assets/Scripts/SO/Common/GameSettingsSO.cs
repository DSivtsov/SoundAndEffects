using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools;

[CreateAssetMenu(fileName = "GameSettingsSO", menuName = "SoundAndEffects/GameSettingsSO")]
public class GameSettingsSO : ScriptableObject
{
    [Header("Game Options")]
    [SerializeField] private ComplexitySO _complexityGame;
    [SerializeField] private PlayMode _usedPlayMode;
    [SerializeField] private bool _notCopyToGlobal;
    [SerializeField] private bool _globalDefaultTopList;
    [Header("Audio Options")]
    [Range(-80,20)]
    [SerializeField] private int _masterVolume;
    [Range(-80, 20)]
    [SerializeField] private int _musicVolume;
    [Range(-80, 20)]
    [SerializeField] private int _effectVolume;
    [SerializeField] private SequenceType _musicSequenceType;

    public override string ToString() => JsonUtility.ToJson(this);


    public PlayMode UsedPlayMode => _usedPlayMode;
    public bool NotCopyToGlobal => _notCopyToGlobal;
    public bool GlobalDefaultTopList => _globalDefaultTopList;
    public ComplexitySO ComplexityGame => _complexityGame;

    public void ChangeComplexitySO() => _complexityGame = null;
}