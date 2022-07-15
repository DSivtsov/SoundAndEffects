using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//All position set ans check through Rigidbody.position and converting these to localPositions
/// <summary>
/// Object is moving by Rigidbody.MovePosition() at FixedUpdate with set option Interpolate
/// Attached to every type of Spawners
/// </summary>
public class MoveObstacleAndSpawn : MonoBehaviour
{
    const int initCountObstacles = 3;
    [SerializeField] private SpawnerTypeSO spawnerType;
    [SerializeField] private Rigidbody spawnedObstacle;

    private Queue<Rigidbody> arrObstacle = new Queue<Rigidbody>();
    /// <summary>
    /// Store the last spawned Obstacle of this Type
    /// </summary>
    private Rigidbody lastObstacle;
    private Pool poolObstacle;
    protected MovingWorldSO movingWorld;
    protected Vector3 currentVectorMoveWorld;
    private MainSpawner mainSpawner;
    /// <summary>
    /// The obstacle of this Type was spawned the most latest
    /// </summary>
    private bool _IamLastObstacle = false;
    private System.Random random;
    private Vector3 initRigidbodyWorldPosition;
    private float initRigidbodyWorldPositionX;
    private float worldPositionXDistanceAfter;
    protected void Awake()
    {
        movingWorld = SingletonController.Instance.GetMovingWorld();
        random = new System.Random();
        //The pool will Instantiate Objects if it will be demands, base on these parameters
        poolObstacle = new Pool(() => Instantiate<Rigidbody>(spawnedObstacle, transform, worldPositionStays: false));
        //The position and values that was obtained by the obstacle after Instantiantion under Parent with transform
        initRigidbodyWorldPosition = transform.position;
        initRigidbodyWorldPositionX = initRigidbodyWorldPosition.x;
        worldPositionXDistanceAfter = initRigidbodyWorldPositionX - spawnerType.DistanceAfter;
    }
    /// <summary>
    /// Action in case of changing WorldSpeed. Can be override by Direved class
    /// </summary>
    protected virtual void UpdateCurrentVelocityMoveWorld() => currentVectorMoveWorld = movingWorld.VectorSpeed;

    public void SpawnObstacle()
    {
        //Rigidbody newRigidbodyObstacle = Instantiate<Rigidbody>(spawnedObstacle, transform, worldPositionStays: false);
        lastObstacle = poolObstacle.GetElement();
        lastObstacle.gameObject.SetActive(true);
        InitObstacleAndArr();
    }

    private void InitObstacleAndArr()
    {
        float newX = spawnerType.DistanceBeforeMin + (float)((spawnerType.DistanceBeforeMax - spawnerType.DistanceBeforeMin)
            * random.NextDouble() * mainSpawner.Multiplier);
        arrObstacle.Enqueue(lastObstacle);
        //lastObstacle.transform.localPosition = Vector3.right * newX;
        lastObstacle.position = initRigidbodyWorldPosition + Vector3.right * newX;
        lastObstacle.name += $"[{arrObstacle.Count}]";
        lastObstacle.velocity = currentVectorMoveWorld;
        //Debug.Log($"{lastObstacle.name} newX={newX}");
    }

    private bool IsObstaclePassSpawnPosition(Rigidbody rigidbody) => rigidbody.position.x < worldPositionXDistanceAfter;
    private float RigidbodyLocalPositionX(Rigidbody rigidbody) => rigidbody.position.x - initRigidbodyWorldPositionX;
    public bool SetIamLastObstacle(bool value) => _IamLastObstacle = value;

    protected void FixedUpdate()
    {
        if (movingWorld.worldIsMoving && arrObstacle.Count > 0)
        {
            //When the last spawned obstacle went the distance "Distance After" set for this type then will be spawned next obstacle
            //The movement going toward by negative Axe X
            //if (_IamLastObstacle && arrObstacle[arrObstacle.Count - 1].transform.localPosition.x < -spawnerType.DistanceAfter)
            if (_IamLastObstacle && IsObstaclePassSpawnPosition(lastObstacle))
            {
                //Debug.Log($"{lastObstacle.name} call SpawnNextObstacle() x={lastObstacle.transform.localPosition.x:F1} Rx={lastObstacle.position.x:F1}");
                mainSpawner.SpawnNextObstacle();
            }
            //Not del the LastObstacle
            if (movingWorld.IsObjectReadyToRemove(RigidbodyLocalPositionX(arrObstacle.Peek())) && (!_IamLastObstacle || arrObstacle.Count > 1))
            {
                RemoveObstacleFromScreen(); 
            }
        }
    }
    /// <summary>
    /// Update the Obstacle Velocity of the one type
    /// </summary>
    public void UpdateWorldSpeed()
    {
        UpdateCurrentVelocityMoveWorld();
        if (arrObstacle.Count > 0)
        {
            foreach (Rigidbody obstacle in arrObstacle)
            {
                obstacle.velocity = currentVectorMoveWorld;
            }
        }
    }

    //Time Solution vs realization pool object and enable and disable them vs Instatiate and Destroy
    protected void RemoveObstacleFromScreen()
    {
        //Debug.Log($"RemoveObstacleFromScreen() : {arrObstacle.Peek().name}");
        poolObstacle.ReturnElement(arrObstacle.Dequeue());
    }

    public void InitMainSpawner(MainSpawner spawner) => mainSpawner = spawner;
}
