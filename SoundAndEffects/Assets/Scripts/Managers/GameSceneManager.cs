using UnityEngine;
using UnityEngine.EventSystems;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject gameCamera;
    [Header("For Testing purpose only, used in Editor only")]
    [SerializeField] private GameObject buttonRestart;

    private MainManager _gameMainManager;
    private CharacterDataController _characterDataCtrl;
    private MainSpawner _mainSpawner;
    private GraveStoneControl _graveStoneControl;
    private GameParametersManager _gameParametersManager;
    private GameObject _characterObject;
    private CharacterManager _characterManager;
    private PlayJukeBoxCollection _playJukeBoxGameCollection;

    private const int DecreaseAmmountLife = -1;
    private string _nameCurrentPlayer;
    public bool GameMainManagerLinked { get; private set; }

    private void Awake()
    {
        CountFrame.DebugLogUpdate(this, $"Awake()");
        _characterDataCtrl = SingletonGame.Instance.GetCharacterDataCtrl();
        _mainSpawner = SingletonGame.Instance.GetMainSpawner();
        _graveStoneControl = SingletonGame.Instance.GetGraveStoneControl();
        _gameParametersManager = SingletonGame.Instance.GetGameParametersManager();
        _gameMainManager = MainManager.Instance;
        _characterManager = SingletonGame.Instance.GetCharacterManager();
        _characterObject = _characterManager.gameObject;
        _playJukeBoxGameCollection = SingletonGame.Instance.GetPlayJukeBox();

        if (_gameMainManager)
        {
            GameMainManagerLinked = true;
            _gameMainManager.LinkGameSceneManager(this);
            //Camera will manage by GameMainManager
            ActivateGameCamera(false);
            //Initially the GameController must be off
            _characterObject.SetActive(false);
        }
        else
        {
            Debug.LogError($"{this} not linked to GameMainManager");
            GameMainManagerLinked = false;
            _characterObject.SetActive(true);
            ActivateGameCamera(true);
            //TurnOnMusic();
            Debug.LogWarning($"Music at Awake not TurnOn because in build will be used Music from Menu Scene before game was started");
        }
    }

    /// <summary>
    /// Move EventSystem focus from any buttons to GameObject on current scene, to give possibility to catch the Enter press vs repeat "Button click"
    /// </summary>
    public void SelectGameObjectFromScene(GameObject gameObject)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void StartNewGame(string playerName, int _overrideCharacterHealth = 0)
    {
        _nameCurrentPlayer = playerName;
        ActivateButtonLocalRestart(false);
        _gameParametersManager.ReInitParameters();
        //The Character GameObject will be Turn off at EndGame to reinit the Character Animator
        _characterObject.SetActive(true);
        SelectGameObjectFromScene(_characterObject);
        _characterDataCtrl.ResetHealth(_overrideCharacterHealth);
        //WaitState before restart Game
        if (_characterManager.CurrentWaitType == WaitType.waitEndGame)
            _characterDataCtrl.ResetScoreDistance();
        _mainSpawner.ReStartSpawner();
        _characterManager.StartNewAttemptGame();
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
            ActivateButtonLocalRestart(true); 
        }
        //It's common part of the EndGame  for Scenes are linked or  NOT linked to GameMainManager
        ClearSceneAfterEndGame();
    }

    public void ActivateButtonLocalRestart(bool activate)
    {
        buttonRestart.SetActive(activate);
    }
}
