using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Manager;
using OdinSerializer;
using System.IO;

public class StoreGameControlSO : MonoBehaviour
{
    public NewGameSettingsSO settingsSO;
    public StoreGame storeGame;
    [SerializeField] private List<Object> unityReferences = new List<Object>();

    #region StoreByUnityJSONSerilization
    public void Save()
    {
        Debug.Log($"{this} : Save() : {settingsSO}");
        Debug.Log($"{this} : Save() : {settingsSO.LevelPlayer}");
        settingsSO.BeforeSerialize();
        storeGame.QuickSave();
    }

    public void Load()
    {
        IOError iOError = storeGame.QuickLoad();
        if (iOError != IOError.NoError)
        {
            Debug.LogError($"Error : {iOError}");
        }
        settingsSO.AfterSerialize();
        Debug.Log($"{this} : Load() : {settingsSO}");
        Debug.Log($"{this} : Load() : {settingsSO.LevelPlayer}");
    }
    #endregion

    #region StoreByOdinJSONSerilization
    public void SaveOdin()
    {
        Debug.Log($"{this} : Save() : {settingsSO}");
        Debug.Log($"{this} : Save() : {settingsSO.LevelPlayer}");
        settingsSO.BeforeSerialize();
        Debug.Log("OdinCallsExample.SaveUnityObject");
        OdinCallsExample.SaveUnityObjectPlainClass(settingsSO, "NewODIN.txt", ref unityReferences);
    }

    public void LoadOdin()
    {
        Debug.Log("OdinCallsExample.LoadUnityObject");
        OdinCallsExample.LoadUnityObjectPlainClass(settingsSO, "NewODIN.txt", ref unityReferences);
        settingsSO.AfterSerialize();
        Debug.Log($"{this} : Load() : {settingsSO}");
        Debug.Log($"{this} : Load() : {settingsSO.LevelPlayer}");
    }
    #endregion

    #region CommonTests
    public void Change()
    {
        Debug.Log($"{this} : Change Before() : {settingsSO}");
        Debug.Log($"{this} : Change Before() : {settingsSO.LevelPlayer}");
        settingsSO.ChangeComplexitySO();
        Debug.Log($"{this} : Change After() : {settingsSO}");
        Debug.Log($"{this} : Change After() : {settingsSO.LevelPlayer}");
    }

    public void Test()
    {
        Debug.Log($"{this} : Test() : {settingsSO}");
        Debug.Log($"{this} : Test() : {settingsSO.LevelPlayer}");
    } 
    #endregion
}
