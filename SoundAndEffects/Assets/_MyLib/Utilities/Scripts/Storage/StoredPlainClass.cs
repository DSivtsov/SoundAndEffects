using System;
using UnityEngine;
using UnityEditor;


namespace GMTools.Manager
{
    /// <summary>
    /// Load and Save does to/from the local storage T[] _arrObjects. Use GetLoadedObjects() and SetObjectsToSave() to get/set data from this storage 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //[ExecuteInEditMode]
    public class StoredPlainClass<T> : StoredObject where T : class
    {
        protected T[] _arrObjects =  default(T[]);
        [ReadOnly]
        [SerializeField] private string _storedClass = typeof(T).Name;

        public T[] GetLoadedObjects()
        {
            if (_arrObjects != null)
            {
                return _arrObjects;
            }
            else
            {
                Debug.LogError($"StoreObjectT : T[].Length == null");
                return null;
            }
        }

        public void SetObjectsToSave(T[] arrObjects)
        {
            if (arrObjects != null)
            {
                _arrObjects = arrObjects;
            }
            else
                Debug.LogError($"StoreObject : T[].Length == null");
        }

        public override void FromJsonAfterLoad(string[] streamStringArr)
        {
            if (streamStringArr.Length != 0)
            {
                _arrObjects = new T[streamStringArr.Length];
                //Debug.Log($"StoreObjectT : Load() for [{gameObject.name}]");
                for (int i = 0; i < _arrObjects.Length; i++)
                {
                    _arrObjects[i] = JsonUtility.FromJson<T>(streamStringArr[i]);
                }
            }
            else
                Debug.LogError("StoreObjectT : Load() {streamArr.Length == 0");
        }

        public override string[] ToJsonBeforeSave()
        {
            if (_arrObjects != null)
            {
                Debug.Log($"StoreObjectT : Save for [{gameObject.name}]");
                streamStringArr = new string[_arrObjects.Length];
                for (int i = 0; i < _arrObjects.Length; i++)
                {
                    streamStringArr[i] = JsonUtility.ToJson(_arrObjects[i]);
                }
                return streamStringArr;
            }
            else
            {
                Debug.LogError("StoreObjectT : Save() _arrObjects == null");
                return null; 
            }
        }
    }
}
