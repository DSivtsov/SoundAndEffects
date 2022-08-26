using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OdinSerializer;
using GMTools;

[CreateAssetMenu(fileName = "NewGameSettingsSO", menuName = "SoundAndEffects/NewGameSettingsSO")]
public class NewGameSettingsSO : ScriptableObject//, ISerializationCallbackReceiver
{
    [Header("Game Options")]
    //[SerializeField, NonOdinSerialized] private LevelSO _levelPlayer;
    [SerializeField] private LevelSO _levelPlayer;
    [SerializeField] private PlayMode _usedPlayMode;
    [SerializeField] private bool _notCopyToGlobal;
    [SerializeField] private bool _globalDefaultTopList;
    [Header("Audio Options")]
    [SerializeField] private float _masterVolume;
    [SerializeField] private float _musicVolume;
    [SerializeField] private float _effectVolume;
    [SerializeField] private SequenceType _musicSequenceType;

    [SerializeField,HideInInspector] private string _levelSOName;

    public override string ToString() => JsonUtility.ToJson(this);
     
    public PlayMode UsedPlayMode => _usedPlayMode;
    public bool NotCopyToGlobal => _notCopyToGlobal;
    public bool GlobalDefaultTopList => _globalDefaultTopList;
    public LevelSO LevelPlayer => _levelPlayer;

    public void ChangeComplexitySO() => _levelPlayer = null;

    //void ISerializationCallbackReceiver.OnAfterDeserialize()
    //{
    //    //if (dict != null)
    //    //{
    //    //    Debug.Log($"OnAfterDeserialize() : dict!=null [{dict != null}]");
    //    //    if (_levelSOName != null && dict != null)
    //    //    {
    //    //        if (dict.TryGetValue(_levelSOName, out LevelSO levelSO))
    //    //        {
    //    //            _levelPlayer = levelSO;
    //    //        }
    //    //        else
    //    //            Debug.LogError($"Can find the LevelSO with {_levelSOName} name");
    //    //    }
    //    //}
    //    //else
    //    //    Debug.Log($"OnAfterDeserialize() : dict==null [{dict == null}]");

    //}

    //void ISerializationCallbackReceiver.OnBeforeSerialize()
    //{
    //    //if (_levelPlayer)
    //    //{
    //    //    _levelSOName = _levelPlayer.name; 
    //    //}
    //    ////Debug.Log("_levelSOName=" + _levelSOName);
    //    //UnitySerializationUtility.SerializeUnityObject(this, ref this.serializationData, false);
    //}

    public string GetStringReference() => _levelPlayer.name;

    public LevelSO ResolveStringReference(string reference)
    {
        if (dict != null)
        {
            if (_levelSOName != null)
            {
                if (dict.TryGetValue(_levelSOName, out LevelSO levelSO))
                {
                    return levelSO;
                }
                else
                    Debug.LogError($"RestoreAfterLoad() : Can find the LevelSO with {_levelSOName} name");
            }
        }
        else
            Debug.Log($"RestoreAfterLoad() : dict==null [{dict == null}]");
        return null;
    }

    public void BeforeSerialize() => _levelSOName = _levelPlayer.name;
    public void AfterSerialize() => _levelPlayer = ResolveStringReference(_levelSOName);
    //{
    //    if (dict != null)
    //    {
    //        if (_levelSOName != null)
    //        {
    //            if (dict.TryGetValue(_levelSOName, out LevelSO levelSO))
    //            {
    //                _levelPlayer = levelSO;
    //            }
    //            else
    //                Debug.LogError($"RestoreAfterLoad() : Can find the LevelSO with {_levelSOName} name");
    //        }
    //    }
    //    else
    //        Debug.Log($"RestoreAfterLoad() : dict==null [{dict == null}]");
    //}

    private void OnEnable()
    {
        if (dict == null)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                FillDictLevelSO();
            }
#else
            FillDictLevelSO();
#endif
        }
    }

    private Dictionary<string, LevelSO> dict;
    private void FillDictLevelSO()
    {
        //Debug.Log("UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode");
        //arrForceJump = Resources.FindObjectsOfTypeAll<LevelSO>();
        //arrForceJump = Resources.LoadAll<LevelSO>("");
        dict = Resources.LoadAll<LevelSO>("").ToDictionary((level) => level.name, (level) => level);
        //CheckArrForceJump(newdict);
        //_listNamesLevelSO = arrForceJump.Select((LevelSO record) => record.name).ToList();
        Debug.Log($"OnEnable() : LevelSO dict filled dict.Count={dict.Count}");
    }

    //private void CheckArrForceJump(ScriptableObject[] arrForceJump)
    //{
    //    Debug.Log($"arrForceJump.Length={arrForceJump.Length}");
    //    for (int i = 0; i < arrForceJump.Length; i++)
    //    {
    //        Debug.Log($"[{i}] = {arrForceJump[i].name}");
    //    }
    //}

    //private LevelSO[] arrForceJump;
    //private List<string> _listNamesLevelSO;
}
