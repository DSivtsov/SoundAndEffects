//#define TRACEON
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object is moving by set the Transform.position at Update, based on movingWorld.CurrentSpeed
/// </summary>
public class MoveBackGround : MonoBehaviour
{
    private MovingWorldSO movingWorld;
    private const float xMaxMove = -56.4f;
    private const float xInitialSpritePos = 0f;
    
    private float yInitialSpritePos;
    private float zInitialSpritePos;
    private float xCurrentSpritePos;
    private Transform backGroundTransform;

    private void Awake()
    {
        movingWorld = SingletonController.Instance.GetMovingWorld();
        backGroundTransform = GetComponent<Transform>();
        xCurrentSpritePos = xInitialSpritePos;
        yInitialSpritePos = backGroundTransform.position.y;
        zInitialSpritePos = backGroundTransform.position.z;

        SetBackGroundPosition();
    }

    private void Update()
    {
        if (movingWorld.worldIsMoving)
        {
            UpdateBackGroundPosition();
        }
    }

    private void UpdateBackGroundPosition()
    {
        xCurrentSpritePos += movingWorld.CurrentSpeed * Time.deltaTime;
        if (xCurrentSpritePos < xMaxMove)
        {
            xCurrentSpritePos = xInitialSpritePos;
        }
        else
            SetBackGroundPosition();
    }

    private void SetBackGroundPosition() => backGroundTransform.position = new Vector3(xCurrentSpritePos, yInitialSpritePos, zInitialSpritePos);

    public void UpdatedWorldSpeedForBackGround()
    {
        throw new System.NotImplementedException("UpdatedWorldSpeedForObstacles() not realized for MoveBackGround");
    }

}
