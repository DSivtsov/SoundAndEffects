using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manger for spawners and manage the parameters of spawners which affect the game complixity related to the current level
/// </summary>
public class MainSpawner : MonoBehaviour
{
    [SerializeField] MoveObstacleAndSpawn[] arrMoveObstacles;

    //At Level = 1
    [SerializeField] private int _initialMaxMultiplier = 5;
    //At Level = 4 the Multiplier ~ 1,01
    [SerializeField] private int _levelMinMultiplier = 6;
    [SerializeField] private int _minMultiplier = 1;

    public float Multiplier { get; private set; }

    private System.Random random;
    private int? prevSpawner = null;
    private int countSpawnedAtThisLivel = 0;
    private SingletonController singleton;
    public int Level { get; private set; } = 1;

    void Awake()
    {
        singleton = SingletonController.Instance;
        for (int i = 0; i < arrMoveObstacles.Length; i++)
        {
            arrMoveObstacles[i].InitMainSpawner(this);
        }
        random = new System.Random();
    }

    void Start()
    {
        UpdateMultiplier();
        SpawnNextObstacle();
    }

    public int IncreaseLevel() => ++Level;

    public void SpawnNextObstacle()
    {
        int nextSpawner = random.Next(arrMoveObstacles.Length);
        if (prevSpawner.HasValue)
        {
            //Skip this for first run
            arrMoveObstacles[prevSpawner.Value].SetIamLastObstacle(false);
        }
        prevSpawner = nextSpawner;

        CheckAndChangeLevel();

        arrMoveObstacles[nextSpawner].SpawnObstacle();
        countSpawnedAtThisLivel++;
        arrMoveObstacles[nextSpawner].SetIamLastObstacle(true);
    }

    /// <summary>
    /// Every ten object Game the raise the Complexity of Level
    /// </summary>
    private void CheckAndChangeLevel()
    {
        if (countSpawnedAtThisLivel > 9)
        {
            Debug.Log($"New Level = {IncreaseLevel()}");
            UpdateMultiplier();
            countSpawnedAtThisLivel = 0;
        }
    }

    /// <summary>
    /// Set the parameter the Level of complexity Multiplier based on the current Game Level and Game Settings
    /// </summary>
    private void UpdateMultiplier()
    {
        //Get 0.99 at singleton.Level = LevelMinMultiplier and 0 at singleton.Level = 1
        float procentLerp = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / (_levelMinMultiplier-1) * (Level - 1));
        Multiplier = Mathf.Lerp(_initialMaxMultiplier, _minMultiplier, procentLerp);
        Debug.Log($"UpdateMultiplier : Level={Level} procentLerp={procentLerp:F2} Multiplier={Multiplier:F2}");
    }

    /// <summary>
    /// Called through UnityEvent InformAboutSpeedChange
    /// </summary>
    public void UpdatedWorldSpeedForObstacles()
    {
        for (int i = 0; i < arrMoveObstacles.Length; i++)
        {
            arrMoveObstacles[i].UpdateWorldSpeed();
        }
    }
}
