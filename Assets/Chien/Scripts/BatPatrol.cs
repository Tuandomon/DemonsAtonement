using UnityEngine;

public class BatAI : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    public LayerMask playerLayer;

    [Header("Ranges")]
    public float detectionRadius = 8f;
    public float attackRadius = 2f;
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Movement")]
    public float normalSpeed = 2f;
    public float chaseSpeed = 4f;
    private float currentSpeed;

    private bool movingRight = true;
    private bool isAttackCooldown = false;

    [Header("Components")]
    public Animator anim;
    public Rigidbody2D rb;

    [Header("Damage Settings")]
    public int attackDamage = 20; // 💥 Sát thương

    private void Start()
    {
        currentSpeed = normalSpeed;

        // 🔹 Bỏ qua va chạm với các enemy khác
        Collider2D myCollider = GetComponent<Collider2D>();
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
        foreach (Collider2D col in allColliders)
        {
            if (col == myCollider) continue;
            if (col.CompareTag("Enemy") || col.CompareTag("EnemyAttack") || col.CompareTag("Enemy_Wolf"))
            {
                Physics2D.IgnoreCollision(myCollider, col);
            }
        }
    }

    private void Update()
    {
        if (isAttackCooldown) return;  // ĐANG NGHỈ → ĐỨNG YÊN
        if (player == null) return;

        // Kiểm tra player có trong phạm vi Left–Right không
        bool playerInBounds = player.position.x >= leftPoint.position.x && player.position.x <= rightPoint.position.x;

        // Nếu player ra ngoài giới hạn → Patrol
        if (!playerInBounds)
        {
            Patrol();
            return;
        }

        // Kiểm tra phạm vi phát hiện và tấn công
        bool playerInDetect = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        bool playerInAttack = Physics2D.OverlapCircle(transform.position, attackRadius, playerLayer);

        if (playerInAttack)
        {
            AttackPlayer();
        }
        else if (playerInDetect)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    // ==========================
    //        PATROL
    // ==========================
    void Patrol()
    {
        anim.Play("Bat_Fly");
        currentSpeed = normalSpeed;

        if (movingRight)
        {
            rb.velocity = new Vector2(currentSpeed, 0);
            transform.rotation = Quaternion.Euler(0, 180, 0);

            if (transform.position.x >= rightPoint.position.x)
                movingRight = false;
        }
        else
        {
            rb.velocity = new Vector2(-currentSpeed, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);

            if (transform.position.x <= leftPoint.position.x)
                movingRight = true;
        }
    }

    // ==========================
    //       CHASE PLAYER
    // ==========================
    void ChasePlayer()
    {
        anim.Play("Bat_Fly");
        currentSpeed = chaseSpeed;

        if (player.position.x > transform.position.x)
        {
            rb.velocity = new Vector2(currentSpeed, 0);
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            rb.velocity = new Vector2(-currentSpeed, 0);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // ==========================
    //         ATTACK
    // ==========================
    void AttackPlayer()
    {
        isAttackCooldown = true;

        rb.velocity = Vector2.zero; // ĐỨNG YÊN

        anim.Play("Bat_Tan cong");

        // 💥 Gây damage ngay khi tấn công
        if (player != null)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(attackDamage);
            }
        }

        // Sau khi animation đánh xong → chuyển sang Fly nhưng vẫn đứng yên
        Invoke(nameof(IdleFly), 0.6f);

        // Sau 2 giây cho phép tấn công tiếp
        Invoke(nameof(ResetAttack), 2f);
    }

    void IdleFly()
    {
        rb.velocity = Vector2.zero;       // GIỮ ĐỨNG YÊN
        anim.Play("Bat_Fly");            // Animation bay nhưng KHÔNG di chuyển
    }

    void ResetAttack()
    {
        isAttackCooldown = false;
    }

    // ==========================
    //       DRAW GIZMOS
    // ==========================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        // Hiển thị phạm vi Left–Right
        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(new Vector3(leftPoint.position.x, transform.position.y, 0),
                            new Vector3(rightPoint.position.x, transform.position.y, 0));
        }
    }
}
