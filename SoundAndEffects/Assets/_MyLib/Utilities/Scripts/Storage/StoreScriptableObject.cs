using System;
using UnityEngine;
using UnityEditor;


namespace GMTools.Manager
{
    public class StoreScriptableObject : StoredObject
    {
        [SerializeField] protected ScriptableObject[] storeScriptableObjectData;

        public override void FromJsonAfterLoad(string[] streamArr)
        {
            if (storeScriptableObjectData != null)
            {
                Debug.Log($"StoreMonoBehaviour : QuickLoad for [{gameObject.name}]");
                for (int i = 0; i < storeScriptableObjectData.Length; i++)
                {
                    JsonUtility.FromJsonOverwrite(streamArr[i], storeScriptableObjectData[i]);
                }
            }
            else
            {
                Debug.LogError("StoreScriptableObject : Load() storeScriptableObjectData == null");
            }
        }

        public override string[] ToJsonBeforeSave()
        {
            if (storeScriptableObjectData != null)
            {
                Debug.Log($"StoreMonoBehaviour : QuickSave for [{gameObject.name}]");
                streamStringArr = new string[storeScriptableObjectData.Length];
                for (int i = 0; i < storeScriptableObjectData.Length; i++)
                {
                    streamStringArr[i] = JsonUtility.ToJson(storeScriptableObjectData[i]);
                    //Debug.Log($"ObjectGuid[{guid}] [{streamArr[i]:F1]}");
                }
                return streamStringArr;
            }
            else
            {
                Debug.LogError("StoreScriptableObject : Save() storeScriptableObjectData == null");
                return null; 
            }
        }

    }

}
