using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyCharacterController : MonoBehaviour, MyControls.IMoveActions
{
    [SerializeField] private bool IsWalkingAfterRUN = false;
    private bool Jump = false;
    [SerializeField] private MoveBackGround moveBackGround;
    [SerializeField] private CollisionGround player;
    [SerializeField] private ParticleSystem explosionPrefab;
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
    private MovementType currentState;
    private bool keyChangeSpeedPressed = false;

    private void Awake()
    {
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

    private void Start()
    {
        CharacterStop();

        //If IsWalking initialy not set to true
        // If exist the the object with initial text message "PressEnter", activate it
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

    private void OnEnable() =>  inputs.Move.Enable();
    private void OnDisable() => inputs.Move.Disable();

    private void Update()
    {
        if (player.IsGrounded)
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
    /// Change the animation and movement speed, if Player press the coresponding button.
    /// Set keyChangeSpeedPressed = false
    /// </summary>
    private void ChangeMoveState()
    {
        switch (currentState)
        {
            case MovementType.Stop:
                CharacterStop();
                break;
            case MovementType.Walk:
                animatorCharacter.SetBool(hashStatic_b, false);
                animatorCharacter.SetFloat(hashSpeed_f, 0.26f);
                UpdateWorldMoveSpeed(MovementType.Walk);
                currentForceJump = forceJumpSO.ForceJumpWalk;
                break;
            case MovementType.Run:
                animatorCharacter.SetBool(hashStatic_b, false);
                animatorCharacter.SetFloat(hashSpeed_f, 0.51f);
                UpdateWorldMoveSpeed(MovementType.Run);
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
    /// Set Animation, WorldSpeed and State for Character Stop. Called from State switch and from Start
    /// </summary>
    private void CharacterStop()
    {
        //IsRun = false;
        currentForceJump = 0;
        animatorCharacter.SetBool(hashStatic_b, true);
        animatorCharacter.SetFloat(hashSpeed_f, 0);
        UpdateWorldMoveSpeed(MovementType.Stop);
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
            movingWorld.SetWorldMovementSpeed(MovementType.Stop);
            rigidbodyCharacter.AddForce(-contactNormal * 5, ForceMode.VelocityChange);

            //Explosion paritcle
            ParticleSystem particle = Instantiate<ParticleSystem>(explosionPrefab,contactPosition,Quaternion.identity);
            particle.Play();

            currentForceJump = 0;
            UpdateWorldMoveSpeed(MovementType.Stop);
            animatorCharacter.SetBool(hashDeath_b, true);
            animatorCharacter.SetInteger(hashDeathType_int, 1);

            IsFirstCollision = false; 
        }
    }
    /// <summary>
    /// Update the currentState and WorlMovementSpeed
    /// </summary>
    /// <param name="moveType"></param>
    private void UpdateWorldMoveSpeed(MovementType moveType)
    {
        currentState = moveType;
        movingWorld.SetWorldMovementSpeed(moveType);
    }

    #region Mapping for Action Map
    public void OnJump(InputAction.CallbackContext context)
    {
        if (player.IsGrounded && context.phase == InputActionPhase.Started)
        {
            Jump = true;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        //The button Run/Walk will affect if current state != Stop
        if (currentState != MovementType.Stop)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                case InputActionPhase.Performed:
                    currentState = MovementType.Run;
                    //IsRun = true;
                    break;
                case InputActionPhase.Canceled:
                    currentState = MovementType.Walk;
                    //IsRun = false;
                    break;
            }
            keyChangeSpeedPressed = true;
        }
    }

    public void OnStart(InputAction.CallbackContext _)
    {
        //The button Start will affect if current state = Stop
        if (currentState == MovementType.Stop)
        {
            //IsWalking = true;
            textTurnOffPressEnter?.Active(false);
            CharacterGo();
        }
    }
    /// <summary>
    /// Reaction on press Go
    /// </summary>
    private void CharacterGo()
    {
        currentState = MovementType.Walk;
        keyChangeSpeedPressed = true;
    }
    #endregion

#if UNITY_EDITOR
    //Show Gizmo when Player not IsGrounded
    [ExecuteAlways]
    private void OnDrawGizmos()
    {
        if (!player.IsGrounded)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
    } 
#endif
}
