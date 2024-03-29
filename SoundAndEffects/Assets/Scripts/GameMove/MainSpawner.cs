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
    //[Header("Parameters Game Complexity rising")]
    ////At Level = 1
    //[Tooltip("The maximum spacing Obstacles at Game start")]
    //[SerializeField] private int _maxMultiplier = 5;
    //[Tooltip("The minimum spacing Obstacles at Selected Level and nexts")]
    //[SerializeField] private int _minMultiplier = 1;
    //[Tooltip("The Selected Level with the minimal spacing Obstacles. At previous Level the value = min + (max - min) * 0,01")]
    //[SerializeField] private int _levelMinMultiplier = 6;


    private System.Random random;
    private int? idxPrevSpawner;// = null;
    //private int countSpawnedAtThisLivel = 0;
    private MovingWorldSO movingWorldSO;
    private GameParametersManager _gameParametersManager;

    public int Level { get; private set; } = 1;
    //public float Multiplier { get; private set; }
    //public event Action<int> LevelChanged;

    void Awake()
    {
        movingWorldSO = SingletonGame.Instance.GetMovingWorld();
        for (int i = 0; i < arrSpawners.Length; i++)
        {
            arrSpawners[i].InitMainSpawner(this);
        }
        random = new System.Random();
    }

    private void OnEnable() => movingWorldSO.WorldSpeedChanged += UpdatedWorldSpeedForObstacles;
    private void OnDisable() => movingWorldSO.WorldSpeedChanged -= UpdatedWorldSpeedForObstacles;

    private void Start()
    {
        _gameParametersManager = SingletonGame.Instance.GetGameParametersManager();
        ReStartSpawner();
    }
    public void ReStartSpawner()
    {
        idxPrevSpawner = null;
        //Temprorary turn off spawing all obstacle For Testing purpose in Editor only
#if UNITY_EDITOR
        if (!SingletonGame.Instance.IsTurnOffAllObstacle)
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



    ///// <summary>
    ///// Set the Multiplier parameter - the Level of complexity based on the current Game Level and Game Settings
    ///// </summary>
    //private void UpdateLevelComplexity()
    //{
    //    //Get 0.99 at (LevelMinMultiplier-1) Level and ~1 at Level = LevelMinMultiplier
    //    float procentLerp = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / (_levelMinMultiplier-1) * (Level - 1));
    //    Multiplier = Mathf.Lerp(_maxMultiplier, _minMultiplier, procentLerp);
    //    ////Debug.Log($"UpdateMultiplier : Level={Level} procentLerp={procentLerp:F2} Multiplier={Multiplier:F2}");
    //}

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
