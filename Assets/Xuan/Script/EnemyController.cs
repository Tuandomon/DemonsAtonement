using UnityEngine;

// Đổi tên class để phản ánh chức năng mới (Chỉ di chuyển)
public class EnemyController : MonoBehaviour
{
    // --- Cài đặt Di chuyển và Nhảy ---
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float jumpForce = 6f;
    public float jumpInterval = 2f;

    // --- CÀI ĐẶT TẦM NHÌN PHÁT HIỆN DÙNG KHOẢNG CÁCH ---
    [Header("Detection Settings")]
    public float detectionRange = 7.0f;

    // Cài đặt Hành vi Lùi Lại
    [Header("Retreat Behavior")]
    public float retreatDistance = 2.5f;

    // Cài đặt Kiểm tra Mặt đất
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // --- Biến nội bộ ---
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isGrounded;
    private bool playerDetected = false;
    private float jumpTimer;

    // --- Loại bỏ: attackTimer, isAttacking, summonTimer, activeMinion...

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        jumpTimer = jumpInterval;

        // Tìm và gán Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // --- Loại bỏ: logic attackHitBox
    }

    void Update()
    {
        // 1. Kiểm tra Mặt đất (Ground Check)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (player == null)
        {
            // Dừng nếu không tìm thấy Player
            rb.velocity = new Vector2(0, rb.velocity.y);
            if (animator != null) animator.SetBool("isRunning", false);
            return;
        }

        // 2. Cập nhật trạng thái Player Detected
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        playerDetected = (distanceToPlayer <= detectionRange);


        if (playerDetected)
        {
            // Player Đã Phát hiện -> Thực hiện Di chuyển
            float distance = distanceToPlayer;
            float direction = Mathf.Sign(player.position.x - transform.position.x);

            // Xử lý Hành vi Đuổi theo / Lùi lại
            float targetDirection;

            if (distance < retreatDistance)
            {
                // Gần quá, lùi lại
                targetDirection = -direction;
            }
            else if (distance > retreatDistance + 0.5f)
            {
                // Xa quá, đuổi theo
                targetDirection = direction;
            }
            else
            {
                // Giữ khoảng cách
                targetDirection = 0;
            }

            // A. Di chuyển
            rb.velocity = new Vector2(targetDirection * moveSpeed, rb.velocity.y);

            // B. Lật Sprite và Cập nhật Hoạt ảnh
            spriteRenderer.flipX = direction < 0; // Quay mặt về phía Player
            if (animator != null) animator.SetBool("isRunning", targetDirection != 0);

            // C. Nhảy định kỳ
            jumpTimer -= Time.deltaTime;
            if (jumpTimer <= 0f && isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpTimer = jumpInterval;
            }
        }
        else // Player ngoài tầm phát hiện
        {
            // Dừng di chuyển
            rb.velocity = new Vector2(0, rb.velocity.y);
            if (animator != null) animator.SetBool("isRunning", false);
        }

        // Cập nhật hoạt ảnh Nhảy
        if (animator != null) animator.SetBool("isJumping", !isGrounded);
    }

    // --- Loại bỏ: AttackPlayer(), HitTarget(), EndHitBox(), EndAttack(), SummonMinions(), MinionDestroyed()
}