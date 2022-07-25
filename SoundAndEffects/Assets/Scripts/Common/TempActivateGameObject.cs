#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Activate selected GameObject if we can't find any one actine GameObject with same MonoBehaviour script, in Editor only
/// </summary>
public class TempActivateGameObject : MonoBehaviour
{
    [SerializeField] private GameObject[] _tempGameObject;
    private void Awake()
    {
        for (int i = 0; i < _tempGameObject.Length; i++)
        {
            Type type = _tempGameObject[i].GetComponent<MonoBehaviour>().GetType();
            UnityEngine.Object[] objects = FindObjectsOfType(type, includeInactive: false);
            if (objects.Length < 1)
            {
                _tempGameObject[i].SetActive(true);
                Debug.LogWarning($"Temporary for [{gameObject.scene.name}] Scene was activate [{_tempGameObject[i].name}] GameObject with [{type}]");
            }

        }
    }
} 
#endif