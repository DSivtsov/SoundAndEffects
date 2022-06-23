using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Singleton for GameObject Unity
/// Give a possibility to set Values to common variables in Inspector and receive access to it through static property
/// </summary>
public class SingletonController : MonoBehaviour
{
    #region SingletonInternalPart
    private static SingletonController _instance;
    /// <summary>
    /// Static property to access the Singleton _instance
    /// </summary>
    public static SingletonController Instance
    {
        get
        {
            if (_instance == null)
            {
                //If it the first time of access to the property and it occurred before Awake() of this class
                //If raise the priority of execution of this script related to other, this check will not demanded
                //But it will not work in Editor mode at Load because FindObjectOfType can found only active objects, which will be after Enter in Play mode
                _instance = FindObjectOfType<SingletonController>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            //If was created many objects of this type, the attempt to execute the Awake() for second and others object of this type will issue the error message
            //It's not possible to estimate which object (of one type) will be executed first, therefore the simple Delete(GameObject) is not correct for this type
            //  of the Singleton, and how it used
            Debug.LogError($"Find dubplicate of SingletonController [{this.name}]");
        }
        else
        {
            //Initialize the static variable by reference to current object
            //It will occur only for first object of this type
            _instance = this;
        }
    }
    #endregion

    [SerializeField] private MyCharacterController characterController;
    [SerializeField] private MovingWorldSO movingWorldSO;
    //All modules which affected by MoveWorldSpeed must be called
    public UnityEvent InformAboutSpeedChange;

    public MovingWorldSO GetMovingWorld() => movingWorldSO;

    public MyCharacterController GetCharacterController() => characterController;
}

