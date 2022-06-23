using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveRotateObstacleAndSpawn : MoveObstacleAndSpawn
{
    [Tooltip("Set for this type of obstacle the relative speed in relation to the ground speed of World. Must be higher then 1 if obstacle are moving")]
    [Range(1, 3f)]
    public float speedModifier = 1f;

    public event Action UpdatedCurrentVelocityMoveWorld;

    protected override void UpdateCurrentVelocityMoveWorld()
    {
        currentVectorMoveWorld = movingWorld.VectorSpeed * speedModifier;
        //Debug.Log($"{this} : movingWorld={movingWorld} Length={UpdatedCurrentVelocityMoveWorld?.GetInvocationList().Length}");
        UpdatedCurrentVelocityMoveWorld?.Invoke();
    }
}
