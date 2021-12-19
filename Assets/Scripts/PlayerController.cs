using System;
using System.Threading.Tasks;
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
    }

    //Movement
    [Header("Movement")]
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private LayerMask groundLayer;
    //jar
    [Header("Throwing Jar")]
    [SerializeField]
    private float throwJarCooldown = 0.4f;
    [SerializeField]
    private GameObject throwJarPrefab;
    [SerializeField]
    private Transform throwJarPoint;
    [SerializeField]
    private float throwJarForce = 10f;
    [SerializeField, Range(0f, 1f)]
    private float throwJarDownDeadZone = 0.2f;
    [Header("Throwing Teleport Jar")]
    [SerializeField]
    private float teleportJarCooldown = 0.2f;
    [SerializeField]
    private GameObject telportJarPrefab;
    [SerializeField, Range(0f, 1f)]
    private float teleportJarDownDeadZone = 0.2f;
    [SerializeField]
    private float teleportJarCooldownAfterTeleport = 0.2f;

    [Header("Health")]
    [SerializeField]
    private int health = 3;
    [SerializeField]
    private float takeDamageCooldown = 0.3f;


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
    private Vector2 aimInput;
    private Vector3 aimDirection;
    public void AimInput(CallbackContext callbackContext)
    {
        if (!listenForInput)
        {
            aimInput = Vector2.right * facingDir;
            return;
        }
        aimInput = callbackContext.ReadValue<Vector2>();
        Vector3 aimPosition = Camera.main.ScreenToWorldPoint(new Vector3(aimInput.x, aimInput.y, 0));
        aimPosition.z = 0;
        aimDirection = aimPosition - throwJarPoint.position;
        aimDirection.Normalize();
    }
    private bool isAiming = false;
    public async void AimJarInput(CallbackContext callbackContext)
    {
        if (!listenForInput)
        {
            return;
        }
        if (callbackContext.performed)
        {
            if (canThrow && allowActions)
            {
                canThrow = false;
                isAiming = true;
                anim.SetBool("Aiming", true);
                GameObject jar = Instantiate(throwJarPrefab, throwJarPoint.position, Quaternion.identity, transform);
                while (isAiming)
                {
                    await Task.Yield();
                    if (!allowActions)
                    {
                        isAiming = false;
                        anim.SetBool("Aiming", false);
                        DestroyImmediate(jar);
                    }
                    if (_wantToThrow)
                    {
                        if (Mathf.Abs(aimDirection.x) < throwJarDownDeadZone && aimDirection.y < 0)
                        {
                            isAiming = false;
                            anim.SetBool("Aiming", false);
                            DestroyImmediate(jar);
                        }
                        else
                        {
                            anim.SetTrigger("Throw");
                            anim.SetBool("Aiming", false);
                            isAiming = false;
                            jar.GetComponent<Jar>().ActivateJar();
                            jar.transform.SetParent(null);
                            Rigidbody2D jarRb = jar.GetComponent<Rigidbody2D>();
                            jarRb.AddForce(aimDirection * throwJarForce, ForceMode2D.Impulse);

                            float nextThrow = Time.time + throwJarCooldown;
                            canThrow = false;
                            while (Time.time <= nextThrow)
                            {
                                await Task.Yield();
                            }
                        }
                    }
                }
                canThrow = true;
            }
        }
    }
    public async void AimJar2Input(CallbackContext callbackContext)
    {
        if (!listenForInput)
        {
            return;
        }
        if (callbackContext.performed)
        {
            if (canThrowTeleport && teleportJar == null && allowActions)
            {
                canThrowTeleport = false;
                isAiming = true;
                anim.SetBool("Aiming", true);
                GameObject jar = Instantiate(telportJarPrefab, throwJarPoint.position, Quaternion.identity, transform);
                while (isAiming)
                {
                    if (!allowActions)
                    {
                        isAiming = false;
                        anim.SetBool("Aiming", false);
                        DestroyImmediate(jar);
                    }
                    await Task.Yield();
                    if (_wantToThrowTeleport)
                    {
                        if (Mathf.Abs(aimDirection.x) < throwJarDownDeadZone && aimDirection.y < 0)
                        {
                            isAiming = false;
                            anim.SetBool("Aiming", false);
                            DestroyImmediate(jar);
                        }
                        else
                        {
                            anim.SetTrigger("Throw");
                            anim.SetBool("Aiming", false);
                            isAiming = false;
                            jar.GetComponent<Jar>().ActivateJar();
                            jar.transform.SetParent(null);
                            Rigidbody2D jarRb = jar.GetComponent<Rigidbody2D>();
                            jarRb.AddForce(aimDirection * throwJarForce, ForceMode2D.Impulse);
                            teleportJar = jar;
                            float nextThrow = Time.time + teleportJarCooldown;
                            canThrowTeleport = false;
                            while (Time.time <= nextThrow)
                            {
                                await Task.Yield();
                            }
                        }
                    }
                }
                canThrowTeleport = true;
            }
            else if (teleportJar != null)
            {
                Destroy(teleportJar);
            }
        }
    }

    private bool _wantToThrow;
    public void ThrowInput(CallbackContext callbackContext)
    {
        if (!listenForInput)
        {
            _wantToThrow = false;
            return;
        }
        if (callbackContext.performed)
        {
            _wantToThrow = true;
        }
    }
    private bool _wantToThrowTeleport;
    public void ThrowTeleportInput(CallbackContext callbackContext)
    {
        if (!listenForInput)
        {
            _wantToThrowTeleport = false;
            return;
        }
        if (callbackContext.performed)
        {
            _wantToThrowTeleport = true;
        }
    }
    bool _wantToTeleport = false;
    public void TeleportInput(CallbackContext callbackContext)
    {
        if (!listenForInput)
        {
            _wantToTeleport = false;
            return;
        }
        if (callbackContext.performed)
        {
            _wantToTeleport = true;
        }
    }

    public void SetControl(bool canControl)
    {
        listenForInput = canControl;
    }

    private bool allowActions = true;
    public async void Damage(int damageAmount)
    {
        if (health <= 0)
        {
            return;
        }
        health -= damageAmount;
        if (health < 0)
        {
            health = 0;
        }
        anim.SetTrigger("Damage");
        allowActions = false;
        float canMoveTimer = Time.time + takeDamageCooldown;
        while (Time.time <= canMoveTimer)
        {
            await Task.Yield();
        }
        allowActions = true;

        if (health == 0)
        {
            Die();
        }
    }

    private Rigidbody2D rb;
    private new Transform transform;
    private Animator anim;
    private SpriteRenderer sprite;
    private bool listenForInput;
    private bool isGrounded;
    private PlayerMoveState moveState;
    private int facingDir;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        transform = base.transform;
        rb.velocity = Vector2.zero;
        listenForInput = true;
        isGrounded = false;
        moveState = PlayerMoveState.Idle;
        FaceRight();
    }

    private bool isDead = false;
    private void FixedUpdate()
    {
        CheckGrounded();
        if (isDead || !allowActions) { }
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
                default:
                    break;
            }
        }
        TryTeleport();
        ResetInput();
    }


    private bool canThrow = true;
    private bool canThrowTeleport = true;
    private GameObject teleportJar = null;

    private async void TryTeleport()
    {
        if (_wantToTeleport && teleportJar != null)
        {
            transform.position = teleportJar.transform.position;
            rb.velocity = Vector2.zero;
            DestroyImmediate(teleportJar);
            float nextThrow = Time.time + teleportJarCooldownAfterTeleport;
            while (Time.time <= nextThrow)
            {
                canThrowTeleport = false;
                await Task.Yield();
            }
            canThrowTeleport = true;
        }
    }

    private void Die()
    {
        isDead = true;
        SetControl(false);
        anim.SetBool("Dead", true);
        rb.gravityScale = 2;
        //rb.isKinematic = true;
        //rb.gravityScale = 0;
        //GetComponent<Collider2D>().enabled = false;
    }


    private void ResetInput()
    {
        _wantToThrow = false;
        _wantToThrowTeleport = false;
        _wantToTeleport = false;
    }

    private void IdleState()
    {
        CheckLookDir();
        if (CheckFalling()) return;
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

    private void StartFalling()
    {
        moveState = PlayerMoveState.Falling;
        anim.SetBool("Falling", true);
    }
    private void StartRunning()
    {
        moveState = PlayerMoveState.Running;
        anim.SetBool("Running", true);
    }

    private void RunningState()
    {
        if (CheckFalling())
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
        if (isAiming)
        {
            if (aimDirection.x > 0)
            {
                FaceRight();
            }
            else
            {
                FaceLeft();
            }
            return;
        }

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
        anim.SetBool("Running", false);
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
        anim.SetBool("Falling", false);
    }

    private ContactPoint2D[] groundedContactResult = new ContactPoint2D[1];
    private RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[1];

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
        sprite.flipX = true;
    }
    private void FaceRight()
    {
        facingDir = 1;
        sprite.flipX = false;
    }
}
