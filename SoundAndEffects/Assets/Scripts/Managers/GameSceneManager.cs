using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject gameCamera;
    [SerializeField] private PlayJukeBoxCollection _playJukeBoxGameCollection;
    [SerializeField] private GraveStoneControl _graveStoneControl;
    [SerializeField] private MainSpawner _mainSpawner;

    private MainManager _gameMainManager;
    private CharacterDataController _characterDataCtrl;
    private GameParametersManager _gameParametersManager;
    private GameObject _characterObject;
    private CharacterManager _characterManager;

    private const int DecreaseAmmountLife = -1;
    private string _nameCurrentPlayer;
    private bool _activateButtonLocalRestart = false;

    public bool GameMainManagerLinked { get; private set; }

    private void Awake()
    {
        CountFrame.DebugLogUpdate(this, $"Awake()");
        _characterDataCtrl = SingletonGame.Instance.GetCharacterDataCtrl();
        _gameParametersManager = SingletonGame.Instance.GetGameParametersManager();
        _gameMainManager = MainManager.Instance;
        _characterManager = SingletonGame.Instance.GetCharacterManager();
        _characterObject = _characterManager.gameObject;

        if (_gameMainManager)
        {
            GameMainManagerLinked = true;
            _gameMainManager.LinkGameSceneManager(this);
            //at Normal Mode GameController and GameCamera must be off 
            ActivateGameCamera(false);
            _characterObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"{this} not linked to GameMainManager");
            GameMainManagerLinked = false;
            //At UnitTest Mode GameController and GameCamera must be on 
            _characterObject.SetActive(true);
            ActivateGameCamera(true);
        }
    }

    private void Start()
    {
        if (!GameMainManagerLinked)
        {
            StartNewGame("Test"); 
        }
    }

    /// <summary>
    /// Move EventSystem focus from any buttons to GameObject on current scene, to give possibility to catch the Enter press vs repeat "Button click"
    /// </summary>
    public void SelectGameObjectFromScene(GameObject gameObject)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void StartNewGame(string playerName, int overrideCharacterHealth = 0)
    {
        _nameCurrentPlayer = playerName;
        //_activateButtonLocalRestart = false;
        ActivateButtonLocalRestart(activate: false);
        _gameParametersManager.ReInitParameters();
        //The Character GameObject will be Turn off at EndGame to reinit the Character Animator
        _characterObject.SetActive(true);
        SelectGameObjectFromScene(_characterObject);
        ResetHealthWithOverride(overrideCharacterHealth);
        ////WaitState before restart Game
        //if (_characterManager.CurrentWaitType == WaitType.waitEndGame)
        _characterDataCtrl.ResetScoreDistance();
        _mainSpawner.ReStartSpawner();
        _characterManager.StartNewAttemptGame();
    }
    /// <summary>
    /// For not linked mode will use value from Inspector otherwise the overrideCharacterHealth value
    /// </summary>
    /// <param name="overrideCharacterHealth"></param>
    private void ResetHealthWithOverride(int overrideCharacterHealth)
    {
        if (_gameMainManager)
            _gameParametersManager.OverrideStartHealth(overrideCharacterHealth); 
        _characterDataCtrl.ResetHealth();
    }

    public void ActivateGameMusic(bool activate = true)
    {
        if (activate)
        {
            _playJukeBoxGameCollection.SetJukeBoxActive(true);
            _playJukeBoxGameCollection.TurnOn(true);
        }
        else
        {
            _playJukeBoxGameCollection.TurnOn(false);
            _playJukeBoxGameCollection.SetJukeBoxActive(false);
        }
    }

    public void SwitchMusicCollection(CollectionName collectionName, bool turnOnMusicAfterSwitch = true)
        => _playJukeBoxGameCollection.SwitchCollection(collectionName, turnOnMusicAfterSwitch);

    public void ActivateGameCamera(bool activate) => gameCamera.SetActive(activate);

    /// <summary>
    /// Try decrease lifes and check the character not died after a collision
    /// </summary>
    /// <returns>true if it not died</returns>
    public bool CharacterNotDiedAfterCollision()
    {
        _characterDataCtrl.ChangeHealth(DecreaseAmmountLife);
        // Character lost the one life and continue the game or Character starts to die 
        return _characterDataCtrl.Health > 0;
    }

    public void CharacterCollision()
    {
        CountFrame.DebugLogUpdate(this, "CharacterCollision()");
        _mainSpawner.RemoveAllObstacles();
        _mainSpawner.ReStartSpawner();
    }

    public void CharacterDied()
    {
        _mainSpawner.RemoveAllObstacles();
        _graveStoneControl.ActivateGraveStoneGroupAndFocusInputField(_nameCurrentPlayer);
    }

    public void ClearSceneAfterEndGame()
    {
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
        if (GameMainManagerLinked)
            MainManager.Instance.SwitchMusicTo(SceneName.Game);
        else
            ActivateGameMusic();
    }

    public void StoreResultAndSwitchGameToMainMenus()
    {
        PlayerData newCharacterData = new PlayerData(_nameCurrentPlayer, _characterDataCtrl.SummaryDistance, _characterDataCtrl.SummaryScores);
        if (GameMainManagerLinked)
        {//It's end Game and Scene linked to GameMainManager
            MainManager.Instance.FromGameToMenus();
            MainManager.Instance.AddNewCharacterData(newCharacterData);
        }
        else
        {
            Debug.Log(newCharacterData);
            //_activateButtonLocalRestart = true;
            ActivateButtonLocalRestart(activate: true);
            ActivateGameMusic(activate: false);
        }
        //It's common part of the EndGame  for Scenes are linked or  NOT linked to GameMainManager
        ClearSceneAfterEndGame();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (_activateButtonLocalRestart)
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.red;
            if (GUI.Button(new Rect(800, 5, 100, 30), "Restart", style))
            {
                Debug.LogError("Restart");
                StartNewGame(_nameCurrentPlayer);
            } 
        }
    }
#endif
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void ActivateButtonLocalRestart(bool activate) => _activateButtonLocalRestart = activate;
}
