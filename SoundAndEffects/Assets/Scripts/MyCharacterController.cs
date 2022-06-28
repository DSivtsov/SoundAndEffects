using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerState
{
    Stop,
    Walk,
    Run
}

public class MyCharacterController : MonoBehaviour, MyControls.IMoveActions
{
    //For Demo purpose only
    [SerializeField] private bool IsWalkingAfterRUN = false;
    [SerializeField] private ParticleSystem explosionPrefab;

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
    /// <summary>
    /// The strength of the jump depends on the selected difficulty level and movement speed 
    /// </summary>
    private float currentForceJump;
    /// <summary>
    /// React on First Collision only
    /// </summary>
    private bool IsFirstCollision = true;
    /// <summary>
    /// Link to Text in Canvas
    /// </summary>
    private TurnOffPressEnter textTurnOffPressEnter;
    private PlayerState _currentState;
    private bool keyChangeSpeedPressed = false;

    private void Awake()
    {
        checkPlayer = FindObjectOfType<PlayerCollisionGround>();
        if (checkPlayer== null)
        {
            Debug.LogError("Absent module <PlayerCollisionGround>");
        }

        inputs = new MyControls();
        inputs.Move.SetCallbacks(this);

        animatorCharacter = GetComponent<Animator>();
        hashStatic_b = Animator.StringToHash("Static_b");
        hashSpeed_f = Animator.StringToHash("Speed_f");
        hashJump_trig = Animator.StringToHash("Jump_trig");
        hashDeath_b = Animator.StringToHash("Death_b");
        hashDeathType_int = Animator.StringToHash("DeathType_int");

        rigidbodyCharacter = GetComponent<Rigidbody>();

        movingWorld = SingletonController.Instance.GetMovingWorld();
    }

    private void OnEnable() => inputs.Move.Enable();
    private void OnDisable() => inputs.Move.Disable();
    
    private void Start()
    {
        CharacterIdle();

        //If IsWalking initialy not set to true
        //If exist the the object with sciprt <TurnOffPressEnter> (initial text message "PressEnter"), activate it
        //Later can turn off by OnStart
        if (!IsWalkingAfterRUN)
        {
            textTurnOffPressEnter = FindObjectOfType<TurnOffPressEnter>();
            textTurnOffPressEnter?.Active(true);
        }
        else
        {
            //in other case will be emulated the press Start button
            CharacterGo();
        }
    }

    private void Update()
    {
        if (checkPlayer.IsGrounded)
        {
            if ( keyChangeSpeedPressed )
            {
                ChangeMoveState();
            }

            if (Jump)
            {
                animatorCharacter.SetTrigger(hashJump_trig);
                rigidbodyCharacter.AddForce(Vector3.up * currentForceJump, ForceMode.Impulse);
                Jump = false;
            } 
        }
    }
    /// <summary>
    /// Change the Player animation and WorldMoveSpeed, if Player press the coresponding button.
    /// Set keyChangeSpeedPressed = false
    /// </summary>
    private void ChangeMoveState()
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
                break;
            case PlayerState.Run:
                //animatorCharacter.SetBool(hashStatic_b, false);
                animatorCharacter.SetBool(hashStatic_b, true);
                animatorCharacter.SetFloat(hashSpeed_f, 0.51f);
                UpdateWorldMoveSpeed(PlayerState.Run);
                currentForceJump = forceJumpSO.ForceJumpRun;
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
        if (IsFirstCollision)
        {
            movingWorld.SetWorldMovementSpeed(PlayerState.Stop);
            rigidbodyCharacter.AddForce(-contactNormal * 5, ForceMode.VelocityChange);
            //Explosion paritcle
            ParticleSystem particle = Instantiate<ParticleSystem>(explosionPrefab, contactPosition, Quaternion.identity);
            particle.Play();

            //Set Player State Died
            WorldStop();
            animatorCharacter.SetBool(hashDeath_b, true);
            animatorCharacter.SetInteger(hashDeathType_int, 1);

            IsFirstCollision = false;
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
                    //IsRun = true;
                    break;
                case InputActionPhase.Canceled:
                    _currentState = PlayerState.Walk;
                    //IsRun = false;
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
            //IsWalking = true;
            textTurnOffPressEnter?.Active(false);
            CharacterGo();
        }
    }
    #endregion

    //Show Gizmo when Player not IsGrounded in Editor only
    private void OnDrawGizmosSelected()
    {
        if (checkPlayer!=null && !checkPlayer.IsGrounded)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
    } 
}
