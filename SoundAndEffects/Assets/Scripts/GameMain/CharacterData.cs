using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterData : MonoBehaviour
{
    [SerializeField] private int _maxHealth;
    public event Action<int> HealthChanged;
    private int _health;
    private float _summaryDistance;
    private PlayerCollisionGround _checkPlayer;
    private bool _characterInAir;

    public int Health
    {
        get => _health;
        set
        {
            if (value > _maxHealth)
                _health = _maxHealth;
            else if (value < 0)
                _health = 0;
            else
                _health = value;
        }
    }

    private void Awake()
    {
        _checkPlayer = SingletonGame.Instance.GetPlayerCollisionGround();
        ScoreCounting.InitScoreCounting(SingletonGame.Instance.GetGameParametersManager());
    }

    private void Start()
    {
        if (SingletonGame.Instance.GetGameSceneManager().GameMainManagerNotLinked)
        {
            ResetHealth();
            ResetScoreDistance();
        }
    }

    private void FixedUpdate()
    {
        if (_checkPlayer.IsGrounded && _characterInAir)
        {
            _characterInAir = false;
            ScoreCounting.JumpFinished();
            //ScoreChanged.Invoke(ScoreCounting.AllSum);
            return;
        }
        if (!_checkPlayer.IsGrounded && !_characterInAir)
        {
            _characterInAir = true;
            return;
        }
    }

    public void ResetHealth()
    {
        Health = _maxHealth;
        HealthChanged?.Invoke(Health);
    }

    public void ResetScoreDistance()
    {
        _summaryDistance = 0;
        _characterInAir = false;
        ScoreCounting.Reset();
        //ScoreChanged.Invoke(ScoreCounting.AllSum);
    }

    /// <summary>
    /// Change health
    /// </summary>
    /// <param name="decreaseAmmount">value with sign</param>
    public void ChangeHealth(int decreaseAmmount)
    {
        Health += decreaseAmmount;
        HealthChanged?.Invoke(Health);
    }

    public void AddDeltaDistance(float deltaDistance)
    {
        _summaryDistance += deltaDistance;
    }

    public int GetCurrentSummaryDistance() => Mathf.RoundToInt(_summaryDistance);

    public int GetCurrentSummaryScore() => ScoreCounting.AllSum;
}

