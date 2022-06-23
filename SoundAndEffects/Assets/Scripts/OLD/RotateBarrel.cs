using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBarrel : MonoBehaviour
{
    public int speedRotation = 30;

    private MovingWorldSO movingWorld;

    private void Awake()
    {
        movingWorld = SingletonController.Instance.GetMovingWorld();
    }

    void Update()
    {
        if (movingWorld.worldIsMoving)
        {
            transform.Rotate(Vector3.up, speedRotation * Time.deltaTime); 
        }
    }
}
