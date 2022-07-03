using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Set the Complexity of Game and other Game parameters
/// </summary>
public class ManagerGameSettings : MonoBehaviour
{
    public ComplexitySO CurrentComplexity;
    [Tooltip("The array of ForceJumpSO")]
    public ForceJumpSO[] arrForceJump;
    [Range(30, 60)]
    [SerializeField] private int FrameRate = 30;

    private MyCharacterController characterController;

    void Awake()
    {
        Application.targetFrameRate = FrameRate;
        characterController = SingletonController.Instance.GetCharacterController();

        SetForceJumpForCurrentComplexity(); 
    }

    //Give a possibility to change the Complexity in process of the Game
    private void OnValidate()
    {
#if UNITY_EDITOR
        //Debug.Log($"OnValidate() : SetForceJumpForCurrentComplexity({CurrentComplexity})");
        SetForceJumpForCurrentComplexity();
#endif
    }

    private void SetForceJumpForCurrentComplexity()
    {
        //If character Controller not initialized then return
        if (characterController)
            foreach (ForceJumpSO item in arrForceJump)
            {
                if (item.ItemForComplexity(CurrentComplexity))
                {
                    characterController.SetForceJumpSO(item);
                }
            }
    }
}
