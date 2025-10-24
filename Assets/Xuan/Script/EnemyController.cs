using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float attackRange = 1f;
    public float jumpForce = 6f;
    public float jumpInterval = 2f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isGrounded;
    private bool playerDetected = false;
    private float jumpTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        jumpTimer = jumpInterval;
    }

    void Update()
    {
        // Kiểm tra mặt đất
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Nếu phát hiện player
        if (playerDetected && player != null)
        {
            float direction = Mathf.Sign(player.position.x - transform.position.x);
            rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

            // Lật hướng sprite
            spriteRenderer.flipX = direction < 0;

            // Cập nhật animation chạy
            animator.SetBool("isRunning", true);

            // Tấn công nếu trong phạm vi
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= attackRange)
            {
                AttackPlayer();
            }

            // Nhảy mỗi 2 giây nếu đang đứng trên mặt đất
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                animator.SetTrigger("jump");
                jumpTimer = jumpInterval;
            }
        }
        else
        {
            // Không nhảy nếu không thấy player
            jumpTimer = jumpInterval;

            // Dừng di chuyển
            rb.velocity = new Vector2(0, rb.velocity.y);

            // Cập nhật animation idle
            animator.SetBool("isRunning", false);
        }

        // Cập nhật trạng thái nhảy
        animator.SetBool("isJumping", !isGrounded);
    }

    void AttackPlayer()
    {
        animator.SetTrigger("attack");
        Debug.Log("Enemy tấn công Player!");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerDetected = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerDetected = false;
            player = null;
        }
    }
}