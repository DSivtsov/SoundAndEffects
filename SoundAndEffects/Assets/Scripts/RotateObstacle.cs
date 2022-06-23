using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rotate Script to Obstacles with BoxColider. Used the Box collider to calculacted the radius.
/// To rotate use the Rigidbody.angularVelocity, which Updated from MoveRotateObstacleAndSpawn
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class RotateObstacle : MonoBehaviour
{
    [ReadOnly(BaseColor.white)]
    [SerializeField] private float speedRelative;

    private float _radius;
    private MovingWorldSO movingWorld;
    private Rigidbody rigidbodyObstacle;
    //private Vector3 m_EulerAngleVelocity;
    private MoveRotateObstacleAndSpawn moveObstacle;
    private float speedDiference;

    private void Awake()
    {
        movingWorld = SingletonController.Instance.GetMovingWorld();
        rigidbodyObstacle = GetComponent<Rigidbody>();
        moveObstacle = GetComponentInParent<MoveRotateObstacleAndSpawn>();
        if (moveObstacle)
        {
            moveObstacle.UpdatedCurrentVelocityMoveWorld += UpdateAngularSpeed;
        }
        else
        {
            Debug.LogError($"Parent Not Have the <MoveObstacleAndSpawn> component");
        }
        speedDiference = moveObstacle.speedModifier - 1;
        if (speedDiference == 0)
        {
            Debug.LogError($"Obstacle speedModifier doesn't allow to rotate (speedModifier = 1)");
        }
        BoxCollider collider = GetComponent<BoxCollider>();
        if (collider)
        {
            Vector3 size = collider.size;
            //the assumption that the diagonal is align the hypotenuse
            _radius = Mathf.Sqrt(size.x * size.x + size.z * size.x) / 2;
        }
        else
        {
            Debug.LogError($"Absent the BoxCollider component, can detect correct value of radius. Set to 1");
            _radius = 1f;
        }
    }

    private void Start()
    {
        UpdateAngularSpeed();
    }

    private void OnDisable()
    {
        moveObstacle.UpdatedCurrentVelocityMoveWorld -= UpdateAngularSpeed;
    }

    //private void FixedUpdate()
    //{
    //    //Set the angular velocity of the Rigidbody (rotating around the Y axis, 100 deg/sec)
    //    //The Angle speed is calculated in radians
    //    speedRelative = Mathf.Abs( movingWorld.CurrentSpeed * speedDiference);
    //    if (movingWorld.worldIsMoving)
    //    {
    //        //m_EulerAngleVelocity = new Vector3(0, Mathf.Rad2Deg * speedRelative / _radius, 0);
    //        //Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
    //        //rigidbodyObstacle.MoveRotation(rigidbodyObstacle.rotation * deltaRotation);
    //        rigidbodyObstacle.angularVelocity = new Vector3(0, 0, speedRelative / _radius);
    //    }
    //    //else
    //    //    rigidbodyObstacle.angularVelocity = new Vector3(0, 0, 0);

    //}

    /// <summary>
    /// Will be called at changing the CurrentVelocityMoveWorld from MoveRotateObstacleAndSpawn
    /// </summary>
    /// <param name="currentSpeed"></param>
    public void UpdateAngularSpeed()
    {
        //Set the angular velocity of the Rigidbody (rotating around the Y axis, 100 deg/sec)
        //The Angle speed is calculated in radians
        speedRelative = Mathf.Abs(movingWorld.CurrentSpeed * speedDiference);
        rigidbodyObstacle.angularVelocity = new Vector3(0, 0, speedRelative / _radius);
    }
}

