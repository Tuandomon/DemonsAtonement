using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Cài đặt Di chuyển và Nhảy
    public float moveSpeed = 3f;
    public float jumpForce = 6f;
    public float jumpInterval = 2f;

    // Cài đặt Tấn công
    public float attackRange = 1f;
    public float attackCooldown = 1.0f; // Thêm Cooldown để không tấn công liên tục

    // Cài đặt Kiểm tra Mặt đất
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform player; // Biến lưu trữ vị trí Player

    // Biến trạng thái
    private bool isGrounded;
    private bool playerDetected = false;
    private float jumpTimer;
    private float attackTimer; // Timer cho Cooldown tấn công

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // Khởi tạo Timer
        jumpTimer = jumpInterval;
        attackTimer = 0f;

        // Cần đảm bảo GameObject Player có tag "Player"
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            // Trong trường hợp Enemy Controller cần biết player trước khi va chạm
            // Tùy theo logic thiết kế, ta có thể bỏ dòng này. 
            // Hiện tại ta vẫn dựa vào OnTriggerEnter2D.
        }
    }

    void Update()
    {
        // 1. Cập nhật trạng thái chạm đất
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // 2. Cập nhật Attack Cooldown
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // 3. Logic Hành vi Chính
        if (playerDetected && player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            float direction = Mathf.Sign(player.position.x - transform.position.x);

            // A. Tấn công nếu trong phạm vi và hết Cooldown
            if (distance <= attackRange)
            {
                AttackPlayer();

                // Dừng di chuyển khi tấn công (tùy chọn)
                rb.velocity = new Vector2(0, rb.velocity.y);
                animator.SetBool("isRunning", false);
            }
            // B. Đuổi theo Player
            else
            {
                // Di chuyển theo Player
                rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

                // Lật hướng sprite
                spriteRenderer.flipX = direction < 0;

                // Cập nhật animation chạy (RUNNING)
                animator.SetBool("isRunning", true);

                // C. Nhảy mỗi 2 giây nếu đang đứng trên mặt đất (JUMPING)
                jumpTimer -= Time.deltaTime;
                if (jumpTimer <= 0f && isGrounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    // Đã loại bỏ animator.SetTrigger("jump");
                    jumpTimer = jumpInterval;
                }
            }
        }
        else // Không phát hiện Player
        {
            // Dừng di chuyển
            rb.velocity = new Vector2(0, rb.velocity.y);

            // Cập nhật animation idle
            animator.SetBool("isRunning", false);

            // Đặt lại jumpTimer về trạng thái sẵn sàng
            jumpTimer = jumpInterval;
        }

        // 4. Cập nhật trạng thái nhảy cho Animator
        // Điều này sẽ điều khiển transition Jump ⇔ Idle/Run trong Animator
        animator.SetBool("isJumping", !isGrounded);
    }

    // Hàm gọi để Boss/Kẻ thù tấn công
    void AttackPlayer()
    {
        if (attackTimer <= 0f)
        {
            animator.SetTrigger("attack");
            attackTimer = attackCooldown; // Đặt lại Cooldown

            // TO DO: Thêm logic gây sát thương cho Player Health System (script PlayerHealth) ở đây
            // Ví dụ: player.GetComponent<PlayerHealth>().TakeDamage(damageAmount);

            Debug.Log("Enemy tấn công Player!");
        }
    }

    // Phát hiện Player đi vào tầm nhìn (trigger collider)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            playerDetected = true;
        }
    }

    // Phát hiện Player đi ra khỏi tầm nhìn
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerDetected = false;
            player = null;

            // Đảm bảo dừng mọi hành động khi mất mục tiêu
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("isRunning", false);
        }
    }

    // TO DO: Thêm hàm TakeDamage/Die để Boss tương tác với script PlayerHealth
    // public void TakeDamage(int damage) { ... }
    // void Die() { ... }
}