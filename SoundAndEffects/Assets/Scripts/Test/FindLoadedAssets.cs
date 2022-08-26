using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;
/// <summary>
/// Find loaded assets in Runtime
/// </summary>
public class FindLoadedAssets : MonoBehaviour
{
    public List<Object> objectsInScene = new List<Object>();

    public List<Object> objectsMinInScene = new List<Object>();

    private void Awake()
    {
        FillArrayObjects();

    }

    private void FillArrayObjects()
    {
        //Func<Object, bool> func = (go) => EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable
        //|| go.hideFlags == HideFlags.HideAndDontSave);

        //GetNonSceneObjects(func, objectsInScene);

        Func<Object, bool> func2 = (_) => true;

        GetNonSceneObjects(func2, objectsMinInScene);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(500, 140, 100, 30), "FillArrayObjects"))
        {
            Debug.Log("FillArrayObjects");
            FillArrayObjects();
        }
    }

    private void GetNonSceneObjects(Func<Object, bool> func, List<Object> objectsInScene)
    {
        objectsInScene.Clear();
        foreach (Object go in Resources.FindObjectsOfTypeAll(typeof(AudioClip)) as Object[])
        {
            if (func(go))
                objectsInScene.Add(go);
        }
    }
}