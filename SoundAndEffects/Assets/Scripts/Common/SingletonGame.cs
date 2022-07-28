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
    [SerializeField] private CharacterManager _characterController;
    [SerializeField] private MovingWorldSO _movingWorldSO;
    [SerializeField] private MainSpawner _mainSpawner;
    [SerializeField] private CharacterDataController _characterDataCtrl;
    [SerializeField] private GraveStoneControl _graveStoneControl;
    [SerializeField] private GameSceneManager _gameSceneManager;
    [SerializeField] private GameParametersManager _gameParametersManager;
    [SerializeField] private PlayJukeBoxCollection _playJukeBoxCollection;
    [SerializeField] private PlayerCollisionGround _playerCollisionGround;
    [Space()]
    [Header("For Testing purpose only, used in Editor only")]
    [SerializeField] private bool isWalkingAfterStart;
    [SerializeField] private bool isTurnOffAllObstacle;
    [SerializeField] private bool isPlayerNotCollide;

    public MovingWorldSO GetMovingWorld() => _movingWorldSO;
    public CharacterManager GetCharacterController() => _characterController;
    public MainSpawner GetMainSpawner() => _mainSpawner;
    public CharacterDataController GetCharacterDataCtrl() => _characterDataCtrl;
    public GraveStoneControl GetGraveStoneControl() => _graveStoneControl;
    public GameSceneManager GetGameSceneManager() => _gameSceneManager;
    public GameParametersManager GetGameParametersManager() => _gameParametersManager;
    public PlayJukeBoxCollection GetPlayJukeBox() => _playJukeBoxCollection;
    public PlayerCollisionGround GetPlayerCollisionGround() => _playerCollisionGround;

    public bool IsWalkingAfterStart { get => isWalkingAfterStart; }

    public bool IsTurnOffAllObstacle { get => isTurnOffAllObstacle; }

    public bool IsPlayerNotCollide { get => isPlayerNotCollide; }

    private void Start()
    {
        //Turn off all Testing modes in Build
#if UNITY_EDITOR
        if (isWalkingAfterStart || isPlayerNotCollide || isTurnOffAllObstacle)
        {
            Debug.LogWarning("Game in Testing mode");
        }
#else
        isWalkingAfterStart = false;
        isTurnOffAllObstacle = false;
        isPlayerNotCollide = false;
#endif
    }
}

