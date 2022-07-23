using System;
using UnityEngine;
using UnityEditor;
using System.IO;


namespace GMTools.Manager
{
    /*
     * Prepare Data from GameObject to store by JsonUtility for complex GameObject which include CharacterController and other related child objects
     * Also script create a new unique ID for this object when it's created (added to GameObject)
     * Requiments: The main class which manager a object positionin must implement the interface ILoadData
     * 
     */
    [ExecuteInEditMode]
    public class StoreObjectsTransform : StoreObjects
    {
        [SerializeField] protected Transform[] storeObjects;

        //In case of the simple objects the script initiated time of restore values of GameObject
        public override void Load(string[] streamArr)
        {
            //this.streamArr = streamArr;
            if (_isStoreObjectsInitialized)
            {
                LoadTransform(streamArr);
            }
        }

        protected void LoadTransform(string[] streamArr)
        {
            Debug.Log($"StoreObjects : GameObjectLoadData({gameObject.name})");
            for (int i = 0; i < storeObjects.Length; i++)
            {
                storeObjects[i].position = JsonUtility.FromJson<Vector3>(streamArr[i * 2]);
                storeObjects[i].rotation = JsonUtility.FromJson<Quaternion>(streamArr[i * 2 + 1]);
                Debug.Log($"({storeObjects[i].name}){storeObjects[i].position:F1}");
                Debug.Log($"({storeObjects[i].name}){storeObjects[i].rotation:F1}");
            }
        }

        public override string[] Save()
        {
            if (_isStoreObjectsInitialized)
            {
                Debug.Log($"StoreObjects : QuickSave({gameObject.name})");
                streamArr = new string[storeObjects.Length * 2];
                for (int i = 0; i < storeObjects.Length; i++)
                {
                    streamArr[i * 2] = JsonUtility.ToJson(storeObjects[i].position);
                    streamArr[i * 2 + 1] = JsonUtility.ToJson(storeObjects[i].rotation);
                    Debug.Log($"{guid} {streamArr[i]:F1}");
                    Debug.Log($"{guid} {streamArr[i + 1]:F1}");
                }
                return streamArr;
            }
            else
                return null;
        }
    }

}
