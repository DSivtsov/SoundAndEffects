using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using GMTools;


namespace GMTools.Manager
{
    public interface IStoredObject
    {
        public string[] ToJsonBeforeSave();
        public void FromJsonAfterLoad(string[] streamArr);
    }
    /*
     * Prepare Data from GameObject to store by JsonUtility for complex GameObject which include CharacterController and other related child objects
     * Also script create a new unique ID for this object when it's created (added to GameObject)
     * Requiments: The main class which manager a object positionin must implement the interface ILoadData
     * 
     */
    [ExecuteInEditMode]
    public abstract class StoredObject : MonoBehaviour, IStoredObject
    {
        private StoreObjectController _storeObjectController;

        public StoreObjectController GetStoreObjectController() => _storeObjectController;
        protected string[] streamStringArr;

        protected void Awake()
        {
            if (Application.isPlaying)
            {
                InitStoreObjects();
            }
        }

        protected virtual void InitStoreObjects()
        {
            _storeObjectController = GetComponent<StoreObjectController>();
            Debug.Log($"Store : Start() - SetStoreObject({this.name}) for {_storeObjectController.name}");
            _storeObjectController.SetIStoreObject(this);
        }

        public override string ToString()
        {
            return gameObject.name;
        }

        public abstract string[] ToJsonBeforeSave();
        public abstract void FromJsonAfterLoad(string[] streamStringArr);
    }

}
