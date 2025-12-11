using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseMoveSpeed = 5f;
    private float currentMoveSpeed;
    public float jumpForce = 7f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;              // đặt dưới chân player
    public float groundCheckRadius = 0.22f;    // điều chỉnh 0.18–0.28 tùy collider
    public LayerMask groundLayer;              // chọn Layer Ground trong Inspector

    [Header("Slow Effect")]
    private bool isSlowed = false;
    private Coroutine slowCoroutine;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float moveInput;
    private bool isGrounded;
    private bool jumpPressed;

    [Header("Fire Point")]
    public Transform firePoint;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashTime = 0.15f;
    public float dashCooldown = 0.6f;
    public KeyCode dashKey = KeyCode.L;

    [Header("Dash Unlock")]
    public bool canDash = false;

    private bool isDashing = false;
    private float lastDashTime;
    private float originalGravity;
    private bool isStunned = false;

    [Header("Sound Effects")]
    public AudioClip dashSound;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        originalGravity = rb.gravityScale;
        currentMoveSpeed = baseMoveSpeed;

        // Khuyến nghị: đặt Collision Detection = Continuous, Interpolate = Interpolate (nếu cần mượt)
        canDash = false;
    }

    void Update()
    {
        if (isStunned || isDashing) return;

        moveInput = Input.GetAxisRaw("Horizontal");
        isGrounded = CheckGrounded();

        // Nhảy chỉ khi đang grounded
        if (Input.GetKeyDown(KeyCode.K) && isGrounded)
        {
            jumpPressed = true;
        }

        // Animator params
        animator.SetBool("isRunning", moveInput != 0);
        animator.SetBool("isJumping", !isGrounded);

        // Lật hướng và vị trí firePoint
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
            if (firePoint != null) firePoint.localPosition = new Vector3(1f, 0f, 0f);
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
            if (firePoint != null) firePoint.localPosition = new Vector3(-1f, 0f, 0f);
        }

        // Dash
        if (canDash && !isDashing && Input.GetKeyDown(dashKey) && Time.time >= lastDashTime + dashCooldown)
        {
            Vector2 dashDir = new Vector2(spriteRenderer.flipX ? -1 : 1, 0);
            StartCoroutine(Dash(dashDir));
        }
        else if (!canDash && Input.GetKeyDown(dashKey))
        {
            Debug.Log("Bạn chưa mở khóa dash!");
        }
    }

    void FixedUpdate()
    {
        if (isStunned || isDashing) return;

        rb.velocity = new Vector2(moveInput * currentMoveSpeed, rb.velocity.y);

        if (jumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpPressed = false;
        }
    }

    public void ApplySlow(float slowPercentage, float duration)
    {
        if (slowCoroutine != null) StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowCoroutine(slowPercentage, duration));
    }

    private IEnumerator SlowCoroutine(float slowPercentage, float duration)
    {
        isSlowed = true;
        currentMoveSpeed = baseMoveSpeed * (1f - slowPercentage);
        yield return new WaitForSeconds(duration);
        RemoveSlowEffect();
    }

    private void RemoveSlowEffect()
    {
        isSlowed = false;
        currentMoveSpeed = baseMoveSpeed;
        slowCoroutine = null;
    }

    IEnumerator Dash(Vector2 dir)
    {
        isDashing = true;
        lastDashTime = Time.time;

        if (isSlowed) RemoveSlowEffect();

        animator.SetTrigger("Dash");

        if (dashSound != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            audioSource.PlayOneShot(dashSound);
        }

        rb.gravityScale = 0;

        float dashTimer = 0f;
        while (dashTimer < dashTime)
        {
            rb.velocity = dir * dashSpeed;
            dashTimer += Time.deltaTime;
            yield return null;
        }

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    public void UnlockDashPermanent()
    {
        if (!canDash)
        {
            canDash = true;
            Debug.Log("DASH ĐÃ ĐƯỢC MỞ KHÓA VĨNH VIỄN!");
            StartCoroutine(ShowDashUnlockedEffect());
        }
        else
        {
            Debug.Log("✅ Bạn đã có dash rồi!");
        }
    }

    IEnumerator ShowDashUnlockedEffect()
    {
        yield return new WaitForSeconds(1f);
    }

    // ✅ Ground check ổn định bằng OverlapCircle + LayerMask
    private bool CheckGrounded()
    {
        // Chỉ kiểm tra với Ground layer để tránh nhầm CameraConfiner/Player
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        return hit != null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DashItem"))
        {
            UnlockDashPermanent();
            Destroy(collision.gameObject);
        }
    }

    public void ApplyStun(float duration)
    {
        if (isSlowed) RemoveSlowEffect();
        if (!isStunned)
        {
            isStunned = true;
            rb.velocity = Vector2.zero;
            animator.SetTrigger("Stunned");
            StartCoroutine(StunCoroutine(duration));
        }
    }

    public void Stun(float duration)
    {
        if (!isStunned)
            StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Stunned");
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }

    internal void ResetState()
    {
        isStunned = false;
        isDashing = false;
        isSlowed = false;
        currentMoveSpeed = baseMoveSpeed;
        rb.gravityScale = originalGravity;
        jumpPressed = false;
        moveInput = 0;

        animator.SetBool("isRunning", false);
        animator.SetBool("isJumping", false);
    }

    // Gizmos để debug vị trí vòng tròn ground check
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}