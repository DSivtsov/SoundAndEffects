using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using GMTools;


namespace GMTools.Manager
{
    /*
     * Prepare Data from GameObject to store by JsonUtility for complex GameObject which include CharacterController and other related child objects
     * Also script create a new unique ID for this object when it's created (added to GameObject)
     * Requiments: The main class which manager a object positionin must implement the interface ILoadData
     * 
     */
    [ExecuteInEditMode]
    public abstract class StoreObjects : MonoBehaviour, IStoreObjects
    {
        [ReadOnly]
        [SerializeField] protected string guid;

        protected bool _isStoreObjectsInitialized = false;
        protected string[] streamArr;

#if UNITY_EDITOR
        /// <summary>
        /// Create a new unique ID for this object when it's created
        /// </summary>
        private void Awake()
        {
            if (!Application.isPlaying && String.IsNullOrEmpty(guid))
            {
                Debug.Log("StoreObjects : Awake() - Generated NewGiud");
                guid = Guid.NewGuid().ToString();
                PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
        }
#endif
        private void Start()
        {
            if (Application.isPlaying)
            {
                InitStoreObjects();
            }
        }

        //Add the StoreObjects to pool objects which will be backuped
        protected virtual void InitStoreObjects()
        {
            Debug.Log($"StoreObjects : Start() - ObjectPool.AddObject({this.name})");
            ObjectPool.AddObject(guid, this);
            //The reserved for more complex procedures to restore and backup values
            _isStoreObjectsInitialized = true;
        }

        public override string ToString()
        {
            return gameObject.name;
        }

        public abstract string[] Save();
        public abstract void Load(string[] streamArr);
    }

}