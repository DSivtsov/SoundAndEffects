using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Complexity", menuName = "SoundAndEffects/Complexity")]
public class ComplexitySO : ScriptableObject//, IStringReferenceResolver<ComplexitySO>
{
//    private static readonly object lockObject = new object();

//    private void OnEnable()
//    {
//        lock (lockObject)
//        {
//            if (_init)
//                return;
//            else
//                _init = true;
//        }
//        if (dict == null)
//        {
//#if UNITY_EDITOR
//            if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
//            {
//                FillDict();
//            }
//#else
//            FillDict();
//#endif
//        }
//    }

//    private static bool _init;

//    private static Dictionary<string, ComplexitySO> dict;
//    private void FillDict()
//    {
//        dict = Resources.LoadAll<ComplexitySO>("").ToDictionary((SO) => SO.name, (SO) => SO);
//        Debug.Log($"OnEnable() : {typeof(ComplexitySO)} dict filled by SO, count={dict.Count}");
//    }

//    string IStringReferenceResolver<ComplexitySO>.GetStringReference() => this.name;

//    ComplexitySO IStringReferenceResolver<ComplexitySO>.ResolveStringReference(string name)
//    {
//        if (dict != null)
//        {
//            if (name != null)
//            {
//                if (dict.TryGetValue(name, out ComplexitySO SO))
//                {
//                    return SO;
//                }
//                else
//                    Debug.LogError($"RestoreAfterLoad() : Can find the LevelSO with {name} name");
//            }
//        }
//        else
//            Debug.Log($"RestoreAfterLoad() : dict==null [{dict == null}]");
//        return null;
//    }
}
