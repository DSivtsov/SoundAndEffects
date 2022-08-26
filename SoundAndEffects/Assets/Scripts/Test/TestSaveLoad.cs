// <copyright file="TestSaveLoad.cs">
//  Copyright (c) 2022 Denis Sivtsov.
//  Licensed under the Apache License, Version 2.0
// </copyright>
//#define STORE_UNITYObj_AS_PLAIN
#define STORE_UnityObject
using System.Collections.Generic;
using UnityEngine;

public class TestSaveLoad : MonoBehaviour
{
    private const string FileExtension = ".txt";
    private const string FilePathBasePartStrDRef = "OdinStrRef";
    private const string FilePathBasePartGuiDRef = "OdinGuiDRef";
    private const string FilePathGuiDRef = FilePathBasePartGuiDRef + FileExtension;
    private const string FilePathMainFile = "TestOdin.txt";
    private const string FilePathMainFileNewGameSettings = "TestOdinNewGameSettings.txt";

    [SerializeField] private NewGameSettingsSO _defaultNewGameSettingsSO;

    //[SerializeField] private GameSettings _defaultgameSettings;

    [SerializeField] private List<Object> unityReferences = new List<Object>();

	private void OnGUI()
	{
#if STORE_UNITYObj_AS_PLAIN
        if (GUI.Button(new Rect(10, 70, 100, 30), "Save"))
        {
            Debug.Log("Save");
            //OdinCallsExample.Save(_defaultgameSettings, FilePathMainFile, ref unityReferences);
            OdinCallsExample.Save(_defaultNewGameSettingsSO, FilePathMainFile, ref unityReferences);
        }

        if (GUI.Button(new Rect(350, 70, 100, 30), "Load"))
        {
            Debug.Log("Load");
            _defaultNewGameSettingsSO = OdinCallsExample.Load<NewGameSettingsSO>(FilePathMainFile, unityReferences);
            Debug.Log(_defaultNewGameSettingsSO);
        }
#endif
#if STORE_UnityObject
        if (GUI.Button(new Rect(10, 70, 100, 30), "SaveUnityObject"))
        {
            Debug.Log("SaveUnityObject");
            Debug.Log(_defaultNewGameSettingsSO);
            OdinCallsExample.SaveUnityObject(_defaultNewGameSettingsSO, FilePathMainFileNewGameSettings, ref unityReferences);
        }

        if (GUI.Button(new Rect(350, 70, 100, 30), "LoadUnityObject"))
        {
            Debug.Log("LoadUnityObject");
            OdinCallsExample.LoadUnityObject(_defaultNewGameSettingsSO, FilePathMainFileNewGameSettings, ref unityReferences);
            Debug.Log(_defaultNewGameSettingsSO);
        }
#endif
#if TWOSTEP_SER
        if (GUI.Button(new Rect(10, 70, 100, 30), "SaveTwoSteps"))
        {
            Debug.Log("Save");
            OdinCallsExample.SaveTwoSteps(_defaultgameSettings, FilePathMainFile, ref unityReferences);
        }

        if (GUI.Button(new Rect(250, 70, 100, 30), "ShowComplexityGame"))
        {
            Debug.Log($"Complexity={_defaultgameSettings.ComplexityGame} {_defaultgameSettings}");
        }

        if (GUI.Button(new Rect(350, 70, 100, 30), "LoadTwoSteps"))
        {
            Debug.Log("Load");
            _defaultgameSettings = OdinCallsExample.LoadTwoSteps<GameSettings>(FilePathMainFile, unityReferences);
            Debug.Log(_defaultgameSettings);
        }

        if (GUI.Button(new Rect(10, 100, 100, 30), "SaveStrRef"))
        {
            Debug.Log("SaveStrRef");
            OdinCallsExample.SaveStrRef((FilePathBasePartStrDRef, FileExtension), unityReferences);
        }

        if (GUI.Button(new Rect(350, 100, 100, 30), "LoadStrRef"))
        {
            Debug.Log("LoadStrRef");
            OdinCallsExample.LoadStrRef((FilePathBasePartStrDRef, FileExtension), unityReferences);
        }
#endif
#if TWOSTEP_SER || STORE_UNITYObj_AS_PLAIN || STORE_UnityObject
        if (GUI.Button(new Rect(250, 70, 100, 30), "ShowUnityObject"))
        {
            Debug.Log("ShowUnityObject");
            Debug.Log($"LevelPlayer={_defaultNewGameSettingsSO.LevelPlayer}");
            Debug.Log(_defaultNewGameSettingsSO);
        }
#endif




#if EditorOnlyStoreOdinClassicRefResolvers
        if (GUI.Button(new Rect(10, 130, 100, 30), "SaveGuiDRef"))
        {
            Debug.Log("SaveGuiDRef");
            OdinCallsExample.SaveGuiDRef(FilePathGuiDRef, unityReferences);
        }

        if (GUI.Button(new Rect(350, 130, 100, 30), "LoadGuiDRef"))
        {
            Debug.Log("LoadGuiDRef");
            OdinCallsExample.LoadGuiDRef(FilePathGuiDRef, unityReferences);
            //unityReferences.Add(OdinCallsExample.LoadGuiDRef(FilePathGuiDRef));
        }
#endif
    }
#if EditorOnlyStoreOdinClassicRefResolvers
    private void Awake()
	{
		//Debug.Log($"Complexity={_defaultgameSettings?.ComplexityGame} {_defaultgameSettings}");
		//Debug.Log("LoadGuiDRef");
		//unityReferences.Add(Example.LoadGuiDRef("TestOdinGuiDRef.txt"));
		//Debug.Log("Load");
		//var t = Example.Load("TestOdin.txt", unityReferences);
		//Debug.Log(t);
		//_defaultgameSettings = t;
		//Debug.Log($"Complexity={_defaultgameSettings?.ComplexityGame} {_defaultgameSettings}");
    }
#endif
}
