using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Manage Initial Game parameters (Complexity and FPS) and Game state
/// </summary>
public class ManagerGame : MonoBehaviour
{
    [Tooltip("Selected the base Game Complexity")]
    [SerializeField] private ComplexitySO InitialComplexity;
    [Tooltip("The array of ForceJumpSO")]
    [SerializeField] private ForceJumpSO[] arrForceJump;
    [Range(30, 60)]
    [Tooltip("Works at Awake")]
    [SerializeField] private int FPS = 30;

    private MyCharacterController characterController;

    void Awake()
    {
        Application.targetFrameRate = FPS;
        characterController = SingletonGame.Instance.GetCharacterController();

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
    /// <summary>
    /// Change ForceJump related to CurrentComplexity
    /// </summary>
    private void SetForceJumpForCurrentComplexity()
    {
        //If character Controller not initialized then return
        if (characterController)
            foreach (ForceJumpSO item in arrForceJump)
            {
                if (item.ItemForComplexity(InitialComplexity))
                {
                    characterController.SetForceJumpSO(item);
                }
            }
    }

    public void RestartGame()
    {
        Debug.Log("RestartGame()");
    }

    public void ExitGame()
    {
        Debug.Log("ExitGame()");
        Application.Quit();
    }
}
