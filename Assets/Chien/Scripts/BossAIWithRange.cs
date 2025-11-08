using UnityEngine;

public class BossAI_RangeAndCircle : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;              // Gán Player
    public float detectionRadius = 5f;    // Vòng tròn phát hiện player
    public float moveSpeed = 2f;          // Tốc độ đuổi
    public float returnSpeed = 2f;        // Tốc độ quay về vị trí ban đầu
    public float stopDistance = 0.2f;     // Khoảng dừng khi về tới chỗ cũ

    [Header("Movement Range")]
    public Transform leftPoint;           // Giới hạn trái
    public Transform rightPoint;          // Giới hạn phải

    [Header("Components")]
    private Animator anim;
    private Rigidbody2D rb;
    private Vector3 startPosition;
    private bool isChasing = false;
    private bool isReturning = false;
    private bool facingRight = true;
    private bool detectionActive = true;  // Cho phép vòng tròn phát hiện hoạt động

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

        // Nếu đang cho phép vòng tròn phát hiện hoạt động
        if (detectionActive && distanceToPlayer <= detectionRadius && playerInRange)
        {
            isChasing = true;
            isReturning = false;
            detectionActive = false; // tắt vòng tròn để boss tiếp tục đuổi
        }

        // Nếu player ra khỏi phạm vi trái–phải → boss quay về
        if (!playerInRange && isChasing)
        {
            isChasing = false;
            isReturning = true;
        }

        if (isChasing)
            ChasePlayer();
        else if (isReturning)
            ReturnToStart();
        else
            Idle();
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
            detectionActive = true; // bật lại vòng tròn phát hiện
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
        // Vẽ vòng tròn phát hiện
        Gizmos.color = detectionActive ? Color.red : new Color(1, 0, 0, 0.3f); // nhạt hơn khi tắt
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
