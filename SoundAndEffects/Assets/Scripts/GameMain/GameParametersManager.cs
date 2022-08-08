using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manage Game parameters (Complexity and FPS)
/// </summary>
public class GameParametersManager : MonoBehaviour
{

    [Header("Game Complexity Jumps")]
    [SerializeField] private ComplexitySO JumpComplexity;
    [Tooltip("The array of ForceJumpSO")]
    [SerializeField] private ForceJumpSO[] arrForceJump;

    [Header("Game Complexity Spacing Obstacles")]
    [Tooltip("Level up after this number of obstacles was spawned")]
    [SerializeField] private int NumObstaclesUpLevel = 10;
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
    private int countSpawnedAtThisLivel;

    public float Multiplier { get; private set; }
    public int Level { get; private set; }
    public int JumpComplexityMultiplier { get; private set; }

    private CharacterManager characterController;

    void Awake()
    {
        Application.targetFrameRate = FPS;
    }

    private void Start()
    {
        characterController = SingletonGame.Instance.GetCharacterController();
        if (!characterController)
            Debug.LogError($"Script {this.name} in {this.gameObject.scene.name} Scene not initialized");
        UpdateForceJumpForCurrentComplexity();

        ReInitParameters();
    }

    //Give a possibility to change the Complexity in process of the Game
    private void OnValidate()
    {
        //Debug.Log($"OnValidate() : SetForceJumpForCurrentComplexity({CurrentComplexity})");
        if (characterController)
            UpdateForceJumpForCurrentComplexity();
    }

    public void ReInitParameters()
    {
        Level = 1;
        LevelChanged.Invoke(Level);
        countSpawnedAtThisLivel = 0;
        UpdateLevelComplexity();
    }

    public void ChangeJumpComplexity(ComplexitySO complexitySO)
    {
        JumpComplexity = complexitySO;
        UpdateForceJumpForCurrentComplexity();
    }

    public void AddNewSpawnedObstacle() => countSpawnedAtThisLivel++;
    private void IncreaseLevel() => Level++;

    /// <summary>
    /// Every NumObstaclesUpLevel Obstacles Game will raise the Complexity of Level
    /// </summary>
    public void CheckAndUpdateLevelGame()
    {
        if (countSpawnedAtThisLivel == NumObstaclesUpLevel)
        {
            IncreaseLevel();
            LevelChanged.Invoke(Level);
            UpdateLevelComplexity();
            countSpawnedAtThisLivel = 0;
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
        foreach (ForceJumpSO item in arrForceJump)
        {
            if (item.ItemForComplexity(JumpComplexity))
            {
                characterController.SetForceJumpSO(item);
                JumpComplexityMultiplier = item.JumpComplexityMultipler;
            }
        }
        
    }


}
