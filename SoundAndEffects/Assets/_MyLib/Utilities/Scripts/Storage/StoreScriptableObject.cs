using System;
using UnityEngine;
using UnityEditor;


namespace GMTools.Manager
{
    public class StoreScriptableObject : Store
    {
        [SerializeField] protected ScriptableObject[] storeScriptableObjectData;

        public override void Load(string[] streamArr)
        {
            if (_isStoreObjectsInitialized)
            {
                Debug.Log($"StoreMonoBehaviour : QuickLoad for [{gameObject.name}]");
                for (int i = 0; i < storeScriptableObjectData.Length; i++)
                {
                    JsonUtility.FromJsonOverwrite(streamArr[i], storeScriptableObjectData[i]);
                }
            }
        }

        public override string[] Save()
        {
            if (_isStoreObjectsInitialized)
            {
                Debug.Log($"StoreMonoBehaviour : QuickSave for [{gameObject.name}]");
                streamArr = new string[storeScriptableObjectData.Length];
                for (int i = 0; i < storeScriptableObjectData.Length; i++)
                {
                    streamArr[i] = JsonUtility.ToJson(storeScriptableObjectData[i]);
                    //Debug.Log($"ObjectGuid[{guid}] [{streamArr[i]:F1]}");
                }
                return streamArr;
            }
            else
                return null;
        }

    }

}
