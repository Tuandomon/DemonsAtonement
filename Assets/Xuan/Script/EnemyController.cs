using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Cài đặt Di chuyển và Nhảy
    public float moveSpeed = 3f;
    public float jumpForce = 6f;
    public float jumpInterval = 2f;

    // Cài đặt Tấn công
    public float attackRange = 1f;
    public float attackCooldown = 1.0f;
    public int attackDamage = 10;

    // Cài đặt Slow Effect (Đã đảm bảo là PUBLIC để fix lỗi CS0106)
    [Header("Slow Effect")]
    public float slowPercent = 0.4f;
    public float slowDuration = 2.0f;

    // Cài đặt Hành vi Lùi Lại
    [Header("Retreat Behavior")]
    public float retreatDistance = 2.5f;

    // CÀI ĐẶT TẦM NHÌN PHÁT HIỆN DÙNG KHOẢNG CÁCH
    [Header("Detection Settings")]
    public float detectionRange = 7.0f;

    // Cài đặt Kỹ năng Triệu hồi
    [Header("Summon Skill Settings")]
    public GameObject minionPrefab;
    public Transform summonPoint;
    public float summonInterval = 15f; // Thời gian tồn tại Minion = Hồi chiêu

    private float summonTimer;
    public GameObject activeMinion;

    // Biến tham chiếu Hit Box
    [Header("Attack Hit Box")]
    public Collider2D attackHitBox;

    // Cài đặt Kiểm tra Mặt đất
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isGrounded;
    private bool playerDetected = false;
    private float jumpTimer;
    private float attackTimer;
    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        jumpTimer = jumpInterval;
        attackTimer = 0f;
        summonTimer = 0f; // Bắt đầu đếm ngược từ 0

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (attackHitBox != null)
        {
            attackHitBox.enabled = false;
        }
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // Cập nhật trạng thái Player Detected
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            bool wasDetected = playerDetected;

            if (distanceToPlayer <= detectionRange)
            {
                playerDetected = true;

                // >>> LOGIC TRIỆU HỒI NGAY LẬP TỨC VÀ ĐỊNH KỲ (KHÔNG DÙNG ANIMATION)
                if (activeMinion == null)
                {
                    // Lần đầu phát hiện HOẶC Minion vừa chết (timer = 0)
                    if (!wasDetected || summonTimer <= 0f)
                    {
                        SpawnMinions(); // Gọi spawn trực tiếp
                        summonTimer = summonInterval;
                    }
                    else
                    {
                        // Giảm timer (dù logic này không cần thiết vì timer được Minion đặt)
                        summonTimer -= Time.deltaTime;
                    }
                }
            }
            else
            {
                if (!isAttacking)
                {
                    playerDetected = false;
                }
            }
        }
        else
        {
            playerDetected = false;
        }


        // ... (Phần còn lại của Logic Hành vi Chính)
        if (playerDetected && player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            float direction = Mathf.Sign(player.position.x - transform.position.x);

            // A. Tấn công
            if (distance <= attackRange)
            {
                if (attackTimer <= 0f && !isAttacking)
                {
                    AttackPlayer();
                }

                rb.velocity = new Vector2(0, rb.velocity.y);
                animator.SetBool("isRunning", false);
            }
            // B. Đuổi theo / Lùi lại
            else if (!isAttacking)
            {
                float targetDirection = direction;
                if (distance < retreatDistance)
                {
                    targetDirection = -direction;
                }
                else if (distance > retreatDistance + 0.5f)
                {
                    targetDirection = direction;
                }
                else
                {
                    targetDirection = 0;
                }

                rb.velocity = new Vector2(targetDirection * moveSpeed, rb.velocity.y);
                spriteRenderer.flipX = direction < 0;
                animator.SetBool("isRunning", targetDirection != 0);

                // C. Nhảy định kỳ
                jumpTimer -= Time.deltaTime;
                if (jumpTimer <= 0f && isGrounded)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    jumpTimer = jumpInterval;
                }
            }
            else
            {
                spriteRenderer.flipX = direction < 0;
            }
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("isRunning", false);
            isAttacking = false;
        }

        animator.SetBool("isJumping", !isGrounded);
    }

    // HÀM SPAWN MINION THỰC SỰ (Không cần gọi từ Animation Event nữa)
    public void SpawnMinions()
    {
        if (minionPrefab == null || summonPoint == null) return;
        if (activeMinion != null) return;

        GameObject newMinion = Instantiate(minionPrefab, summonPoint.position, Quaternion.identity);
        activeMinion = newMinion; // Gán tham chiếu

        SummonedMinion minionScript = newMinion.GetComponent<SummonedMinion>();
        if (minionScript != null)
        {
            minionScript.bossController = this;
        }
    }

    // HÀM GỌI TỪ SCRIPT MINION KHI MINION BỊ HỦY
    public void MinionDestroyed(GameObject minion)
    {
        if (minion == activeMinion)
        {
            activeMinion = null;
            summonTimer = 0f;    // Đặt về 0 để Minion mới được triệu hồi ngay lập tức (nếu Player còn trong tầm)
        }
    }

    // ... Các hàm Tấn công/HitBox giữ nguyên (đã loại bỏ SummonMinions() cũ)
    void AttackPlayer()
    {
        isAttacking = true;
        animator.SetTrigger("attack");
        attackTimer = attackCooldown;
    }

    public void HitTarget()
    {
        if (attackHitBox != null)
        {
            attackHitBox.enabled = true;
        }
    }

    public void EndHitBox()
    {
        if (attackHitBox != null)
        {
            attackHitBox.enabled = false;
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        if (attackHitBox != null)
        {
            attackHitBox.enabled = false;
        }
    }
}