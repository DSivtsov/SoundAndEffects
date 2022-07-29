using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterDataController : MonoBehaviour
{
    private readonly struct PassedObstacleWScore
    {
        public readonly int IDObstacle;
        public readonly int ScoreObstacle;

        public PassedObstacleWScore(int IDObstacle, int ScoreObstacle)
        {
            this.IDObstacle = IDObstacle;
            this.ScoreObstacle = ScoreObstacle;
        }
    }

    [SerializeField] private int _maxHealth;

    public event Action<int> HealthChanged;
    public event Action<int> ScoreChanged;

    private int _health;
    private float _summaryDistance;
    private int _summaryScore;

    private PlayerCollisionGround _checkPlayer;
    private GameParametersManager _gameParametersManager;
    private bool _characterInAir;
    private Stack<PassedObstacleWScore> passedObstaclesWScore = new Stack<PassedObstacleWScore>(10);


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

    public int SummaryScores
    {
        get => _summaryScore;
    }
    /// <summary>
    /// Summary distance in meters
    /// </summary>
    public int SummaryDistance
    {
        get => Mathf.RoundToInt(_summaryDistance);
    } 

    private void Awake()
    {
        _checkPlayer = SingletonGame.Instance.GetPlayerCollisionGround();
        _gameParametersManager = SingletonGame.Instance.GetGameParametersManager();
    }

    private void Start()
    {
        if (!SingletonGame.Instance.GetGameSceneManager().GameMainManagerLinked)
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
            ScoreAfterJumpFinished();
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
        _summaryScore = 0;
        ScoreChanged.Invoke(0);
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

    public void AddObstacleToStack(int IDObstacle, int ScoreObstacle)
    {
        passedObstaclesWScore.Push(new PassedObstacleWScore(IDObstacle, ScoreObstacle));
    }

    public void ScoreAfterJumpFinished()
    {
        //CountFrame.DebugLogFixedUpdate("-------------Jump Finished--------------");
        int sum = 0;
        foreach (PassedObstacleWScore item in passedObstaclesWScore)
        {
            //Debug.Log($"{item.Key} {item.Value}");
            sum += item.ScoreObstacle;
        }
        //CountFrame.DebugLogFixedUpdate($"[Jump Finished] Count={scorePassedObstacles.Count} sum={sum}");
        _summaryScore += sum * ((passedObstaclesWScore.Count > 1) ? _gameParametersManager.Level + passedObstaclesWScore.Count : 1);
        ScoreChanged.Invoke(_summaryScore);
        passedObstaclesWScore.Clear();
    }

    public void ScoreAfterCollisionOccur(int IDObstacleCollided)
    {
        //UnityEngine.Debug.Log($"count={passedObstaclesWScore.Count}");
        if (passedObstaclesWScore.Count > 0)
        {
            //UnityEngine.Debug.Log($"before AllSum={AllSum}");
            PassedObstacleWScore lastPassedObstacle = passedObstaclesWScore.Peek();
            if (lastPassedObstacle.IDObstacle == IDObstacleCollided)
                passedObstaclesWScore.Pop();
            if (passedObstaclesWScore.Count > 0)
            {
                ScoreAfterJumpFinished();
                //UnityEngine.Debug.Log($"after JumpFinished  AllSum={AllSum}");
            }
        }
    }
}

