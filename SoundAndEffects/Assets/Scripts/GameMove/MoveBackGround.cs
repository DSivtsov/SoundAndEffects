//#define TRACEON
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object is moving by set the Transform.position at Update, based on movingWorld.CurrentSpeed
/// </summary>
public class MoveBackGround : MonoBehaviour
{
    private MovingWorldSO _movingWorld;
    private const float _xMaxMove = -56.4f;
    private const float _xInitialSpritePos = 0f;
    private float _yInitialSpritePos;
    private float _zInitialSpritePos;
    private float _zxCurrentSpritePos;
    private Transform _backGroundTransform;
    private CharacterDataController _characterDataCtrl;

    private void Awake()
    {
        _movingWorld = SingletonGame.Instance.GetMovingWorld();
        _characterDataCtrl = SingletonGame.Instance.GetCharacterDataCtrl();
        _backGroundTransform = GetComponent<Transform>();
        _zxCurrentSpritePos = _xInitialSpritePos;
        _yInitialSpritePos = _backGroundTransform.position.y;
        _zInitialSpritePos = _backGroundTransform.position.z;

        SetBackGroundPosition();
    }

    private void Update()
    {
        if (_movingWorld.worldIsMoving)
        {
            float deltaDistance = _movingWorld.CurrentSpeed * Time.deltaTime;
            UpdateBackGroundPosition(deltaDistance);
            _characterDataCtrl.AddDeltaDistance(Math.Abs(deltaDistance));
        }
    }

    private void UpdateBackGroundPosition(float deltaDistance)
    {
        _zxCurrentSpritePos += deltaDistance;
        if (_zxCurrentSpritePos < _xMaxMove)
        {
            _zxCurrentSpritePos = _xInitialSpritePos;
        }
        else
            SetBackGroundPosition();
    }

    private void SetBackGroundPosition() => _backGroundTransform.position = new Vector3(_zxCurrentSpritePos, _yInitialSpritePos, _zInitialSpritePos);

    public void UpdatedWorldSpeedForBackGround()
    {
        throw new System.NotImplementedException("UpdatedWorldSpeedForObstacles() not realized for MoveBackGround");
    }

}
