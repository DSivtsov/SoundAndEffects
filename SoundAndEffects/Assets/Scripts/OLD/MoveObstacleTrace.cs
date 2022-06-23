//#define TRACEON
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using MyPlayerControler;

/// <summary>
/// Object is moving by Rigidbody.MovePosition() at FixedUpdate with set option Interpolate
/// </summary>
public class MoveObstacleTrace : MonoBehaviour
{
    [Range(1,3f)]
    public float speedModifier = 1f;
    //[SerializeField] private MovingWorld movingWorld;
    private MovingWorldSO movingWorld;

    [SerializeField] private Rigidbody rigidbodyObstacle;

#if TRACEON
    //It used only if defined #TRACEON through the ConditionalAttribute
    TraceObjectMove MoveObstaclesUpdate, MoveObstaclesFixedUpdate; 
#endif

    private void Awake()
    {
        movingWorld = SingletonController.Instance.GetMovingWorld();
        //rigidbodyObstacle = GetComponentInChildren<Rigidbody>();
        if (!rigidbodyObstacle)
        {
            Debug.LogError($"Not Set <Rigidbody> field");
        }

#if TRACEON
        MoveObstaclesUpdate = new TraceObjectMove("MoveObstacles : Update", rigidbodyObstacle.gameObject.transform);
        MoveObstaclesFixedUpdate = new TraceObjectMove("MoveObstacles : FixedUpdate", rigidbodyObstacle.gameObject.transform); 
#endif
    }

    private void FixedUpdate()
    {
#if TRACEON
        MoveObstaclesFixedUpdate.InterateFixedUpdate(); 
#endif
        if (movingWorld.worldIsMoving)
        {
            rigidbodyObstacle.MovePosition(rigidbodyObstacle.gameObject.transform.position + Vector3.right * movingWorld.CurrentSpeed * Time.fixedDeltaTime * speedModifier);
#if TRACEON
            MoveObstaclesFixedUpdate.ShowDeltaXAndDeltaTime(TraceObjectMove.TypeCycle.FixedUpdate); 
#endif
        }
    }

#if TRACEON
    private void Update()
    {
        if (movingWorld.worldIsMoving)
        {
            MoveObstaclesUpdate.ShowDeltaXAndDeltaTime(TraceObjectMove.TypeCycle.Update);
        }
    }

    private void LateUpdate()
    {
        MoveObstaclesFixedUpdate.InterateUpdate();
    } 
#endif

}
