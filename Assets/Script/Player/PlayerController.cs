using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Nhận input di chuyển ngang
        moveInput = Input.GetAxisRaw("Horizontal");

        // Kiểm tra mặt đất bằng Raycast và tag "Ground"
        isGrounded = CheckGrounded();

        // Nhấn phím nhảy
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            jumpPressed = true;
        }

        // Cập nhật animation chạy
        animator.SetBool("isRunning", moveInput != 0);

        // Cập nhật animation nhảy
        animator.SetBool("isJumping", !isGrounded);

        // Lật sprite theo hướng di chuyển
        if (moveInput > 0)
            spriteRenderer.flipX = false;
        else if (moveInput < 0)
            spriteRenderer.flipX = true;
    }

    void FixedUpdate()
    {
        // Di chuyển ngang
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Thực hiện nhảy
        if (jumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpPressed = false;
        }
    }

    // Hàm kiểm tra mặt đất bằng Raycast và tag "Ground"
    private bool CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckRadius);
        return hit.collider != null && hit.collider.CompareTag("Ground");
    }
}