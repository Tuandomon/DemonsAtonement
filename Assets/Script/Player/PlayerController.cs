using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float moveInput;
    private bool isGrounded;
    private bool jumpPressed;

    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashTime = 0.15f;
    public float dashCooldown = 0.6f;
    public KeyCode dashKey = KeyCode.LeftShift;

    [Header("Dash Unlock")]
    public bool canDash = false; // 🚫 Ban đầu chưa có dash
    private bool isDashing = false;
    private float lastDashTime;
    private float originalGravity;

    [Header("Dash Effects")]
    public TrailRenderer dashTrail;     // Hiệu ứng vệt sáng khi dash
    public Color dashColor = Color.magenta; // Màu tím khi dash
    public AudioClip dashSound;         // Âm thanh dash (nếu có)

    private AudioSource audioSource;
    private Color originalColor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        originalGravity = rb.gravityScale;
        originalColor = spriteRenderer.color;

        canDash = false;
        Debug.Log("🚫 Dash chưa được mở khóa - cần nhặt item!");
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        isGrounded = CheckGrounded();

        // Nhảy
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpPressed = true;
        }

        // Animation di chuyển
        animator.SetBool("isRunning", moveInput != 0);
        animator.SetBool("isJumping", !isGrounded);

        // Lật hướng nhân vật
        if (moveInput > 0) spriteRenderer.flipX = false;
        else if (moveInput < 0) spriteRenderer.flipX = true;

        // Dash (chỉ khi đã mở khóa)
        if (canDash && !isDashing && Input.GetKeyDown(dashKey) && Time.time >= lastDashTime + dashCooldown)
        {
            Vector2 dashDir = new Vector2(spriteRenderer.flipX ? -1 : 1, 0);
            StartCoroutine(Dash(dashDir));
        }

        // Nếu chưa mở khóa dash
        if (!canDash && Input.GetKeyDown(dashKey))
        {
            Debug.Log("⚠️ Bạn chưa mở khóa dash! Hãy tìm item dash để kích hoạt.");
        }
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isDashing)
        {
            jumpPressed = true;
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        // Di chuyển
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Nhảy
        if (jumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpPressed = false;
        }
    }

    IEnumerator Dash(Vector2 dir)
    {
        isDashing = true;
        lastDashTime = Time.time;

        // 🔥 Bật animation Dash
        animator.SetTrigger("Dash");

        // 🔊 Phát âm thanh dash nếu có
        if (dashSound != null && audioSource != null)
            audioSource.PlayOneShot(dashSound);

        // 🌈 Hiệu ứng màu tím
        spriteRenderer.color = dashColor;

        // 💫 Bật Trail nếu có
        if (dashTrail != null)
            dashTrail.emitting = true;

        rb.gravityScale = 0;
        rb.velocity = dir * dashSpeed;

        yield return new WaitForSeconds(dashTime);

        // Tắt hiệu ứng
        if (dashTrail != null)
            dashTrail.emitting = false;

        spriteRenderer.color = originalColor;
        rb.gravityScale = originalGravity;
        rb.velocity = Vector2.zero;

        isDashing = false;
    }

    // 🔓 Mở khóa dash vĩnh viễn
    public void UnlockDashPermanent()
    {
        if (!canDash)
        {
            canDash = true;
            Debug.Log("✅ DASH ĐÃ ĐƯỢC MỞ KHÓA VĨNH VIỄN!");
            StartCoroutine(ShowDashUnlockedEffect());
        }
        else
        {
            Debug.Log("⚡ Bạn đã có dash rồi!");
        }
    }

    IEnumerator ShowDashUnlockedEffect()
    {
        Debug.Log("✨ Hiệu ứng mở khóa dash!");
        // Có thể thêm Particle hoặc animation mở khóa ở đây
        yield return new WaitForSeconds(1f);
    }

    private bool CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius);
        return hit.collider != null && hit.collider.CompareTag("Ground");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 🧿 Khi chạm vào Item Dash
        if (collision.CompareTag("DashItem"))
        {
            UnlockDashPermanent();
            Destroy(collision.gameObject);
            Debug.Log("🎯 Đã nhặt item dash và kích hoạt dash vĩnh viễn!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
