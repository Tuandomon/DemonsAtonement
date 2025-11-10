using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseMoveSpeed = 5f;
    private float currentMoveSpeed;
    public float jumpForce = 7f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

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
        originalGravity = rb.gravityScale;
        audioSource = GetComponent<AudioSource>();
        currentMoveSpeed = baseMoveSpeed;
        canDash = false;
    }

    void Update()
    {
        if (isStunned || isDashing) return;

        moveInput = Input.GetAxisRaw("Horizontal");
        isGrounded = CheckGrounded();

        if (Input.GetKeyDown(KeyCode.K) && isGrounded)
        {
            jumpPressed = true;
        }

        animator.SetBool("isRunning", moveInput != 0);
        animator.SetBool("isJumping", !isGrounded);

        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
            firePoint.localPosition = new Vector3(1f, 0f, 0f);
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
            firePoint.localPosition = new Vector3(-1f, 0f, 0f);
        }

        if (canDash && !isDashing && Input.GetKeyDown(dashKey) && Time.time >= lastDashTime + dashCooldown)
        {
            Vector2 dashDir = new Vector2(spriteRenderer.flipX ? -1 : 1, 0);
            StartCoroutine(Dash(dashDir));
        }

        if (!canDash && Input.GetKeyDown(dashKey))
        {
            Debug.Log(" Bạn chưa mở khóa dash!");
        }

        if (Input.GetKeyDown(KeyCode.K) && isGrounded && !isDashing)
        {
            jumpPressed = true;
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
            Debug.Log(" DASH ĐÃ ĐƯỢC MỞ KHÓA VĨNH VIỄN!");
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

    private bool CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius);
        return hit.collider != null && hit.collider.CompareTag("Ground");
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
}
