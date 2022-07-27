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
    private readonly struct SpawnedObstacleWScore
    {
        public readonly Rigidbody RigidbodyObstacle;
        public readonly int ScoreObstacle;

        public SpawnedObstacleWScore(Rigidbody RigidbodyObstacle, int ScoreObstacle)
        {
            this.RigidbodyObstacle = RigidbodyObstacle;
            this.ScoreObstacle = ScoreObstacle;
        }
    }

    private const int initCountObstaclesInQueues = 10;
    [SerializeField] private SpawnerTypeSO spawnerType;
    [SerializeField] private Rigidbody spawnedObstacle;

    //private Queue<Rigidbody> arrObstacle = new Queue<Rigidbody>();
    private Queue<SpawnedObstacleWScore> arrSpawnedObstacleWScore = new Queue<SpawnedObstacleWScore>(initCountObstaclesInQueues);
    private Queue<Rigidbody> arrObstaclesRemoveQueue = new Queue<Rigidbody>(initCountObstaclesInQueues);
    /// <summary>
    /// Store the last spawned Obstacle of this Type
    /// </summary>
    private Rigidbody lastSpawnedObstacle;
    private Pool poolObstacle;
    protected MovingWorldSO movingWorld;
    protected Vector3 currentVectorMoveWorld;
    private MainSpawner mainSpawner;
    
    /// <summary>
    /// The obstacle of this Type was spawned the most latest
    /// </summary>
    private bool _IamLastSpawner = false;
    private System.Random random;
    private Vector3 initRigidbodyWorldPosition;
    private float initRigidbodyWorldPositionX;
    private float worldPositionXDistanceAfter;
    private GameParametersManager gameParametersManager;
    /// <summary>
    /// the local position of the character, relative to the spawned position of these obstacles
    /// </summary>
    private float _characterInitLocPosX;
    private CharacterData characterData;
    protected void Awake()
    {
        movingWorld = SingletonGame.Instance.GetMovingWorld();
        characterData = SingletonGame.Instance.GetCharacterData();
        random = new System.Random();
        //The pool will Instantiate Objects if it will be demands, base on these parameters
        poolObstacle = new Pool(() => Instantiate<Rigidbody>(spawnedObstacle, transform, worldPositionStays: false));
        //The position and values that was obtained by the obstacle after Instantiantion under Parent with transform
        initRigidbodyWorldPosition = transform.position;
        initRigidbodyWorldPositionX = initRigidbodyWorldPosition.x;
        worldPositionXDistanceAfter = initRigidbodyWorldPositionX - spawnerType.DistanceAfter;
        _characterInitLocPosX = SingletonGame.Instance.GetCharacterController().GetCharacterInitWordPosX() - initRigidbodyWorldPositionX;
    }

    private void Start()
    {
        gameParametersManager = SingletonGame.Instance.GetGameParametersManager();
    }

    /// <summary>
    /// Action in case of changing WorldSpeed. Can be override by Direved class
    /// </summary>
    protected virtual void UpdateCurrentVelocityMoveWorld() => currentVectorMoveWorld = movingWorld.VectorSpeed;

    public void SpawnObstacle()
    {
        lastSpawnedObstacle = poolObstacle.GetElement();
        lastSpawnedObstacle.gameObject.SetActive(true);
        InitObstacleAndArr();
    }

    private void InitObstacleAndArr()
    {
        float newX = (   spawnerType.DistanceBeforeMin + (float)( (spawnerType.DistanceBeforeMax - spawnerType.DistanceBeforeMin) * random.NextDouble() )    )
            * gameParametersManager.Multiplier;
        arrSpawnedObstacleWScore.Enqueue(new SpawnedObstacleWScore(lastSpawnedObstacle, spawnerType.BaseScore * gameParametersManager.Level)); 
        lastSpawnedObstacle.position = initRigidbodyWorldPosition + Vector3.right * newX;
        lastSpawnedObstacle.name += $"[{arrSpawnedObstacleWScore.Count}]";
        lastSpawnedObstacle.velocity = currentVectorMoveWorld;
    }

    //The all movement going toward by negative Axe X therefore the all values of positions is negative
    private bool IsLastObstaclePassSpawnPosition(Rigidbody rigidbody) => rigidbody.position.x < worldPositionXDistanceAfter;
    private bool IsAnObstaclePassInitCharacterPosition(float rigidbodyLocalPositionX) => rigidbodyLocalPositionX < _characterInitLocPosX;
    private float GetRigidbodyLocalPositionX(Rigidbody rigidbody) => rigidbody.position.x - initRigidbodyWorldPositionX;
    public bool SetIamLastSpawner(bool value) => _IamLastSpawner = value;
    int count;
    protected void FixedUpdate()
    {
        if (movingWorld.worldIsMoving && arrSpawnedObstacleWScore.Count > 0)
        {
            //When the last spawned obstacle went the distance "Distance After" set for this type then will be spawned next obstacle
            //will call only for lastObstacle and only if current spawner is "_IamLastSpawner"
            if (_IamLastSpawner && IsLastObstaclePassSpawnPosition(lastSpawnedObstacle))
            {
                //Debug.Log($"{lastObstacle.name} call SpawnNextObstacle() x={lastObstacle.transform.localPosition.x:F1} Rx={lastObstacle.position.x:F1}");
                mainSpawner.SpawnNextObstacle();
            }
            SpawnedObstacleWScore firstObstacleInQueue = arrSpawnedObstacleWScore.Peek();
            Rigidbody rigidbodyFirstObstacle = firstObstacleInQueue.RigidbodyObstacle;
            float firstObstacleLocalPositionX = GetRigidbodyLocalPositionX(rigidbodyFirstObstacle);
            //Not put to Remove Queue the LastObstacle
            //If the spawned "spacing parameters will very big", possible a case when a Obstacle will be placed in arrObstaclesRemoveQueue very far
            // from InitCharacterPosition. In this case the counting of that Obstacle will delayed to this moment for this code:
            // if (IsAnObstaclePassInitCharacterPosition(firstObstacleLocalPositionX) && (!_IamLastSpawner || arrSpawnedObstacleWScore.Count > 1))
            //This code does not have the previously mentioned specifics "delayed counting", and it must work because used the lastSpawnedObstacle
            // object not stored in arrSpawnedObstacleWScore
            if (IsAnObstaclePassInitCharacterPosition(firstObstacleLocalPositionX))
            {
                ScoreCounting.AddObstacleToStack(rigidbodyFirstObstacle.GetInstanceID(), firstObstacleInQueue.ScoreObstacle);
                //CountFrame.DebugLogFixedUpdate($"AddObstacleToStack([{rigidbodyFirstObstacle.name}] [{firstObstacleInQueue.ScoreObstacle}]" +
                //    $" Pos={firstObstacleLocalPositionX:F2})");
                arrObstaclesRemoveQueue.Enqueue(rigidbodyFirstObstacle);
                arrSpawnedObstacleWScore.Dequeue();
            }
            if (arrObstaclesRemoveQueue.Count > 0)
            {
                float localPosXFirstRigidbodyInRemoveQueue = GetRigidbodyLocalPositionX(arrObstaclesRemoveQueue.Peek());
                if (movingWorld.IsObjectReadyToRemove(localPosXFirstRigidbodyInRemoveQueue))
                {
                    RemoveObstacleFromScreen();
                } 
            }
        }
    }
    /// <summary>
    /// Update the Obstacle Velocity of the one type
    /// </summary>
    public void UpdateWorldSpeed()
    {
        UpdateCurrentVelocityMoveWorld();
        if (arrSpawnedObstacleWScore.Count > 0)
        {
            foreach (SpawnedObstacleWScore obstacle in arrSpawnedObstacleWScore)
            {
                obstacle.RigidbodyObstacle.velocity = currentVectorMoveWorld;
            }
        }
    }

    //Time Solution vs realization pool object and enable and disable them vs Instatiate and Destroy
    protected void RemoveObstacleFromScreen()
    {
        poolObstacle.ReturnElement(arrObstaclesRemoveQueue.Dequeue());
    }

    public void InitMainSpawner(MainSpawner spawner) => mainSpawner = spawner;

    public void RemoveAllObstacleFromScreen()
    {
        int count = arrSpawnedObstacleWScore.Count;
        //Debug.Log($"RemoveAllObstacleFromScreen({this.gameObject.name}) = {count} ");
        for (int i = 0; i < count; i++)
        {
            poolObstacle.ReturnElement(arrSpawnedObstacleWScore.Dequeue().RigidbodyObstacle);
        }
    }
}
