using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ForceJump", menuName = "SoundAndEffects/ForceJump")]
public class ForceJumpSO : ScriptableObject
{
    [SerializeField] private ComplexitySO _complexity;
    //Speed at m/s
    [SerializeField] private float _forceJumpWalk = 350f;
    [SerializeField] private float _forceJumpRun = 450f;
    [SerializeField] private int _jumpComplexityMultipler = 1;

    public float ForceJumpWalk { get => _forceJumpWalk; }
    public float ForceJumpRun { get => _forceJumpRun; }
    public int JumpComplexityMultipler { get => _jumpComplexityMultipler; }

    public ComplexitySO Complexity { get => _complexity; }
    /// <summary>
    /// Check complexity of Item
    /// </summary>
    /// <param name="checkComplexity"></param>
    /// <returns>true if Item for this complexity</returns>
    public bool ItemForComplexity (ComplexitySO checkComplexity) => checkComplexity ==_complexity;

}
