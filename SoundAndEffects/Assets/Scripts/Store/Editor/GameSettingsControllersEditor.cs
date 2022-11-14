using UnityEngine;
using UnityEditor;
using GMTools.Manager;

[CustomEditor(typeof(GameSettingsSOController), true)]
public class GameSettingsControllersEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameSettingsSOController gameSettingsController = target as GameSettingsSOController;
        DrawDefaultInspector();

        EditorGUILayout.Space();
        //To exclude the possibility to create the temp GameObject in the process of run the Play mode
        //EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
        EditorGUI.BeginDisabledGroup(false);
        if (GUILayout.Button("Init data"))
        {
            gameSettingsController.InitGameSettings();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
        EditorGUI.BeginDisabledGroup(false);
        if (GUILayout.Button("Save data"))
        {
            gameSettingsController.Save();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
        EditorGUI.BeginDisabledGroup(false);
        if (GUILayout.Button("Load data"))
        {
            gameSettingsController.Load();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
        EditorGUI.BeginDisabledGroup(false);
        if (GUILayout.Button("Reset data"))
        {
            gameSettingsController.LoadDefault();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
        EditorGUI.BeginDisabledGroup(false);
        if (GUILayout.Button("Show data"))
        {
            gameSettingsController.ShowData();
        }
        EditorGUI.EndDisabledGroup();
    }
}