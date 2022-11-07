using UnityEngine;
using UnityEditor;
using GMTools.Manager;

[CustomEditor(typeof(GameSettingsController), true)]
public class GameSettingsControllersEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GameSettingsController gameSettingsController = target as GameSettingsController;
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
            gameSettingsController.Reset();
        }
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
        EditorGUI.BeginDisabledGroup(false);
        if (GUILayout.Button("Show data"))
        {
            Debug.Log(gameSettingsController.GameSettings.ToString());
        }
        EditorGUI.EndDisabledGroup();
    }
}