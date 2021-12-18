using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    enum PlayerMoveState
    {
        Idle,
        Running,
        Falling,
        Jumping,
    }

    //Movement
    [Header("Movement")]
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float jumpSpeed;
    [SerializeField]
    private float jumpSteps;
    [SerializeField]
    private float jumpCoolDown;
    [SerializeField]
    private int jumpHelpFrames;
    [SerializeField]
    private float jumpStepsMin;
    [SerializeField]
    private LayerMask groundLayer;

    public void MoveInput(CallbackContext callbackContext)
    {
        if (!listenForInput)
        {
            _moveInput = 0;
            return;
        }
        _moveInput = callbackContext.ReadValue<float>();
        if (_moveInput > 0.01f)
        {
            _moveInput = 1;
        }
        else if (_moveInput < -0.01f)
        {
            _moveInput = -1;
        }
        else
        {
            _moveInput = 0;
        }
    }

    public void JumpInput(CallbackContext callbackContext)
    {
        if (!listenForInput)
        {
            wantToJump = false;
            return;
        }
        if (callbackContext.performed)
        {
            wantToJump = true;
            jumpHelpFramesLeft = jumpHelpFrames;
        }
    }
    //public void AimInput(CallbackContext callbackContext)
    //{
    //    if (!listenForInput)
    //    {
    //        aimInput = Vector2.right * facingDir;
    //        return;
    //    }
    //    aimInput = callbackContext.ReadValue<Vector2>();
    //    aimInput.Normalize();
    //}
    public void ResetDirection()
    {
        FaceRight();
    }
    public void SetControl(bool canControl)
    {
        listenForInput = canControl;
    }


    private static PlayerController instance;

    private Rigidbody2D rb;
    private new Transform transform;
    private Animator anim;
    private bool listenForInput;
    private float _moveInput;
    private float moveInput
    {
        get
        {
            return _moveInput;
        }
        set
        {
            _moveInput = value;
        }
    }
    private bool isGrounded;
    private PlayerMoveState moveState;
    private bool wantToJump;
    private int currentJumpSteps;
    private float jumpCoolDownTimer;
    private int facingDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        transform = base.transform;
        rb.velocity = Vector2.zero;
        listenForInput = true;
        isGrounded = false;
        moveState = PlayerMoveState.Idle;
        jumpCoolDownTimer = 0;
        FaceRight();
    }

    private bool isDead = false;
    private void FixedUpdate()
    {
        CheckGrounded();
        if (isDead) { }
        else
        {
            rb.gravityScale = 1;
            switch (moveState)
            {
                case PlayerMoveState.Idle:
                    IdleState();
                    break;
                case PlayerMoveState.Running:
                    RunningState();
                    break;
                case PlayerMoveState.Falling:
                    FallingState();
                    break;
                case PlayerMoveState.Jumping:
                    JumpingState();
                    break;
                default:
                    break;
            }
        }
        ResetInput();
        UpdateTimers();
    }

    private void Die()
    {
        isDead = true;
        StopJumping();
        SetControl(false);
        anim.SetBool("Hit", false);
        anim.SetTrigger("Die");
        rb.gravityScale = 2;
        //rb.isKinematic = true;
        //rb.gravityScale = 0;
        //GetComponent<Collider2D>().enabled = false;
    }


    private void ResetInput()
    {

        if (wantToJump && jumpHelpFramesLeft == 0)
            wantToJump = false;
    }
    private void UpdateTimers()
    {
        if (jumpCoolDownTimer > 0)
            jumpCoolDownTimer -= Time.fixedDeltaTime;
        if (jumpHelpFramesLeft > 0)
            jumpHelpFramesLeft--;
    }


    private void IdleState()
    {
        if (CheckFalling() || TryJump()) return;
        Vector2 vel = rb.velocity;
        vel.x = moveInput * runSpeed;
        if (vel.x > 0.001f || vel.x < -0.001f)
        {
            StartRunning();
        }
    }


    private bool CheckFalling()
    {
        if (!isGrounded && rb.velocity.y < 0.0001f)
        {
            StartFalling();
            return true;
        }
        return false;
    }
    private bool TryJump()
    {
        if (wantToJump && jumpCoolDownTimer <= 0)
        {
            StartJumping();
            return true;
        }
        return false;
    }


    private void StartJumping()
    {
        currentJumpSteps = 0;
        moveState = PlayerMoveState.Jumping;
        //anim.SetBool("Jumping", true);
    }

    private void StartFalling()
    {
        moveState = PlayerMoveState.Falling;
        //anim.SetBool("Falling", true);
    }
    private void StartRunning()
    {
        moveState = PlayerMoveState.Running;
        //anim.SetBool("Running", true);
    }

    private void RunningState()
    {
        if (CheckFalling() || TryJump())
        {
            StopRunning();
        }
        CheckLookDir();
        Vector2 vel = rb.velocity;
        vel.x = moveInput * runSpeed;
        if (vel.x < 0.001f && vel.x > -0.001f)
        {
            moveState = PlayerMoveState.Idle;
            StopRunning();
        }
        //if (attackState != PlayerAttackState.Hit)
        rb.velocity = vel;
    }


    private void CheckLookDir()
    {
        if (moveInput > 0.01f)
        {
            FaceRight();
        }
        else if (moveInput < -0.01f)
        {
            FaceLeft();
        }
    }

    private void StopRunning()
    {
        //anim.SetBool("Running", false);
    }

    private void FallingState()
    {
        rb.gravityScale = 3;
        if (isGrounded)
        {
            StopFalling();
            moveState = PlayerMoveState.Idle;
        }
        CheckLookDir();
        Vector2 vel = rb.velocity;
        vel.x = moveInput * runSpeed;
        rb.velocity = vel;
    }

    private void StopFalling()
    {
        //anim.SetBool("Falling", false);
    }

    private void JumpingState()
    {
        rb.gravityScale = 2;
        CheckLookDir();
        if (currentJumpSteps < jumpSteps)
        {

            Vector2 velocity = rb.velocity;
            velocity.y = jumpSpeed;
            rb.velocity = velocity;
        }
        else
        {
            StopJumping();
            StartFalling();
        }
        currentJumpSteps++;
    }

    private void StopJumping()
    {
        jumpCoolDownTimer = jumpCoolDown;
        //anim.SetBool("Jumping", false);
    }


    private ContactPoint2D[] groundedContactResult = new ContactPoint2D[1];
    private RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[1];
    private int jumpHelpFramesLeft;

    private void CheckGrounded()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.NoFilter();
        contactFilter.useNormalAngle = true;
        contactFilter.minNormalAngle = 85;
        contactFilter.maxNormalAngle = 95;
        int result = rb.GetContacts(contactFilter, groundedContactResult);
        if (result > 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        if (Physics2D.RaycastNonAlloc(transform.position, -transform.up, raycastHit2Ds, 0.2f, groundLayer.value) > 0)
        {
            isGrounded = true;
        }

    }
    private void FaceLeft()
    {
        facingDir = -1;
    }
    private void FaceRight()
    {
        facingDir = 1;
    }
}
