using UnityEngine;
using UnityEditor;
using GMTools.Manager;

[CustomEditor(typeof(StoreObjectController), true)]
public class StoreObjectControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StoreObjectController storeObjectController = target as StoreObjectController;
        DrawDefaultInspector();
        EditorGUILayout.Space();
        //To exclude the possibility to create the temp GameObject in the process of run the Play mode
        //EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
        EditorGUI.BeginDisabledGroup(false);
        if (GUILayout.Button("Save data"))
        {
            storeObjectController.SetIStoreObject();
            storeObjectController.Save();
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.Space();
        EditorGUI.BeginDisabledGroup(false);
        if (GUILayout.Button("Load data"))
        {
            storeObjectController.SetIStoreObject();
            storeObjectController.Load();
        }
        EditorGUI.EndDisabledGroup();
    }
}