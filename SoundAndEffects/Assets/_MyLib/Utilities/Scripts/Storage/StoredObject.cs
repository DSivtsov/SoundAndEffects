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
    public abstract class StoredObject : MonoBehaviour, IStoredObject
    {
        protected string[] streamStringArr;

        public override string ToString() => gameObject.name;

        public abstract string[] ToJsonBeforeSave();
        public abstract void FromJsonAfterLoad(string[] streamStringArr);
    }

}
