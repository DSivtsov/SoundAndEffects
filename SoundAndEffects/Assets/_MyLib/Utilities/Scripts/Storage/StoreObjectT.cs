using System;
using UnityEngine;
using UnityEditor;


namespace GMTools.Manager
{
    /// <summary>
    /// Load and Save does to/from the local storage T[] _arrObjects. Use GetLoadedObjects() and SetObjectsToSave() to get/set data from this storage 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ExecuteInEditMode]
    public class StoreObjectT<T> : Store where T : class
    {
        protected T[] _arrObjects =  new T[0];
        [ReadOnly]
        [SerializeField] private string TClass;

        private new void Start()
        {
            base.Start();
            TClass = typeof(T).Name;
        }

        public T[] GetLoadedObjects()
        {
            if (_arrObjects != null)
            {
                //Debug.Log($" T[].Length={_arrObjects.Length}");
                return _arrObjects;
            }
            else
            {
                Debug.LogError($"StoreObject : T[].Length == null");
                return null;
            }
        }

        public void SetObjectsToSave(T[] arrObjects)
        {
            if (arrObjects != null)
            {
                //Debug.Log($"T[].Length={arrObjects.Length}");
                _arrObjects = arrObjects;
            }
            else
                Debug.LogError($"StoreObject : T[].Length == null");
        }

        public override void Load(string[] streamArr)
        {
            if (_isStoreObjectsInitialized && streamArr.Length != 0)
            {
                _arrObjects = new T[streamArr.Length];
                Debug.Log($"StoreObject : QuickLoad() for [{gameObject.name}]");
                for (int i = 0; i < _arrObjects.Length; i++)
                {
                    _arrObjects[i] = JsonUtility.FromJson<T>(streamArr[i]);
                }
            }
            else
                Debug.LogError("StoreObject : Load() {_isStoreObjectsInitialized && streamArr.Length != 0} != true");
        }

        public override string[] Save()
        {
            if (_isStoreObjectsInitialized && _arrObjects != null)
            {
                Debug.Log($"StoreObject : QuickSave for [{gameObject.name}]");
                streamArr = new string[_arrObjects.Length];
                for (int i = 0; i < _arrObjects.Length; i++)
                {
                    streamArr[i] = JsonUtility.ToJson(_arrObjects[i]);
                    //Debug.Log($"{guid} {streamArr[i]:F1}");
                }
                return streamArr;
            }
            else
            {
                Debug.LogError("StoreObject : Save() {isStoreObjectsInitialized && _arrObjects != null} != true");
                return null; 
            }
        }
    }
}
