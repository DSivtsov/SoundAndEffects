using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Direved class for rotated objects
/// </summary>
public class MoveRotateObstacleAndSpawn : MoveObstacleAndSpawn
{
    [Tooltip("Set for this type of obstacle the relative speed in relation to the ground speed of World. Must be higher then 1 if obstacle are moving")]
    [Range(1, 3f)]
    public float speedModifier = 1f;

    public event Action UpdatedCurrentVelocityMoveWorld;

    /// <summary>
    /// The Rotated objected must move faster then "Ground" and updates its Abgular speed if was updatted the "Ground speed"
    /// </summary>
    protected override void UpdateCurrentVelocityMoveWorld()
    {
        currentVectorMoveWorld = movingWorld.VectorSpeed * speedModifier;
        UpdatedCurrentVelocityMoveWorld?.Invoke();
    }
}
