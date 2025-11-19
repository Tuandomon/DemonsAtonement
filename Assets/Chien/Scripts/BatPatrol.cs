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
    public int attackDamage = 20;

    private void Start()
    {
        currentSpeed = normalSpeed;

        // Ignore collision with other enemies
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
        if (isAttackCooldown) return;
        if (player == null) return;

        // Luôn xoay hướng theo player ở mọi state
        RotateToPlayer();

        bool playerInBounds = player.position.x >= leftPoint.position.x && player.position.x <= rightPoint.position.x;

        if (!playerInBounds)
        {
            Patrol();
            return;
        }

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

    // ⭐ Xoay theo hướng player
    void RotateToPlayer()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x)
            transform.rotation = Quaternion.Euler(0, 180, 0); // nhìn phải
        else
            transform.rotation = Quaternion.Euler(0, 0, 0);   // nhìn trái
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

            if (transform.position.x >= rightPoint.position.x)
                movingRight = false;
        }
        else
        {
            rb.velocity = new Vector2(-currentSpeed, 0);

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
            rb.velocity = new Vector2(currentSpeed, 0);
        else
            rb.velocity = new Vector2(-currentSpeed, 0);
    }

    // ==========================
    //         ATTACK
    // ==========================
    void AttackPlayer()
    {
        isAttackCooldown = true;

        rb.velocity = Vector2.zero; // đứng yên
        RotateToPlayer();           // ⭐ xoay ngay lập tức khi bắt đầu tấn công

        anim.Play("Bat_Tan cong");

        if (player != null)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(attackDamage);
            }
        }

        Invoke(nameof(IdleFly), 0.6f);
        Invoke(nameof(ResetAttack), 2f);
    }

    void IdleFly()
    {
        rb.velocity = Vector2.zero;
        RotateToPlayer();          // ⭐ xoay theo player khi đang idle, không bị đứng quay ngược
        anim.Play("Bat_Fly");
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

        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(new Vector3(leftPoint.position.x, transform.position.y, 0),
                            new Vector3(rightPoint.position.x, transform.position.y, 0));
        }
    }
}
