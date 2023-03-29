using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/// <summary>
/// Activate selected GameObject if we can't find any one actine GameObject with same MonoBehaviour script, in Editor only
/// </summary>
public class TempActivateGameObject : MonoBehaviour
{
    [SerializeField] private GameObject[] _tempGameObject;
#if UNITY_EDITOR
    private void Awake()
    {
        string strMsg;
        for (int i = 0; i < _tempGameObject.Length; i++)
        {
            strMsg = $"Temporary for [{gameObject.scene.name}] Scene was activate [{_tempGameObject[i].name}] GameObject.";
            MonoBehaviour script;
            if (script = _tempGameObject[i].GetComponent<MonoBehaviour>())
            {
                string strPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(script));
                if (strPath.IndexOf("Packages/") == 0)
                {
                    //if object haven't a MonoBehaviour sciprt from Asset folder it will be activate w/o additional checks
                    Debug.LogWarning(strMsg);
                    _tempGameObject[i].SetActive(true);
                    continue;
                }
                Type type = script.GetType();
                UnityEngine.Object[] objects = FindObjectsOfType(type, includeInactive: true);
                if (objects.Length == 1)
                {
                    Debug.LogWarning($"{strMsg} As a result exist the {objects.Length} GameObjects with [{type}] script");
                    _tempGameObject[i].SetActive(true);
                }
                else
                    //if can exist two indentical object not create the temporary object
                    continue;
            }
            else
            {
                //if object doesn't have a MonoBehaviour component it will be activate w/o additional checks 
                Debug.LogWarning(strMsg);
                _tempGameObject[i].SetActive(true); 
            }
        }
    }
#endif
}
