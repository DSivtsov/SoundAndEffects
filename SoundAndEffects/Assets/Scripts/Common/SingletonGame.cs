using System;
using UnityEngine;
using UnityEngine.Events;
using GMTools;

/// <summary>
/// Singleton for GameObject Unity
/// Give a possibility to set Values to common variables in Inspector and receive access to it through static property
/// </summary>
public class SingletonGame : SingletonController<SingletonGame>
{
    [SerializeField] private MyCharacterController _characterController;
    [SerializeField] private MovingWorldSO _movingWorldSO;
    [SerializeField] private MainSpawner _mainSpawner;
    [SerializeField] private CharacterData _characterData;
    [SerializeField] private GraveStoneControl _graveStoneControl;
    [SerializeField] private GameSceneManager _gameSceneManager;
    [Space()]
    [Header("For Testing purpose only, used in Editor only")]
    [SerializeField] private bool isWalkingAfterStart;

    //All modules which affected by MoveWorldSpeed must be called
    //public UnityEvent WorldSpeedChanged;

    public MovingWorldSO GetMovingWorld() => _movingWorldSO;
    public MyCharacterController GetCharacterController() => _characterController;
    public MainSpawner GetMainSpawner() => _mainSpawner;
    public CharacterData GetCharacterData() => _characterData;
    public GraveStoneControl GetGraveStoneControl() => _graveStoneControl;
    public GameSceneManager GameSceneManager() => _gameSceneManager;

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
            Debug.LogWarning("Game in Testing mode");
        }
    } 
#endif
}

