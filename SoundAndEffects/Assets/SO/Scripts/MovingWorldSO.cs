using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "MovingWorld", menuName = "ScriptableObject/MovingWorld")]
public class MovingWorldSO : ScriptableObject
{
    [Tooltip("Speed at m/s")]
    [SerializeField] private float moveSpeed;
    [Tooltip("Speed at m/s")]
    [SerializeField] private float runSpeed;
    [Tooltip("The distance which must pass the objects on Screen before remove them")]
    [SerializeField] private float xPositionOutScreen;
    public bool worldIsMoving { get; private set; }
    private float _currentSpeed;
    public float CurrentSpeed
    {
        get => _currentSpeed;
        private set
        {
            _currentSpeed = value;
            VectorSpeed = Vector3.right * value;
        }
    }

    public Vector3 VectorSpeed { get; private set; }

    /// <summary>
    /// Check the Position of Object
    /// </summary>
    /// <param name="xPosition">object go to negative direction</param>
    /// <returns>true if Object pass the demanded position</returns>
    public bool IsObjectReadyToRemove(float xPosition) => xPosition < xPositionOutScreen;

    /// <summary>
    /// Set the movement speed based on the current type of movement
    /// </summary>
    /// <param name="moveType"></param>
    public void SetWorldMovementSpeed(MovementType moveType)
    {
        switch (moveType)
        {
            case MovementType.Stop:
                worldIsMoving = false;
                UpdateWorldSpeed(0);
                break;
            case MovementType.Walk:
                worldIsMoving = true;
                UpdateWorldSpeed(-moveSpeed);
                break;
            case MovementType.Run:
                worldIsMoving = true;
                UpdateWorldSpeed(-runSpeed);
                break;
            default:
                Debug.LogError("SetMoveState wrong state");
                break;
        }
    }
     
    private void UpdateWorldSpeed(float newSpeed)
    {
        CurrentSpeed = newSpeed;
        SingletonController.Instance.InformAboutSpeedChange.Invoke();
    }
}
