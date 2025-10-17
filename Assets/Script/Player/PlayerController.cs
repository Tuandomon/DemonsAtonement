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
    public bool canDash = false; // 🚫 Ban đầu không dash được

    private bool isDashing = false;
    private float lastDashTime;
    private float originalGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        // Input di chuyển ngang
        moveInput = Input.GetAxisRaw("Horizontal");

        // Kiểm tra chạm đất
        isGrounded = CheckGrounded();

        // Nhảy
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpPressed = true;
        }

        // Animation
        animator.SetBool("isRunning", moveInput != 0);
        animator.SetBool("isJumping", !isGrounded);

        // Lật sprite theo hướng
        if (moveInput > 0) spriteRenderer.flipX = false;
        else if (moveInput < 0) spriteRenderer.flipX = true;

        // Dash (chỉ khi đã mở khóa)
        if (canDash && !isDashing && Input.GetKeyDown(dashKey) && Time.time >= lastDashTime + dashCooldown)
        {
            Vector2 dashDir = new Vector2(spriteRenderer.flipX ? -1 : 1, 0);
            StartCoroutine(Dash(dashDir));
        }
    }

    void FixedUpdate()
    {
        // Nếu đang dash, bỏ qua di chuyển bình thường
        if (isDashing) return;

        // Di chuyển ngang
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

        // Vô hiệu trọng lực khi dash
        rb.gravityScale = 0;

        // Tốc độ dash
        rb.velocity = dir * dashSpeed;

        // Chờ dash kết thúc
        yield return new WaitForSeconds(dashTime);

        // Khôi phục
        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    // 🎁 Gọi khi nhặt item để bật dash
    public void UnlockDash(bool temporary = false, float duration = 0f)
    {
        canDash = true;
        Debug.Log("Dash Unlocked!");

        if (temporary && duration > 0)
            StartCoroutine(LoseDashAfter(duration));
    }

    IEnumerator LoseDashAfter(float time)
    {
        yield return new WaitForSeconds(time);
        canDash = false;
        Debug.Log("Dash Lost!");
    }

    // Kiểm tra mặt đất
    private bool CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius);
        return hit.collider != null && hit.collider.CompareTag("Ground");
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
