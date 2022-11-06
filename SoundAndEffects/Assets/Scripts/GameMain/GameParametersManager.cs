using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// Manage Game parameters (Complexity and FPS)
/// </summary>
public class GameParametersManager : MonoBehaviour
{

    [Header("Game Complexity Jumps")]
    [SerializeField] private ComplexitySO _gameComplexity;
    [Tooltip("The array of ComplexitySO")]
    [SerializeField] private ComplexitySO[] _arrGameComplexity;
    [Tooltip("The array of ForceJumpSO")]
    [SerializeField] private ForceJumpSO[] _arrForceJump;

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

    public event Action<int> LevelChanged;
    private int _countSpawnedAtThisLivel;
    public float Multiplier { get; private set; }
    public int Level { get; private set; }
    public int JumpComplexityMultiplier { get; private set; }

    private CharacterManager _characterManager;
    /// <summary>
    /// Tha Base collection which used to create the _orderGameComplexityValues and the "ListGameComplexityValues", which will have the same order as this collection,
    /// becuase create by LINQ.Select() and therefore it can be used to get the ComplexitySO based on the position of the selected value in the ListGameComplexityValues in DropBox
    /// </summary>
    private List<ComplexitySO> _listGameComplexityValues;
    /// <summary>
    /// The collection which used to get the index of the ComplexitySO in _listGameComplexityValues which will be the same as index of arrForceJump record which contains
    /// the same ComplexitySO value
    /// </summary>
    private Dictionary<ComplexitySO, int> _idxGameComplexityValues = new Dictionary<ComplexitySO, int>();

    void Awake()
    {
        _characterManager = SingletonGame.Instance.GetCharacterManager();

        Application.targetFrameRate = FPS;
        InitCompexityParameters();
    }

    private void Start()
    {
        UpdateForceJumpForCurrentComplexity();
        ReInitParameters();
    }

    //Give a possibility to change the Complexity in process of the Game
    private void OnValidate()
    {
        //Debug.Log($"OnValidate() : SetForceJumpForCurrentComplexity({CurrentComplexity})");
        if (_characterManager)
            UpdateForceJumpForCurrentComplexity();
    }

    private void InitCompexityParameters()
    {
        _listGameComplexityValues = _arrForceJump.Select((ForceJumpSO record) => record.Complexity).ToList();
        for (int i = 0; i < _listGameComplexityValues.Count; i++)
        {
            _idxGameComplexityValues.Add(_listGameComplexityValues[i], i);
        }
    }

    public void ReInitParameters()
    {
        Level = 1;
        LevelChanged.Invoke(Level);
        _countSpawnedAtThisLivel = 0;
        UpdateLevelComplexity();
    }

    public UnityAction<int> GetActionOnValueChanged() => (int newValue) =>
    {
        //Debug.Log($"_listGameComplexityValues[newValue]={_listGameComplexityValues[newValue].name}");
        ChangeJumpComplexity(_listGameComplexityValues[newValue]);
    };

    public int GetInitialValueGameComplexity() => _idxGameComplexityValues[_gameComplexity];

    public List<string> GetListGameComplexityValues() => _listGameComplexityValues.Select(record => record.name).ToList();

    public void ChangeJumpComplexity(ComplexitySO complexitySO)
    {
        _gameComplexity = complexitySO;
        UpdateForceJumpForCurrentComplexity();
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
        //Get 0.99 at (LevelMinMultiplier-1) Level and ~1 at Level = LevelMinMultiplier
        float procentLerp = 1f - Mathf.Exp(Mathf.Log(1f - 0.99f) / (_levelMinMultiplier - 1) * (Level - 1));
        Multiplier = Mathf.Lerp(_maxMultiplier, _minMultiplier, procentLerp);
        ////Debug.Log($"UpdateMultiplier : Level={Level} procentLerp={procentLerp:F2} Multiplier={Multiplier:F2}");
    }

    /// <summary>
    /// Update characterController.ForceJump related to CurrentComplexity
    /// </summary>
    private void UpdateForceJumpForCurrentComplexity()
    {
        ForceJumpSO item = _arrForceJump[_idxGameComplexityValues[_gameComplexity]];
        //Debug.Log($"nameComplexity={JumpComplexity.name} [{_orderGameComplexityValues[JumpComplexity]}]" +
        //    $" {arrForceJump[_orderGameComplexityValues[JumpComplexity]].name}");
        if (item.ItemForComplexity(_gameComplexity))
        {
            _characterManager.SetForceJumpSO(item);
            JumpComplexityMultiplier = item.JumpComplexityMultipler;
        }
    }
}
