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

public class MyCharacterController : MonoBehaviour, MyControls.IMoveActions
{
    #region SerializedFields
    [SerializeField] private ParticleSystem explosionPrefab;
    [SerializeField] private GameObject dirtSplatter;
    [SerializeField] private int BackwardForceAtCollision = 2;
    [SerializeField] private GameSceneManager gameSceneManager;
    #endregion

    #region NonSerializedFields
    private PlayerCollisionGround checkPlayer;
    private bool Jump = false;
    //The current selected the difficulty level 
    private ForceJumpSO forceJumpSO;
    private MovingWorldSO movingWorld;
    private MyControls inputs;
    private Animator animatorCharacter;
    private Rigidbody rigidbodyCharacter;
    private int hashStatic_b;
    private int hashSpeed_f;
    private int hashJump_trig;
    private int hashDeath_b;
    private int hashDeathType_int;
    private int AnimatorState_Dead_01;
    private int AnimatorLayerIndex_Death;
    /// <summary>
    /// The strength of the jump depends on the selected difficulty level and movement speed 
    /// </summary>
    private float currentForceJump;
    /// <summary>
    /// React on First Collision only
    /// </summary>
    private bool isWasCollision = false;
    /// <summary>
    /// Link to Text in Canvas
    /// </summary>
    private TurnOffPressEnter textTurnOffPressEnter;
    private PlayerState _currentState;
    private bool keyChangeSpeedPressed = false;
    private const float changeSoundMovementDelay = .25f;
    private bool restoreSoundAndEffectAfterGrounded = false;
    private PlaySetAudio gameSound;
    #endregion
    public bool IsCharacterDied { get; private set; } = false;

    private void Awake()
    {
        if (!gameSceneManager)
            Debug.LogError($"{this} not linked to GameSceneManager");
        
        checkPlayer = FindObjectOfType<PlayerCollisionGround>();
        if (checkPlayer== null)
            Debug.LogError($"{this} absent the <PlayerCollisionGround> module");

        inputs = new MyControls();
        inputs.Move.SetCallbacks(this);

        animatorCharacter = GetComponent<Animator>();
        hashStatic_b = Animator.StringToHash("Static_b");
        hashSpeed_f = Animator.StringToHash("Speed_f");
        hashJump_trig = Animator.StringToHash("Jump_trig");
        hashDeath_b = Animator.StringToHash("Death_b");
        hashDeathType_int = Animator.StringToHash("DeathType_int");
        
        AnimatorState_Dead_01 = Animator.StringToHash("Dead_01");
        AnimatorLayerIndex_Death = animatorCharacter.GetLayerIndex("Death");

        rigidbodyCharacter = GetComponent<Rigidbody>();

        movingWorld = SingletonController.Instance.GetMovingWorld();

        gameSound = GetComponent<PlaySetAudio>();
    }

    private void OnEnable() => inputs.Move.Enable();
    private void OnDisable() => inputs.Move.Disable();
    
    private void Start()
    {
        //For Demo Purpose Call only in Editor
        SetPhysicsIgnoreObstaclesCollisions();

        CharacterIdle();
#if UNITY_EDITOR
        //For Demo Purpose Call only in Editor
        CheckIsWalkingAfterStart();
#else
        ShowStartScreen();
#endif
    }

    private void ShowStartScreen()
    {
        //If exist the the object with sciprt <TurnOffPressEnter> (initial text message "PressEnter"), activate it
        //Later can turn off by OnStart
        textTurnOffPressEnter = FindObjectOfType<TurnOffPressEnter>();
        textTurnOffPressEnter?.Active(true);
    }

    private void Update()
    {
        if (isWasCollision)
        {
            //skip other Game Logic
            //Debug.Log("I'm here");
            if (IsCharacterDied)
                CharacterDiedIteraction();
            if (IsCurrentAnimatorState(AnimatorState_Dead_01))
            {
                //Character died
                IsCharacterDied = true;
                gameSceneManager.ManDied();
            }
            return;
        }
        if (checkPlayer.IsGrounded)
        {
            if (restoreSoundAndEffectAfterGrounded)
            {
                gameSound.PlaySound(_currentState, changeSoundMovementDelay);
                dirtSplatter.SetActive( (_currentState == PlayerState.Run)? true : false);
                restoreSoundAndEffectAfterGrounded = false;
            }

            if (keyChangeSpeedPressed)
            {
                ChangeMoveSpeed();
            }

            if (Jump)
            {
                animatorCharacter.SetTrigger(hashJump_trig);
                rigidbodyCharacter.AddForce(Vector3.up * currentForceJump, ForceMode.Impulse);
                gameSound.PlaySound(PlayerState.Jump);
                dirtSplatter.SetActive(false);
                restoreSoundAndEffectAfterGrounded = true;
                Jump = false;
            }
        }
    }

    private void CharacterDiedIteraction()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Check the current Animator state
    /// </summary>
    /// <param name="shortNameHashAnimatorState">Animator state hash</param>
    /// <returns>true if equal to shortNameHashAnimatorState</returns>
    private bool IsCurrentAnimatorState(int shortNameHashAnimatorState)
    {
        //if (clips.Length != 0)
        //    Debug.Log($"num={clips.Length} ClipInfo[0].name = {animatorCharacter.GetCurrentAnimatorClipInfo(AnimatorLayerIndex_Death)[0].clip.name}");
        //Debug.Log($"GetCurrentAnimatorStateInfo = {animatorCharacter.GetCurrentAnimatorStateInfo(AnimatorLayerIndex_Death).shortNameHash}");
        return animatorCharacter.GetCurrentAnimatorStateInfo(AnimatorLayerIndex_Death).shortNameHash == shortNameHashAnimatorState;
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
    /// Can be called from Start() also: called before any Update()
    /// Set Animation, WorldSpeed and State for Character Stop. 
    /// </summary>
    private void CharacterIdle()
    {
        WorldStop();
        //animatorCharacter.SetBool(hashStatic_b, true);
        animatorCharacter.SetFloat(hashSpeed_f, 0);
    }

    private void WorldStop()
    {
        currentForceJump = 0;
        UpdateWorldMoveSpeed(PlayerState.Stop);
    }

    /// <summary>
    /// Reaction on press Go.React through ChangeMoveState()
    /// </summary>
    private void CharacterGo()
    {
        _currentState = PlayerState.Walk;
        keyChangeSpeedPressed = true;
    }
    /// <summary>
    /// Called from Obstacle after detected the Collision with Player
    /// </summary>
    /// <param name="contactPosition"></param>
    /// <param name="contactNormal"></param>
    public void ObstacleCollision(Vector3 contactPosition, Vector3 contactNormal)
    {
        if (!isWasCollision)
        {
            dirtSplatter.SetActive(false);
            movingWorld.SetWorldMovementSpeed(PlayerState.Stop);
            WorldStop();

            if (gameSceneManager.TryDecreaseLifes())
            {
                //Character left lives and continue run 
            }
            else
            {
                //Character throw Foce
                rigidbodyCharacter.AddForce(-contactNormal * BackwardForceAtCollision, ForceMode.VelocityChange);
                //Explosion paritcle
                ParticleSystem particle = Instantiate<ParticleSystem>(explosionPrefab, contactPosition, Quaternion.identity);
                particle.Play();
                gameSound.PlaySound(PlayerState.Died);
                
                animatorCharacter.SetBool(hashDeath_b, true);
                animatorCharacter.SetInteger(hashDeathType_int, 1);
                isWasCollision = true;
                //Character starts to die 
            }

        }
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
            Jump = true;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        //The button Run/Walk will affect if current state != Stop
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

    public void OnStart(InputAction.CallbackContext _)
    {
        //The button Start will affect if current state = Stop
        if (_currentState == PlayerState.Stop)
        {
            //if (!textTurnOffPressEnter)
            //{
            //    Debug.LogError("Time Workaround !!!");
            //    textTurnOffPressEnter = FindObjectOfType<TurnOffPressEnter>();
            //    textTurnOffPressEnter?.Active(false);
            //}
            textTurnOffPressEnter?.Active(false);
            CharacterGo();
        }
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

    private void CheckIsWalkingAfterStart()
    {
        //If IsWalking initialy not set to true For Demo Purpose
        if (!SingletonController.Instance.IsWalkingAfterStart)
        {
            ShowStartScreen();
        }
        else
        {
            //in other case will be emulated the press Start button
            CharacterGo();
        }
    }
#endif

    /// <summary>
    /// Temporary set the Ignore Collision between Player and Obstacles. Editor only for Demo purpose
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void SetPhysicsIgnoreObstaclesCollisions()
    {
        if (SingletonController.Instance.IsPlayerNotCollide)
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
