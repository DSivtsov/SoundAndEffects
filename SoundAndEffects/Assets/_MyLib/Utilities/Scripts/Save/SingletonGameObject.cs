using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTools.Manager;

public class SingletonGameObject : MonoBehaviour
{
    #region SingletonSelfCode
    private static SingletonGameObject _instance;

    public static SingletonGameObject Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    #region SingletonData
    public CharacterData dwarfData;
    public CharacterData robotfData;
    #endregion 

}