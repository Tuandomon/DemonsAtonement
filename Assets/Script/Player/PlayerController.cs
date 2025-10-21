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
    public bool canDash = false; //  BAN ĐẦU KHÔNG DASH ĐƯỢC

    private bool isDashing = false;
    private float lastDashTime;
    private float originalGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalGravity = rb.gravityScale;

        // CHẮC CHẮN DASH CHƯA MỞ KHÓA KHI BẮT ĐẦU
        canDash = false;
        Debug.Log(" Dash chưa được mở khóa - cần nhặt item!");
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

        // Animation
        animator.SetBool("isRunning", moveInput != 0);
        animator.SetBool("isJumping", !isGrounded);

        // Lật hướng nhân vật
        if (moveInput > 0) spriteRenderer.flipX = false;
        else if (moveInput < 0) spriteRenderer.flipX = true;

        //  DASH - CHỈ HOẠT ĐỘNG KHI ĐÃ MỞ KHÓA
        if (canDash && !isDashing && Input.GetKeyDown(dashKey) && Time.time >= lastDashTime + dashCooldown)
        {
            Vector2 dashDir = new Vector2(spriteRenderer.flipX ? -1 : 1, 0);
            StartCoroutine(Dash(dashDir));
        }
        
        //  HIỂN THỊ CẢNH BÁO NẾU CHƯA MỞ KHÓA DASH
        if (!canDash && Input.GetKeyDown(dashKey))
        {
            Debug.Log(" Bạn chưa mở khóa dash! Hãy tìm item dash để mở khóa.");
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

        rb.gravityScale = 0;
        rb.velocity = dir * dashSpeed;

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    // HÀM MỞ KHÓA DASH VĨNH VIỄN
    public void UnlockDashPermanent()
    {
        if (!canDash) // Chỉ mở khóa nếu chưa có
        {
            canDash = true;
            Debug.Log(" DASH ĐÃ ĐƯỢC MỞ KHÓA VĨNH VIỄN!");

            // Có thể thêm hiệu ứng, âm thanh ở đây
            StartCoroutine(ShowDashUnlockedEffect());
        }
        else
        {
            Debug.Log("✅ Bạn đã có dash rồi!");
        }
    }

    IEnumerator ShowDashUnlockedEffect()
    {
        // Hiệu ứng khi mở khóa dash (tuỳ chọn)
        Debug.Log(" Hiệu ứng mở khóa dash!");
        yield return new WaitForSeconds(1f);
    }

    private bool CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius);
        return hit.collider != null && hit.collider.CompareTag("Ground");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //  KHI CHẠM VÀO ITEM DASH
        if (collision.CompareTag("DashItem"))
        {
            // TỰ ĐỘNG MỞ KHÓA DASH VĨNH VIỄN
            UnlockDashPermanent();
            
            // TỰ ĐỘNG BIẾN MẤT ITEM
            Destroy(collision.gameObject);
            
            Debug.Log("🎯 Đã nhặt item dash và kích hoạt thành công!");
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