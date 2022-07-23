using System;
using UnityEngine;
using UnityEditor;


namespace GMTools.Manager
{
    [ExecuteInEditMode]
    public class StoreObjectsClass : StoreObjects
    {
        [SerializeField] protected CharacterData[] storeCharacterData;

        public override void Load(string[] streamArr)
        {
            if (_isStoreObjectsInitialized)
            {
                Debug.Log($"StoreObjects : GameObjectLoadData({gameObject.name})");
                for (int i = 0; i < storeCharacterData.Length; i++)
                {
                    JsonUtility.FromJsonOverwrite(streamArr[i], storeCharacterData[i]);
                    Debug.Log($"({storeCharacterData[i].name}){storeCharacterData[i]}");
                }
            }
        }

        public override string[] Save()
        {
            if (_isStoreObjectsInitialized)
            {
                Debug.Log($"StoreObjects : QuickSave({gameObject.name})");
                streamArr = new string[storeCharacterData.Length];
                for (int i = 0; i < storeCharacterData.Length; i++)
                {
                    streamArr[i] = JsonUtility.ToJson(storeCharacterData[i]);
                    Debug.Log($"{guid} {streamArr[i]:F1}");
                }
                return streamArr;
            }
            else
                return null;
        }

    }

}
