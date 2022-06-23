//#define TRACEON
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Object is moving by Rigidbody.MovePosition() at FixedUpdate with set option Interpolate
/// Attached to every type of Spawners
/// </summary>
public class MoveObstacleAndSpawn : MonoBehaviour
{
    const int initCountObstacles = 3;
    //[Tooltip("Set for this type of obstacle the relative speed in relation to the ground speed of World. Must be higher then 1 if obstacle are moving")]
    //[Range(1,3f)]
    //public float speedModifier = 1f;
    [SerializeField] private SpawnerTypeSO spawnerType;
    [SerializeField] private Rigidbody spawnedObstacle;

    private List<Rigidbody> arrObstacle = new List<Rigidbody>(initCountObstacles);
    //private List<Obstacle> arrObstacle = new List<Obstacle>(initCountObstacles);
    protected MovingWorldSO movingWorld;
    protected Vector3 currentVectorMoveWorld;
    private MainSpawner mainSpawner;
    /// <summary>
    /// Is this obstacle was spawned the most latest
    /// </summary>
    private bool _IamLastObstacle = false;
    private int idxLastObstacleThatType = -1;
    private System.Random random;
    protected void Awake()
    {
        movingWorld = SingletonController.Instance.GetMovingWorld();
        random = new System.Random();
    }

    protected virtual void UpdateCurrentVelocityMoveWorld() => currentVectorMoveWorld = movingWorld.VectorSpeed;

    public void SpawnObstacle()
    {
        float newX = spawnerType.DistanceBeforeMin + (float)((spawnerType.DistanceBeforeMax - spawnerType.DistanceBeforeMin) * random.NextDouble() * mainSpawner.Multiplier);
        Rigidbody newRigidbodyObstacle = Instantiate<Rigidbody>(spawnedObstacle, transform, worldPositionStays: false);
        //Obstacle newObstacle = new Obstacle(newRigidbodyObstacle, newRigidbodyObstacle.transform);
        arrObstacle.Add(newRigidbodyObstacle);
        idxLastObstacleThatType++;
        newRigidbodyObstacle.position += Vector3.right * newX;
        newRigidbodyObstacle.name += $"[{idxLastObstacleThatType}]";
        //newRigidbodyObstacle.velocity = Vector3.right * movingWorld.CurrentSpeed * speedModifier;
        newRigidbodyObstacle.velocity = currentVectorMoveWorld;
    }

    public bool SetIamLastObstacle(bool value) => _IamLastObstacle = value;

    protected void FixedUpdate()
    {
        if (movingWorld.worldIsMoving)
        {
            if (idxLastObstacleThatType >= 0)
            {
                for (int i = 0; i < arrObstacle.Count; i++)
                {
                    if (movingWorld.IsObjectReadyToRemove(arrObstacle[i].transform.localPosition.x))
                    {
                        RemoveObstacleFromScreen(i);
                    }
                }

                //When the last spawned obstacle went the distance "Distance After" set for this type then will be spawned next obstacle
                if (_IamLastObstacle && arrObstacle[idxLastObstacleThatType].transform.localPosition.x < -spawnerType.DistanceAfter)
                {
                    mainSpawner.SpawnNextObstacle();
                }
#if TRACEON
                else
                {
                    if (_IamLastObstacle) Debug.Log($"{name}({idxLastObstacleThatType + 1}) x={arrXDistanceObstacles[idxLastObstacleThatType]:F1}");
                }  
#endif
            }
        }
    }
    /// <summary>
    /// Update the Obstacle Velocity of the one type
    /// </summary>
    public void UpdateWorldSpeed()
    {
        UpdateCurrentVelocityMoveWorld();
        if (idxLastObstacleThatType >= 0)
        {
            //Suggestion the all Obstacles of one type have the same speed
            //Vector3 actualVelocityForThisType = Vector3.right * movingWorld.CurrentSpeed * speedModifier;
            for (int i = 0; i < arrObstacle.Count; i++)
            {
                arrObstacle[i].velocity = currentVectorMoveWorld;
            }
        }
    }

    //Time Solution vs realization pool object and enable and disable them vs Instatiate and Destroy
    protected void RemoveObstacleFromScreen(int idxObstacle)
    {
        Destroy(arrObstacle[idxObstacle].gameObject);
        arrObstacle.RemoveAt(idxObstacle);
        idxLastObstacleThatType--;
    }

    public void InitMainSpawner(MainSpawner spawner) => mainSpawner = spawner;
}
