using System;
using UnityEngine;
using UnityEngine.Events;
using GMTools;

/// <summary>
/// Singleton for Game Scene
/// Contains the links to interconnect GameObjects
/// </summary>
public class SingletonGame : SingletonController<SingletonGame>
{
    [SerializeField] private CharacterManager _characterManager;
    [SerializeField] private MovingWorldSO _movingWorldSO;
    [SerializeField] private CharacterDataController _characterDataCtrl;
    [SerializeField] private GameSceneManager _gameSceneManager;
    [SerializeField] private GameParametersManager _gameParametersManager;
    [SerializeField] private CharacterCollisionGround _playerCollisionGround;

    public MovingWorldSO GetMovingWorld() => _movingWorldSO;
    public CharacterManager GetCharacterManager() => _characterManager;
    public CharacterDataController GetCharacterDataCtrl() => _characterDataCtrl;
    public GameSceneManager GetGameSceneManager() => _gameSceneManager;
    public GameParametersManager GetGameParametersManager() => _gameParametersManager;
    public CharacterCollisionGround GetPlayerCollisionGround() => _playerCollisionGround;
}

