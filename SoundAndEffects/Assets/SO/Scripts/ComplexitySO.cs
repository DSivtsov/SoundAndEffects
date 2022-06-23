using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Complexity", menuName = "ScriptableObject/Complexity")]
public class ComplexitySO : ScriptableObject
{
    [SerializeField] private string complexity;

    //private void Awake()
    //{
    //    Debug.Log("Complexity : Awake()");
    //}

    //private void OnEnable()
    //{
    //    Debug.Log("Complexity : OnEnable()");
    //}



    //public bool worldIsMoving { get; private set; } = false;
    //public float CurrentSpeed { get; private set; }

    //public float xMaximumMoveDistance = 36f;

    //public void SetMoveState(MoveState state)
    //{
    //    switch (state)
    //    {
    //        case MoveState.Stop:
    //            worldIsMoving = false;
    //            break;
    //        case MoveState.Walk:
    //            worldIsMoving = true;
    //            CurrentSpeed = -moveSpeed;
    //            break;
    //        case MoveState.Run:
    //            worldIsMoving = true;
    //            CurrentSpeed = -runSpeed;
    //            break;
    //        default:
    //            Debug.LogError("SetMoveState wrong state");
    //            break;
    //    }
    //}
}
