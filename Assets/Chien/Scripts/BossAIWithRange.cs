using UnityEngine;

public class BossAI_MoveOnly : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    public float detectionRadius = 5f;      // Vòng phát hiện player
    public float attackRadius = 1.5f;       // Vòng tấn công player
    public float moveSpeed = 2f;
    public float returnSpeed = 2f;
    public float stopDistance = 0.2f;

    [Header("Movement Range")]
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Components")]
    private Animator anim;
    private Rigidbody2D rb;
    private Vector3 startPosition;

    private bool isChasing = false;
    private bool isReturning = false;
    private bool facingRight = true;
    private bool isAttacking = false;

    private float attackCooldown = 1.2f;   // Thời gian giữa 2 lần đánh
    private float attackTimer = 0f;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        anim.SetBool("isRunning", false);
        anim.SetBool("isAttacking", false);
    }

    void Update()
    {
        if (player == null || leftPoint == null || rightPoint == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool playerInRange = player.position.x > leftPoint.position.x && player.position.x < rightPoint.position.x;

        // Phát hiện và rượt đuổi
        if (!isChasing && distanceToPlayer <= detectionRadius && playerInRange)
        {
            isChasing = true;
            isReturning = false;
        }

        // Nếu player ra khỏi vùng → quay lại
        if (!playerInRange && isChasing)
        {
            isChasing = false;
            isReturning = true;
        }

        if (isChasing)
        {
            ChasePlayer(distanceToPlayer);
        }
        else if (isReturning)
        {
            ReturnToStart();
        }
        else
        {
            Idle();
        }

        attackTimer -= Time.deltaTime;
    }

    void ChasePlayer(float distanceToPlayer)
    {
        // Nếu player trong phạm vi tấn công → đánh
        if (distanceToPlayer <= attackRadius)
        {
            Attack();
        }
        else
        {
            // 🔹 Ra khỏi phạm vi tấn công → chuyển sang chạy
            if (isAttacking)
            {
                isAttacking = false;
                anim.SetBool("isAttacking", false);
                anim.ResetTrigger("Attack");
                anim.SetBool("isRunning", true);
            }

            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            FlipSprite(direction.x);
        }
    }

    void Attack()
    {
        // Không spam tấn công liên tục
        if (attackTimer <= 0f)
        {
            isAttacking = true;
            anim.SetBool("isAttacking", true);
            anim.SetBool("isRunning", false);
            anim.SetTrigger("Attack");
            rb.velocity = Vector2.zero;

            attackTimer = attackCooldown;
        }
    }

    void ReturnToStart()
    {
        anim.SetBool("isRunning", true);
        anim.SetBool("isAttacking", false);
        anim.ResetTrigger("Attack");

        Vector2 direction = (startPosition - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * returnSpeed, rb.velocity.y);
        FlipSprite(direction.x);

        if (Vector2.Distance(transform.position, startPosition) <= stopDistance)
        {
            rb.velocity = Vector2.zero;
            isReturning = false;
            Idle();
        }
    }

    void Idle()
    {
        anim.SetBool("isRunning", false);
        anim.SetBool("isAttacking", false);
        anim.ResetTrigger("Attack");
        rb.velocity = Vector2.zero;
        isAttacking = false;
    }

    // Gọi trong animation event cuối clip Attack
    public void EndAttack()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", false);
    }

    void FlipSprite(float moveX)
    {
        if (moveX > 0 && !facingRight)
            Flip();
        else if (moveX < 0 && facingRight)
            Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        // 🔴 Vòng phát hiện
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // 🟢 Vòng tấn công
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        // Phạm vi trái–phải
        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftPoint.position + Vector3.up, leftPoint.position + Vector3.down);
            Gizmos.DrawLine(rightPoint.position + Vector3.up, rightPoint.position + Vector3.down);
        }
    }
}
