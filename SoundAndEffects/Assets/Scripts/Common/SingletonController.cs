using System;
using UnityEngine;
using UnityEngine.Events;
using GMTools;

/// <summary>
/// Singleton for GameObject Unity
/// Give a possibility to set Values to common variables in Inspector and receive access to it through static property
/// </summary>
public class SingletonController : SingletonController<SingletonController>
{
    [SerializeField] private MyCharacterController characterController;
    [SerializeField] private MovingWorldSO movingWorldSO;
    //All modules which affected by MoveWorldSpeed must be called
    public UnityEvent InformAboutSpeedChange;

    public MovingWorldSO GetMovingWorld() => movingWorldSO;

    public MyCharacterController GetCharacterController() => characterController;

    [Space()]
    [Header("For Demo purpose only, used only in Editor")]
    [SerializeField] private bool isWalkingAfterStart;
    public bool IsWalkingAfterStart { get => isWalkingAfterStart; }
    [SerializeField] private bool isTurnOffAllObstacle;
    public bool IsTurnOffAllObstacle { get => isTurnOffAllObstacle; }
    [SerializeField] private bool isPlayerNotCollide;
    public bool IsPlayerNotCollide { get => isPlayerNotCollide; }

#if UNITY_EDITOR
    private void Start()
    {
        if (isWalkingAfterStart || isPlayerNotCollide || isTurnOffAllObstacle)
        {
            Debug.LogWarning("Game in Demo mode");
        }
    } 
#endif
}

