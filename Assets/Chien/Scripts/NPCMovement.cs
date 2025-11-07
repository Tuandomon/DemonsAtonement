using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [Header("Cấu hình di chuyển")]
    public float moveSpeed = 2f;               // Tốc độ di chuyển
    public Transform leftPoint;                // Giới hạn trái
    public Transform rightPoint;               // Giới hạn phải
    public float stopChance = 0.2f;            // Xác suất dừng lại (0.2 = 20%)
    public float stopDurationMin = 1f;         // Thời gian dừng tối thiểu
    public float stopDurationMax = 3f;         // Thời gian dừng tối đa

    private bool movingRight = true;           // Hướng di chuyển
    private bool isStopped = false;            // Có đang đứng im không
    private float stopTimer;                   // Bộ đếm dừng
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null)
            Debug.LogWarning("⚠️ Không tìm thấy Animator trên NPC!");
        if (leftPoint == null || rightPoint == null)
            Debug.LogWarning("⚠️ Hãy gán LeftPoint và RightPoint trong Inspector!");

        animator.Play("Nu di chuyen");
    }

    void Update()
    {
        if (isStopped)
        {
            // Đang dừng thì chỉ trừ thời gian chờ
            stopTimer -= Time.deltaTime;
            if (stopTimer <= 0)
            {
                isStopped = false;
                animator.Play("Nu di chuyen");
            }
            return;
        }

        Move();

        // Thỉnh thoảng sẽ dừng lại ngẫu nhiên
        if (Random.value < stopChance * Time.deltaTime)
        {
            StopForAWhile();
        }
    }

    void Move()
    {
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        // Lật mặt (nếu sprite mặc định quay TRÁI → để dòng này)
        spriteRenderer.flipX = movingRight;

        // Kiểm tra chạm giới hạn
        if (movingRight && transform.position.x >= rightPoint.position.x)
        {
            movingRight = false;
        }
        else if (!movingRight && transform.position.x <= leftPoint.position.x)
        {
            movingRight = true;
        }
    }

    void StopForAWhile()
    {
        isStopped = true;
        stopTimer = Random.Range(stopDurationMin, stopDurationMax);
        animator.Play("Nu dung im");
    }

    void OnDrawGizmosSelected()
    {
        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftPoint.position, rightPoint.position);
            Gizmos.DrawSphere(leftPoint.position, 0.1f);
            Gizmos.DrawSphere(rightPoint.position, 0.1f);
        }
    }
}
