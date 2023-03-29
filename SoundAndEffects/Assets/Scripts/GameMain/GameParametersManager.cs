using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Manage Game Scene parameters
/// </summary>
public class GameParametersManager : MonoBehaviour
{
    [Serializable]
    struct UnitTest
    {
        public ComplexitySO _complexityGame;
        public bool _notShowCollisionAnimation;
    }
    private const int StandartStartHealthForNormalMode = 3;
    [SerializeField] private int _startHealth;
    [SerializeField] private GameSettingsSO _gameSettings;
    [SerializeField] private ComplexityParametersSO[] _arrComplexityParameters;
    [Header("Game Complexity Spacing Obstacles")]
    [Tooltip("Level up after this number of obstacles was spawned")]
    [SerializeField] private int _numObstaclesUpLevel = 10;
    [Tooltip("The maximum spacing Obstacles at Game start at Level = 1")]
    [SerializeField] private float _maxMultiplier = 5;
    [Tooltip("The minimum spacing Obstacles at Selected Level and nexts")]
    [SerializeField] private float _minMultiplier = 1;
    [Tooltip("The Selected Level with the minimal spacing Obstacles. At previous Level the value = min + (max - min) * 0,01")]
    [SerializeField] private int _levelMinMultiplier = 6;
    [Header("Common Parameters")]
    [Range(30, 60)]
    [Tooltip("Works at Awake")]
    [SerializeField] private int FPS = 30;
    [Header("Settings for UnitTests")]
    [SerializeField] private UnitTest _unitTestGameSettings;
    [SerializeField] private bool _isWalkingAfterStart;
    [SerializeField] private bool _isTurnOffAllObstacle;
    [SerializeField] private bool _isPlayerNotCollide;
    public int StartHealth { get => _startHealth; }
    public bool IsWalkingAfterStart { get => _isWalkingAfterStart; }

    public bool IsTurnOffAllObstacle { get => _isTurnOffAllObstacle; }

    public bool IsPlayerNotCollide { get => _isPlayerNotCollide; }

    public event Action<int> LevelChanged;
    private int _countSpawnedAtThisLivel;
    public float Multiplier { get; private set; }
    public int Level { get; private set; }
    public int ComplexityMultiplierScore { get; private set; }

    private CharacterManager _characterManager;
    private GameSceneManager _gameSceneManager;
    private bool _checkPassedArrComplexityParameters = false;

    void Awake()
    {
        _characterManager = SingletonGame.Instance.GetCharacterManager();
        _gameSceneManager = SingletonGame.Instance.GetGameSceneManager();
        Application.targetFrameRate = FPS;
        CheckArrComplexityParameters();
    }

    //private void Start() => ReInitParameters();
    private void Start()
    {
        //Turn off all Testing modes in Build
#if UNITY_EDITOR
        if (_isWalkingAfterStart || _isPlayerNotCollide || _isTurnOffAllObstacle)
        {
            Debug.LogWarning("Game in Testing mode");
        }
#else
        _isWalkingAfterStart = false;
        _isTurnOffAllObstacle = false;
        _isPlayerNotCollide = false;
#endif
    }
    /// <summary>
    /// For Normal mode will use value StandartStartHealthForNormalMode always
    /// </summary>
    /// <param name="overrideStartHealth"></param>
    public void OverrideStartHealth(int overrideStartHealth)
    {
        if (overrideStartHealth != 0)
            _startHealth = overrideStartHealth;
        else
            _startHealth = StandartStartHealthForNormalMode;
    }

    private void CheckArrComplexityParameters()
    {
        if (_arrComplexityParameters.Length == 0)
        {
            Debug.LogError($"{this}: array ForceJumpSO is Empty");
            ComplexityMultiplierScore = 1;
        }
        else
            _checkPassedArrComplexityParameters = true;
    }

    public void ReInitParameters()
    {
        SetGameSettingsParameters();
        Level = 1;
        LevelChanged.Invoke(Level);
        _countSpawnedAtThisLivel = 0;
        UpdateLevelComplexity();
    }
    /// <summary>
    /// Set Complexity and ShowCollisionAnimation Parameters
    /// </summary>
    private void SetGameSettingsParameters()
    {
        if (_gameSceneManager.GameMainManagerLinked)
        {
            SetComplexityParameters(_gameSettings.FieldComplexityGame.GetCurrentValue());
            SetShowCollisionAnimation(!_gameSettings.FieldNotShowCollisionAnimation.GetCurrentValue());
        }
        else
        {
            SetComplexityParameters(_unitTestGameSettings._complexityGame);
            SetShowCollisionAnimation(!_unitTestGameSettings._notShowCollisionAnimation);
        }
    }

    public void AddNewSpawnedObstacle() => _countSpawnedAtThisLivel++;
    private void IncreaseLevel() => Level++;

    /// <summary>
    /// Every NumObstaclesUpLevel Obstacles Game will raise the Complexity of Level
    /// </summary>
    public void CheckAndUpdateLevelGame()
    {
        if (_countSpawnedAtThisLivel == _numObstaclesUpLevel)
        {
            IncreaseLevel();
            LevelChanged.Invoke(Level);
            UpdateLevelComplexity();
            _countSpawnedAtThisLivel = 0;
        }
    }

    /// <summary>
    /// Update the Space Multiplier parameter - the Level of complexity based on the current Game Level and Game Settings
    /// </summary>
    private void UpdateLevelComplexity()
    {
        //procentLerp Must Get 0.99 at (LevelMinMultiplier-1) Level and ~1 at Level = LevelMinMultiplier
        float procentLerp = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / (_levelMinMultiplier - 1) * (Level - 1));
        Multiplier = Mathf.Lerp(_maxMultiplier, _minMultiplier, procentLerp);
    }

    private void SetComplexityParameters(ComplexitySO currentGameComplexity)
    {
        if (_checkPassedArrComplexityParameters)
        {
            ComplexityParametersSO complexityParameters = GetComplexityParametersSO(currentGameComplexity);
            _characterManager.SetForceJump(complexityParameters);
            ComplexityMultiplierScore = complexityParameters.ComplexityMultiplerScore; 
        }
    }

    private ComplexityParametersSO GetComplexityParametersSO(ComplexitySO currentComplexity)
    {
        for (int i = 0; i < _arrComplexityParameters.Length; i++)
        {
            if (_arrComplexityParameters[i].Complexity == currentComplexity)
                return _arrComplexityParameters[i];
        }
        Debug.LogError($"{this}: absent ComplexityParametersSO for [{currentComplexity}] complexity, will use the first founded ComplexityParametersSO object");
        return _arrComplexityParameters[0];
    }

    private void SetShowCollisionAnimation(bool show) => _characterManager.SetShowCollisionAnimation(show);
}
