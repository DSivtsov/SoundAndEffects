using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TopListController), true)]
public class TopListControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TopListController topListCtrl = target as TopListController;
        DrawDefaultInspector();
        EditorGUILayout.Space();
        //To exclude the possibility to create the temp GameObject in the process of run the Play mode
        //EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
        EditorGUI.BeginDisabledGroup(false);
        if (GUILayout.Button("Load data for TopList"))
        {
            topListCtrl.LoadTopList();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space();
        EditorGUI.BeginDisabledGroup(false);
        if (GUILayout.Button("Update and Show TopList"))
        {
            topListCtrl.InitUpdateAndShowTopList();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space();
        EditorGUI.BeginDisabledGroup(false);
        if (GUILayout.Button("Save data from TopList"))
        {
            topListCtrl.SaveTopList();
        }
        EditorGUI.EndDisabledGroup();
    }
}