using System;
using UnityEngine;
using UnityEditor;


namespace GMTools.Manager
{
    public interface ILoadData
    {
        //Used to inform The main class regrading request to restore data and ready fo it
        //In the main class must be selected the right place and additional action before restore values of GameObjects by ILoadData
        public bool IsRequestedLoad { get; set; }
        //Load Data from Json to GameObjects
        public event Action ILoadData ;
    }
    /*
     * Prepare Data from GameObject to store by JsonUtility for complex GameObject which include CharacterController and other related child objects
     * Also script create a new unique ID for this object when it's created (added to GameObject)
     * Requiments: The main class which manager a object positionin must implement the interface ILoadData
     * 
     */
    [ExecuteInEditMode]
    public class StoreObjectsCharacterController : StoreObjectsTransform
    {
        private ILoadData gameObjectLoadData;

        //Add the StoreObjects to pool objects which will be backuped
        //Override to additionally check that main script realized the ILoadData interface and init coresponding Event
        protected override void InitStoreObjects()
        {
            gameObjectLoadData = GetComponent<ILoadData>();
            if (gameObjectLoadData != null)
            {
                Debug.Log($"StoreObjects : Start() - ObjectPool.AddObject({this.name})");
                gameObjectLoadData.ILoadData += LoadDataToGameObject;
                ObjectPool.AddObject(guid, this);
                _isStoreObjectsInitialized = true;
            }
            else
            {
                Debug.LogWarning($"Not initialized the StoreObjects for GameObject({gameObject.name})");
            }
        }

        //In case of the complex object the Main script select the point in the  Update cycle in which will be restored values of GameObject
        public override void Load(string[] streamArr)
        {
            this.streamArr = streamArr;
            if (_isStoreObjectsInitialized)
            {
                //to inform the Main script regarding the request on Restore values of GameObject
                gameObjectLoadData.IsRequestedLoad = true;
                Debug.Log("StoreObjects : QuickLoad(IsRequestedLoad = true)"); 
            }
        }

        //The restored values of GameObject which initiated from Main script
        private void LoadDataToGameObject()
        {
            LoadTransform(streamArr);
            gameObjectLoadData.IsRequestedLoad = false;
        }
    }

}
