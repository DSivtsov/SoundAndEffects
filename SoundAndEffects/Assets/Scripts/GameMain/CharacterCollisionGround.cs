using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detect the collision the Player with Ground
/// Physics set the Collider matrix - Layer Ground Can have collisions with Player Layer only
///  Therefore not demanding to check the collision object at PlayerGrounded
/// </summary>
public class CharacterCollisionGround : MonoBehaviour
{
    public bool IsGrounded { get; private set; } = true;

    private void OnCollisionExit()
    {
        IsGrounded = false;
    }

    private void OnCollisionEnter()
    {
        IsGrounded = true;
    }
}
