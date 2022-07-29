using System;
using UnityEngine;
using UnityEditor;


namespace GMTools.Manager
{
    [ExecuteInEditMode]
    public class StoreMonoBehaviour : Store
    {
        [SerializeField] protected MonoBehaviour[] storeMonoBehaviouData;

        public override void Load(string[] streamArr)
        {
            if (_isStoreObjectsInitialized)
            {
                Debug.Log($"StoreMonoBehaviour : QuickLoad for [{gameObject.name}]");
                for (int i = 0; i < storeMonoBehaviouData.Length; i++)
                {
                    JsonUtility.FromJsonOverwrite(streamArr[i], storeMonoBehaviouData[i]);
                    //Debug.Log($"({storeCharacterData[i].name}){storeCharacterData[i]}");
                }
            }
        }

        public override string[] Save()
        {
            if (_isStoreObjectsInitialized)
            {
                Debug.Log($"StoreMonoBehaviour : QuickSave for [{gameObject.name}]");
                streamArr = new string[storeMonoBehaviouData.Length];
                for (int i = 0; i < storeMonoBehaviouData.Length; i++)
                {
                    streamArr[i] = JsonUtility.ToJson(storeMonoBehaviouData[i]);
                    Debug.Log($"{guid} {streamArr[i]:F1}");
                }
                return streamArr;
            }
            else
                return null;
        }

    }

}
