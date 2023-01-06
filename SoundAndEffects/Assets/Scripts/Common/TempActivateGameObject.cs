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
        for (int i = 0; i < _tempGameObject.Length; i++)
        {
            string strMsg;
            strMsg = $"Temporary for [{gameObject.scene.name}] Scene was activate [{_tempGameObject[i].name}] GameObject.";
            MonoBehaviour script;
            if (script = _tempGameObject[i].GetComponent<MonoBehaviour>())
            {
                string strPath = AssetDatabase.GetAssetPath(MonoScript.FromMonoBehaviour(script));
                if (strPath.IndexOf("Packages/") == 0)
                {
                    //Debug.Log(strPath);
                    //if object haven't a MonoBehaviour sciprt from Asset folder it will be activate w/o additional checks
                    _tempGameObject[i].SetActive(true);
                    Debug.LogWarning(strMsg);
                    continue;
                }
                Type type = script.GetType();
                UnityEngine.Object[] objects = FindObjectsOfType(type, includeInactive: true);
                if (objects.Length == 1)
                {
                    _tempGameObject[i].SetActive(true);
                    strMsg += $" As a result exist the {objects.Length} GameObjects with [{type}] script";
                }
                else
                    //if can exist two indentical object not create the temporary object
                    continue;
            }
            else
                //if object doesn't have a MonoBehaviour component it will be activate w/o additional checks 
                _tempGameObject[i].SetActive(true);
            Debug.LogWarning(strMsg);
        }
    }
#endif
}
