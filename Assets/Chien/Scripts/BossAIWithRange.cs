using UnityEngine;

public class BossAI_MoveOnly : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    public float detectionRadius = 5f;     // Vòng phát hiện player
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

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        anim.SetBool("isRunning", false);
    }

    void Update()
    {
        if (player == null || leftPoint == null || rightPoint == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool playerInRange = player.position.x > leftPoint.position.x && player.position.x < rightPoint.position.x;

        // Nếu player lọt vào vùng phát hiện → bắt đầu rượt
        if (!isChasing && distanceToPlayer <= detectionRadius && playerInRange)
        {
            isChasing = true;
            isReturning = false;
        }

        // Nếu player ra khỏi vùng trái–phải → quay lại
        if (!playerInRange && isChasing)
        {
            isChasing = false;
            isReturning = true;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else if (isReturning)
        {
            ReturnToStart();
        }
        else
        {
            Idle();
        }
    }

    void ChasePlayer()
    {
        anim.SetBool("isRunning", true);

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        FlipSprite(direction.x);
    }

    void ReturnToStart()
    {
        anim.SetBool("isRunning", true);

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
        rb.velocity = Vector2.zero;
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

        // Vẽ phạm vi trái–phải
        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftPoint.position + Vector3.up, leftPoint.position + Vector3.down);
            Gizmos.DrawLine(rightPoint.position + Vector3.up, rightPoint.position + Vector3.down);
        }
    }
}
