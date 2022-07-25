using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject gameCamera;
    [SerializeField] private GameObject gameCanvas;

    [SerializeField] private AudioSource audioSource;

    private GameMainManager _gameMainManager;
    private CharacterData _characterData;
    private MainSpawner _mainSpawner;
    private GraveStoneControl _graveStoneControl;

    private const int DecreaseAmmountLife = -1;
    public bool GameMainManagerOff { get; private set; }

    private void Awake()
    {
        _gameMainManager = GameMainManager.Instance;
        _characterData = SingletonGame.Instance.GetCharacterData();
        _mainSpawner = SingletonGame.Instance.GetMainSpawner();
        _graveStoneControl = SingletonGame.Instance.GetGraveStoneControl();
        if (_gameMainManager)
        {
            GameMainManagerOff = false;
            _gameMainManager.LinkGameSceneManager(this);
        }
        else
        {
            Debug.LogError($"{this} not linked to GameMainManager");
            GameMainManagerOff = true;
        }
    }

    public void TurnOnGame(bool value)
    {
        if (value)
        {
            gameCanvas.SetActive(true);
            SingletonGame.Instance.GetCharacterController().gameObject.SetActive(false);
            SingletonGame.Instance.GetCharacterController().gameObject.SetActive(true);
            _characterData.ResetHealth();
            SingletonGame.Instance.GetCharacterController().ReStartGame();
        }
        else
            gameCanvas.SetActive(false);

    }


    /// <summary>
    /// Try decrease lifes and check the character not died after a collision
    /// </summary>
    /// <returns>true if it not died</returns>
    public bool CharacterNotDiedAfterCollision()
    {
        _characterData.ChangeHealth(DecreaseAmmountLife);
        if (_characterData.Health > 0)
        {
            // Character lost the one life and continue the game
            return true;
        }
        else
        {
            //Character starts to die 
            return false;
        }
    }

    public void CharacterCollision()
    {
        Debug.LogWarning("GameSceneManager : CharacterCollision()");
        _mainSpawner.RemoveAllObstacles();
        _mainSpawner.Start();
    }

    public void CharacterDied()
    {
        Debug.LogWarning("GameSceneManager : CharacterDied()");
        _mainSpawner.RemoveAllObstacles();
        _graveStoneControl.ActivategraveStoneGroupAndFocusInputField();
        audioSource.Play();
    }

    public void ClearReinitScene()
    {
        audioSource.Stop();
        _graveStoneControl.DeactivategraveStoneGroup();
        //SingletonGame.Instance.GetCharacterController().ReStartGame();
    }
}
