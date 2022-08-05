using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Stop,
    Walk,
    Run,
    Jump,
    Died,
    Collision
}

public class CharacterManager : MonoBehaviour, MyControls.IMoveActions
{
    #region SerializedFields
    [SerializeField] private ParticleSystem explosionDied;
    [SerializeField] private ParticleSystem explosionNotDied;
    [SerializeField] private GameObject dirtSplatter;
    [SerializeField] private int ReactForceAtCollision = 2;
    [SerializeField] private GameSceneManager gameSceneManager;
    #endregion

    #region NonSerializedFields
    private PlayerCollisionGround checkPlayer;
    private bool WasPressedJump;
    //The current selected the difficulty level 
    private ForceJumpSO forceJumpSO;
    private MovingWorldSO movingWorld;
    private CharacterDataController characterDataCtrl;
    private MyControls inputs;
    private Animator animatorCharacter;
    private Rigidbody rigidbodyCharacter;
    private int hashStatic_b;
    private int hashSpeed_f;
    private int hashJump_trig;
    private int hashDeath_b;
    private int hashDeathType_int;
    private int hashCollision_t;
    private int hashBody_Vertical_f;
    private int hashBody_Horizontal_f;
    private int hashWeaponType_int;
    private int AnimatorState_Dead_01;
    private int AnimatorState_Alive;
    private int AnimatorLayerIndex_Death;
    /// <summary>
    /// The strength of the jump depends on the selected difficulty level and movement speed 
    /// </summary>
    private float currentForceJump;
    /// <summary>
    /// React on First Collision only
    /// </summary>
    private bool isWasCollision;
    /// <summary>
    /// Link to Text in Canvas
    /// </summary>
    private TurnOffPressEnter textTurnOffPressEnter;
    private PlayerState _currentState;
    private bool _inAir;
    private bool keyChangeSpeedPressed;
    private const float changeSoundMovementDelay = .25f;
    private const float DragObstacleAfterLightCollision = 0.5f;
    private PlaySetAudio gameSound;
    private Rigidbody lastCollised;
    private float storeDraglastCollised;
    //private bool isGameEndState;
    private Vector3 characterInitWordPos;
    public TypeWaitMsg WaitState { get; private set; }
    #endregion
    public bool DiedAnimManFinished { get; private set; }
    public bool CollisionAnimManFinished { get; private set; }
    public float GetCharacterInitWordPosX() => characterInitWordPos.x;

    private void Awake()
    {
        if (!gameSceneManager)
            Debug.LogError($"{this} not linked to GameSceneManager");
        checkPlayer = SingletonGame.Instance.GetPlayerCollisionGround();
        if (checkPlayer == null)
            Debug.LogError($"{this} absent the <PlayerCollisionGround> module");

        characterDataCtrl = SingletonGame.Instance.GetCharacterDataCtrl();
        inputs = new MyControls();
        inputs.Move.SetCallbacks(this);

        animatorCharacter = GetComponent<Animator>();
        
        hashStatic_b = Animator.StringToHash("Static_b");
        hashSpeed_f = Animator.StringToHash("Speed_f");
        hashJump_trig = Animator.StringToHash("Jump_trig");
        hashDeath_b = Animator.StringToHash("Death_b");
        hashDeathType_int = Animator.StringToHash("DeathType_int");
        hashCollision_t = Animator.StringToHash("Collision_t");
        hashBody_Vertical_f = Animator.StringToHash("Body_Vertical_f");
        hashBody_Horizontal_f = Animator.StringToHash("Body_Horizontal_f");
        hashWeaponType_int = Animator.StringToHash("WeaponType_int");
        AnimatorState_Dead_01 = Animator.StringToHash("Dead_01");
        AnimatorState_Alive = Animator.StringToHash("Alive");
        AnimatorLayerIndex_Death = animatorCharacter.GetLayerIndex("Death");
        
        rigidbodyCharacter = GetComponent<Rigidbody>();
        
        movingWorld = SingletonGame.Instance.GetMovingWorld();

        //gameSound = GetComponent<PlaySetAudio>();
        gameSound = FindObjectOfType<PlaySetAudio>();

        characterInitWordPos = transform.position;
        textTurnOffPressEnter = FindObjectOfType<TurnOffPressEnter>();
    }

    private void OnEnable()
    {
        inputs.Move.Enable();
        //isGameEndState = true;
        WaitState = TypeWaitMsg.waitEndGame;
    }
    private void OnDisable() => inputs.Move.Disable();
    
    public void Start()
    {
        if (!gameSceneManager.GameMainManagerLinked)
            StartNewAttemptGame();
    }
    /// <summary>
    /// Start New Game or New Attempt
    /// </summary>
    public void StartNewAttemptGame()
    {
        WasPressedJump = false;
        _currentState = PlayerState.Stop;
        isWasCollision = false;
        keyChangeSpeedPressed = false;
        DiedAnimManFinished = false;
        CollisionAnimManFinished = false;
        _inAir = false;
        //if (isGameEndState)
        if (WaitState == TypeWaitMsg.waitEndGame)
        {
            WaitState = TypeWaitMsg.waitStart;
            //isGameEndState = false; 
        }
        //For Demo Purpose Call only in Editor
        SetPhysicsIgnoreObstaclesCollisions();
        CharacterMoveToInitPosition();
        CharacterIdle();
#if UNITY_EDITOR
        //For Demo Purpose Call only in Editor
        //If IsWalking initialy set to true For Demo Purpose
        if (SingletonGame.Instance.IsWalkingAfterStart)
            CharacterGo();
        else
            ShowWaitScreen();
#else
        ShowWaitScreen();
#endif
    }

    /// <summary>
    /// It's Start/Restart or end Game after user press Enter after wait State
    /// </summary>
    private void WasPressedEnter()
    {
        //CountFrame.DebugLog(this, "WasPressedEnter()");
        //The button Start will affect if current state = Stop
        textTurnOffPressEnter.Active(false);
        if (WaitState != TypeWaitMsg.waitEndGame)
        {//It's Start or Restart Game
            CharacterGo();
            if (WaitState == TypeWaitMsg.waitStart)
            {
                gameSceneManager.SwitchMusicCollection(CollectionName.Walking, false);
                gameSceneManager.SwitchMusicToGameScene();
            }
        }
        else
        {
            gameSceneManager.StoreResultAndSwitchGameToMainMenus(); 
        }
    }

    private void CharacterMoveToInitPosition()
    {
        animatorCharacter.enabled = false;
        transform.position = characterInitWordPos;
        animatorCharacter.enabled = true;
    }

    private void ShowWaitScreen()
    {
        //If exist the the object with sciprt <TurnOffPressEnter> (initial text message "PressEnter"), activate it
        //Later can turn off by OnStart
        
        textTurnOffPressEnter.Active(true, WaitState);
    }

    private void Update()
    {
        //if (isGameEndState)
        if (WaitState == TypeWaitMsg.waitEndGame)
        {
            //The game is in a final state, waiting for a username to be entered and Enter to exit using the WasPressedEnter() script
            //Debug.LogWarning($"I'm Here Update() isGameEndState={isGameEndState}");
            return;
        }
        if (isWasCollision)
        {
            //skip other Game Logic and waiting the finishing Animation Collision or Dying
             if (CollisionAnimManFinished)
            {
                RestoreInitialParametersLastCollisedRigidbody();
                gameSceneManager.CharacterCollision();
                WaitState = TypeWaitMsg.waitContinue;
                StartNewAttemptGame();
            }
            else if (DiedAnimManFinished)
            {
                gameSceneManager.CharacterDied();
                WaitState = TypeWaitMsg.waitEndGame;
                ShowWaitScreen();
                //isGameEndState = true;
            }
            return;
        }
        if (checkPlayer.IsGrounded)
        {
            if (WasPressedJump)
            {
                animatorCharacter.SetTrigger(hashJump_trig);
                rigidbodyCharacter.AddForce(Vector3.up * currentForceJump, ForceMode.Impulse);
                gameSound.PlaySound(PlayerState.Jump);
                dirtSplatter.SetActive(false);
                _inAir = true;
                WasPressedJump = false;
                return;
            }
            if (_inAir)
            {
                _inAir = false;
                if (!keyChangeSpeedPressed)
                {
                    gameSound.PlaySound(_currentState, changeSoundMovementDelay);
                    dirtSplatter.SetActive((_currentState == PlayerState.Run) ? true : false);
                }
            }
            if (keyChangeSpeedPressed)
            {
                ChangeMoveSpeed();
            }
        }
    }
    /// <summary>
    /// Check the current Animator state in the Layer
    /// </summary>
    /// <param name="layerindex"></param>
    /// <param name="shortNameHashAnimatorState">Animator state hash</param>
    /// <returns>true if equal to shortNameHashAnimatorState</returns>
    private bool IsCurrentAnimatorState(int layerindex, int shortNameHashAnimatorState)
    {
        //if (clips.Length != 0)
        //    Debug.Log($"num={clips.Length} ClipInfo[0].name = {animatorCharacter.GetCurrentAnimatorClipInfo(AnimatorLayerIndex_Death)[0].clip.name}");
        //Debug.Log($"GetCurrentAnimatorStateInfo = {animatorCharacter.GetCurrentAnimatorStateInfo(AnimatorLayerIndex_Death).shortNameHash}");
        return animatorCharacter.GetCurrentAnimatorStateInfo(layerindex).shortNameHash == shortNameHashAnimatorState;
    }


    /// <summary>
    /// Change the Player animation and WorldMoveSpeed, if Player press the coresponding button.
    /// Set keyChangeSpeedPressed = false
    /// </summary>
    private void ChangeMoveSpeed()
    {
        switch (_currentState)
        {
            case PlayerState.Stop:
                CharacterIdle();
                break;
            case PlayerState.Walk:
                //animatorCharacter.SetBool(hashStatic_b, false);
                animatorCharacter.SetBool(hashStatic_b, true);
                animatorCharacter.SetFloat(hashSpeed_f, 0.26f);
                UpdateWorldMoveSpeed(PlayerState.Walk);
                currentForceJump = forceJumpSO.ForceJumpWalk;
                gameSound.PlaySound(PlayerState.Walk, changeSoundMovementDelay);
                dirtSplatter.SetActive(false);
                break;
            case PlayerState.Run:
                //animatorCharacter.SetBool(hashStatic_b, false);
                animatorCharacter.SetBool(hashStatic_b, true);
                animatorCharacter.SetFloat(hashSpeed_f, 0.51f);
                UpdateWorldMoveSpeed(PlayerState.Run);
                currentForceJump = forceJumpSO.ForceJumpRun;
                gameSound.PlaySound(PlayerState.Run, changeSoundMovementDelay);
                dirtSplatter.SetActive(true);
                break;
            default:
                Debug.LogError("SetMoveState wrong state");
                break;
        }
        keyChangeSpeedPressed = false;
    }

    public void SetForceJumpSO(ForceJumpSO newForceJumpSO) => forceJumpSO = newForceJumpSO;

    /// <summary>
    /// Set Animation, WorldSpeed and State for Character Idle (not Walk or Run Anim) 
    /// </summary>
    private void CharacterIdle()
    {
        dirtSplatter.SetActive(false);
        animatorCharacter.SetFloat(hashSpeed_f, 0);
        //WorldStop();
        currentForceJump = 0;
        UpdateWorldMoveSpeed(PlayerState.Stop);
    }

    //private void WorldStop()
    //{
    //    currentForceJump = 0;
    //    UpdateWorldMoveSpeed(PlayerState.Stop);
    //}

    /// <summary>
    /// Reaction on press Go.React through ChangeMoveState()
    /// </summary>
    private void CharacterGo()
    {
        _currentState = PlayerState.Walk;
        keyChangeSpeedPressed = true;
    }
    /// <summary>
    /// Called from Obstacle OnCollisionEnter() after detected the Collision with Player
    /// </summary>
    /// <param name="contactPosition"></param>
    /// <param name="contactNormal"></param>
    public void ObstacleCollision(Vector3 contactPosition, Vector3 contactNormal, Rigidbody rigidbodyObstacle)
    {
        if (!isWasCollision)
        {
            //Set Anim to Idle at Layer Walk the Dying and CollisionAndGym Anim on Layers Died and Body
            CharacterIdle();
            characterDataCtrl.ScoreAfterCollisionOccur(rigidbodyObstacle.GetInstanceID());
            if (gameSceneManager.CharacterNotDiedAfterCollision())
            {
                //Character left lives and continue run
                //obstacle after not killing collision receive an impulse and must have a drag
                TempChangeDragLastCollisedRigidbody(rigidbodyObstacle);
                rigidbodyObstacle.AddForce(contactNormal * ReactForceAtCollision, ForceMode.Impulse);
                ParticleSystem particle = Instantiate<ParticleSystem>(explosionNotDied, contactPosition, Quaternion.identity);
                particle.Play();
                gameSound.PlaySound(PlayerState.Collision);
                //Start Corunite before Transition starts and check when it finished
                StartAndCheckFinishCollisionAnim();
            }
            else
            {
                gameSceneManager.SwitchMusicCollection(CollectionName.Died);
                //Character throw Force
                rigidbodyCharacter.AddForce(-contactNormal * ReactForceAtCollision, ForceMode.Impulse);
                //Explosion paritcle
                ParticleSystem particle = Instantiate<ParticleSystem>(explosionDied, contactPosition, Quaternion.identity);
                particle.Play();
                gameSound.PlaySound(PlayerState.Died);
                //Start Animation and check when it finished
                StartCoroutine(StartAndCheckFinishDiedAnim());
            }
            isWasCollision = true;
        }
    }

    private void TempChangeDragLastCollisedRigidbody(Rigidbody rigidbodyObstacle)
    {
        lastCollised = rigidbodyObstacle;
        storeDraglastCollised = rigidbodyObstacle.drag;
        rigidbodyObstacle.drag = DragObstacleAfterLightCollision;
    }
    /// <summary>
    /// After collision the rigidbody of Obstacle must receive initial drag = 0 and restore the initial rotation before next its time of spawning
    /// </summary>
    private void RestoreInitialParametersLastCollisedRigidbody()
    {
        lastCollised.drag = storeDraglastCollised;
        lastCollised.transform.localRotation = Quaternion.identity;
    }


    /// <summary>
    /// Start DiedAnim after finish set DiedAnimManFinished = true
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator StartAndCheckFinishDiedAnim()
    {
        animatorCharacter.SetBool(hashDeath_b, true);
        animatorCharacter.SetInteger(hashDeathType_int, 1);

        do { yield return null; } while (!IsCurrentAnimatorState(AnimatorLayerIndex_Death, AnimatorState_Dead_01));
        DiedAnimManFinished = true;
    }

    /// <summary>
    /// Start CollisionAnim after finish set CollisionAnimManFinished = true
    /// </summary>
    /// <param name="showMakeGymnasticAnim"></param>
    private void StartAndCheckFinishCollisionAnim(bool showMakeGymnasticAnim = true)
    {
        //Coroutine for guarantee must run before SetTriger is initiated and Transition starts, so as not to miss the exit from the current state
        StartCoroutine(CheckFinishCollisionAnim(showMakeGymnasticAnim));
        //Initiate the Tsansition
        animatorCharacter.SetTrigger(hashCollision_t);
    }

    private IEnumerator CheckFinishCollisionAnim(bool showMakeGymnasticAnim)
    {
        //The coroutine runs before the animator exits the AnimatorState_Alive state.
        while (IsCurrentAnimatorState(AnimatorLayerIndex_Death, AnimatorState_Alive)) { yield return null; }

        //Pause till finishing the hashCollision_t animation
        do { yield return null; } while (!IsCurrentAnimatorState(AnimatorLayerIndex_Death, AnimatorState_Alive));
        if (showMakeGymnasticAnim)
            StartCoroutine(CharacterMakeGyme());
        else
            CollisionAnimManFinished = true;
    }

    private IEnumerator CharacterMakeGyme()
    {
        int part = 20;
        float deltaAnimationStep = 1f / part;
        float pauseIteration = 1f / part / 1.4f;
        //To Acitivate Layer Body must not used on Layer Weapon
        animatorCharacter.SetInteger(hashWeaponType_int, 20);
        for (int i = 0; i <= part; i++)
        {
            animatorCharacter.SetFloat(hashBody_Vertical_f, i * deltaAnimationStep);
            yield return new WaitForSeconds(pauseIteration);
        }
        pauseIteration = 1f / part / 1.6f;
        for (int i = part; i >= -part; i--)
        {
            animatorCharacter.SetFloat(hashBody_Vertical_f, i * deltaAnimationStep);
            yield return new WaitForSeconds(pauseIteration);
        }
        pauseIteration = 1f / part / 1.8f;
        for (int i = -part; i <= 0; i++)
        {
            animatorCharacter.SetFloat(hashBody_Vertical_f, i * deltaAnimationStep);
            yield return new WaitForSeconds(pauseIteration);
        }
        CollisionAnimManFinished = true;
    }

    /// <summary>
    /// Update the currentState and WorlMovementSpeed
    /// </summary>
    /// <param name="moveType"></param>
    private void UpdateWorldMoveSpeed(PlayerState moveType)
    {
        _currentState = moveType;
        movingWorld.SetWorldMovementSpeed(moveType);
    }

    #region Mapping for Action Map
    public void OnJump(InputAction.CallbackContext context)
    {
        if (checkPlayer.IsGrounded && context.phase == InputActionPhase.Started)
        {
            WasPressedJump = true;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        //The button Run/Walk will affect if current state != Stop
        //if (_currentState != PlayerState.Stop && _currentState != PlayerState.Jump)
        if (_currentState != PlayerState.Stop)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                case InputActionPhase.Performed:
                    _currentState = PlayerState.Run;
                    break;
                case InputActionPhase.Canceled:
                    _currentState = PlayerState.Walk;
                    break;
            }
            keyChangeSpeedPressed = true;
        }
    }

    public void OnStart(InputAction.CallbackContext context)
    {
        if (_currentState == PlayerState.Stop && context.phase == InputActionPhase.Started)
            WasPressedEnter();
    }
    #endregion

    #region Call only in UNITY_EDITOR
#if UNITY_EDITOR
    //Show Gizmo when Player not IsGrounded in Editor only
    private void OnDrawGizmosSelected()
    {
        if (checkPlayer != null && !checkPlayer.IsGrounded)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
    }
#endif

    /// <summary>
    /// Temporary set the Ignore Collision between Player and Obstacles. Editor only for Demo purpose
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void SetPhysicsIgnoreObstaclesCollisions()
    {
        if (SingletonGame.Instance.IsPlayerNotCollide)
        {
            int layPlayer = LayerMask.NameToLayer("Player");
            int layObstacle = LayerMask.NameToLayer("Obstacles");
            if (layPlayer < 0 || layObstacle < 0)
            {
                Debug.LogWarning("SetPhysicsIgnoreObstaclesCollisions() : Can't set for Physics to ignore collisions between [Player] and [Obstacles]");
            }
            else
            {
                Physics.IgnoreLayerCollision(layPlayer, layObstacle);
                Debug.Log("Was set for Physics -  Ignore collisions between [Player] and [Obstacles]");
            }
        }
    } 
    #endregion
}
