using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject gameCamera;
    //[SerializeField] private GameObject gameCanvas;

    [SerializeField] private AudioSource audioSource;

    private GameMainManager _gameMainManager;
    private CharacterData _characterData;
    private MainSpawner _mainSpawner;
    private GraveStoneControl _graveStoneControl;
    private GameParametersManager _gameParametersManager;
    private GameObject _characterObject;
    private CharacterManager _characterManager;
    private PlayJukeBoxCollection _playJukeBoxGameCollection;

    private const int DecreaseAmmountLife = -1;
    public bool GameMainManagerNotLinked { get; private set; }

    private void Awake()
    {
        CountFrame.DebugLogUpdate(this, $"Awake()");
        _characterData = SingletonGame.Instance.GetCharacterData();
        _mainSpawner = SingletonGame.Instance.GetMainSpawner();
        _graveStoneControl = SingletonGame.Instance.GetGraveStoneControl();
        _gameParametersManager = SingletonGame.Instance.GetGameParametersManager();
        _gameMainManager = GameMainManager.Instance;
        _characterManager = SingletonGame.Instance.GetCharacterController();
        _characterObject = _characterManager.gameObject;
        _playJukeBoxGameCollection = SingletonGame.Instance.GetPlayJukeBox();

        if (_gameMainManager)
        {
            GameMainManagerNotLinked = false;
            _gameMainManager.LinkGameSceneManager(this);
            //Camera will manage by GameMainManager
            ActivateGameCamera(false);
        }
        else
        {
            Debug.LogError($"{this} not linked to GameMainManager");
            GameMainManagerNotLinked = true;
            ActivateGameCamera(true);
            //TurnOnMusic();
            Debug.LogWarning($"Music at Awake not TurnOn because in build will be used Music from Menu Scene");
        }
    }
    /// <summary>
    /// Move EventSystem focus from any buttons to GameObject on current scene, to give possibility to catch the Enter press vs repeat "Button click"
    /// </summary>
    public void SelectGameObjectFromScene(GameObject gameObject)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void StartNewGame()
    {
        _gameParametersManager.ReInitParameters();
        //The Character GameObject will be Turn off at EndGame to reinit the Character Animator
        _characterObject.SetActive(true);
        SelectGameObjectFromScene(_characterObject);
        _characterData.ResetHealth();
        //WaitState before restart Game
        if (_characterManager.WaitState == TypeWaitMsg.waitEndGame)
            _characterData.ResetScoreDistance();
        _mainSpawner.ReStartSpawner();
        _characterManager.StartNewAttemptGame();
    }

    public void TurnOnMusic(bool turnOn = true) => _playJukeBoxGameCollection.TurnOn(turnOn);

    public void SwitchMusicCollection(CollectionName collectionName, bool turnOnMusicAfterSwitch = true)
        => _playJukeBoxGameCollection.SwitchCollection(collectionName, turnOnMusicAfterSwitch);

    public void ActivateGameCamera(bool activate) => gameCamera.SetActive(activate);

    /// <summary>
    /// Try decrease lifes and check the character not died after a collision
    /// </summary>
    /// <returns>true if it not died</returns>
    public bool CharacterNotDiedAfterCollision()
    {
        _characterData.ChangeHealth(DecreaseAmmountLife);
        // Character lost the one life and continue the game or Character starts to die 
        return _characterData.Health > 0;
    }

    public void CharacterCollision()
    {
        //Debug.LogWarning("GameSceneManager : CharacterCollision()");
        _mainSpawner.RemoveAllObstacles();
        _mainSpawner.ReStartSpawner();
    }

    public void CharacterDied()
    {
        Debug.LogWarning("GameSceneManager : CharacterDied()");
        _mainSpawner.RemoveAllObstacles();
        _graveStoneControl.ActivateGraveStoneGroupAndFocusInputField();
        //Debug.LogError("Temp Solution CharacterDied() audioSource.Play()");
        //audioSource.Play();
    }

    public void ClearSceneAfterEndGame()
    {
        audioSource.Stop();
        _graveStoneControl.DeactivategraveStoneGroup();
        //Turn off Character GameObject to reinit the Character Animator
        _characterObject.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("ExitGame()");
        Application.Quit();
    }

    public void SwitchMusicToGameScene()
    {
        if (!GameMainManagerNotLinked)
            GameMainManager.Instance.SwitchMusicTo(SceneName.Game);
        else
            TurnOnMusic();
    }

    public void StoreResultAndSwitchGameToMainMenus()
    {
        _graveStoneControl.StoreUserResult();
        if (!GameMainManagerNotLinked)
        {//It's end Game and Scene linked to GameMainManager
            GameMainManager.Instance.FromGameToMenus();
        }
        //It's common part of the EndGame  for Scenes are linked or  NOT linked to GameMainManager
        ClearSceneAfterEndGame();
    }
}
