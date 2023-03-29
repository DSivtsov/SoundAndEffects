using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manger for spawners and manage the parameters of spawners which affect the game complixity related to the current level
/// </summary>
public class MainSpawner : MonoBehaviour
{
    [SerializeField] MoveObstacleAndSpawn[] arrSpawners;

    private System.Random random;
    private int? idxPrevSpawner;
    private MovingWorldSO _movingWorldSO;
    private GameParametersManager _gameParametersManager;

    public int Level { get; private set; } = 1;

    void Awake()
    {
        _movingWorldSO = SingletonGame.Instance.GetMovingWorld();
        _gameParametersManager = SingletonGame.Instance.GetGameParametersManager();
        for (int i = 0; i < arrSpawners.Length; i++)
        {
            arrSpawners[i].InitMainSpawner(this);
        }
        random = new System.Random();
    }

    private void OnEnable() => _movingWorldSO.WorldSpeedChanged += UpdatedWorldSpeedForObstacles;
    private void OnDisable() => _movingWorldSO.WorldSpeedChanged -= UpdatedWorldSpeedForObstacles;

    //private void Start()
    //{
    //    ReStartSpawner();
    //}
    public void ReStartSpawner()
    {
        idxPrevSpawner = null;
        //Temprorary turn off spawing all obstacle For Testing purpose in Editor only
#if UNITY_EDITOR
        if (!_gameParametersManager.IsTurnOffAllObstacle)
        {
            SpawnNextObstacle(false);
        }
#else
        SpawnNextObstacle(false);
#endif
    }

    public void SpawnNextObstacle(bool notFirst = true, float distanceAfter = 0)
    {
        int idxCurrentSpawner = random.Next(arrSpawners.Length);
        if (idxPrevSpawner.HasValue)
        {
            //Skip this for first run
            arrSpawners[idxPrevSpawner.Value].SetIamLastSpawner(false);
        }
        idxPrevSpawner = idxCurrentSpawner;
        _gameParametersManager.CheckAndUpdateLevelGame();
        arrSpawners[idxCurrentSpawner].SpawnObstacle(notFirst, distanceAfter);
        _gameParametersManager.AddNewSpawnedObstacle();
        arrSpawners[idxCurrentSpawner].SetIamLastSpawner(true);
    }

    public void UpdatedWorldSpeedForObstacles()
    {
        for (int i = 0; i < arrSpawners.Length; i++)
        {
            arrSpawners[i].UpdateWorldSpeed();
        }
    }

    public void RemoveAllObstacles()
    {
        for (int i = 0; i < arrSpawners.Length; i++)
        {
            arrSpawners[i].SetIamLastSpawner(false);
            arrSpawners[i].RemoveAllObstacleFromScreen();
        }
    }
}
