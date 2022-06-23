using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attached to Obstacles
/// Physics set the Collider matrix - Layer Obstcales Can have collisions with Player Layer only
//  Therefore not demanding to check the collision object
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class CollisionObstacle : MonoBehaviour
{
    //public MyCharacterController characterController;
    private MyCharacterController characterController;

    private void Awake()
    {
        characterController = SingletonController.Instance.GetCharacterController();
    }

    /// <summary>
    /// Collision The Player with Obstacle
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.GetContact(0);
        print(contact.thisCollider.name + " hit " + contact.otherCollider.name);
        // Visualize the contact point
        Debug.DrawRay(contact.point, -contact.normal * 3, Color.blue,10f);

        characterController?.ObstacleCollision(contact.point, contact.normal);
    }
}
