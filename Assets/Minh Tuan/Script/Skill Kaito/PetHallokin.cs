using UnityEngine;
using System.Linq;

public class PetHallokin : MonoBehaviour
{
    [Header("Tham chiếu")]
    private Transform player;                 // Player sẽ tự tìm bằng tag
    private Animator animator;                // Animator (Idle, Run, Attack)
    private SpriteRenderer spriteRenderer;    // Flip hướng

    [Header("Di chuyển")]
    public float moveSpeed = 4f;
    public float followDistance = 1.5f;      // Khoảng cách đứng cạnh Player
    public float returnThreshold = 6f;       // Nếu xa hơn thì chạy về Player
    public float teleportDistance = 15f;     // Nếu xa hơn thì dịch chuyển về Player

    [Header("Chiến đấu")]
    public float detectionRadius = 6f;       // Phạm vi tìm Enemy
    public float attackRange = 1.2f;         // Phạm vi đánh
    public float attackCooldown = 1f;        // 1 giây / lần đánh
    public int attackDamage = 20;            // Sát thương
    //public LayerMask enemyLayer;             // Layer Enemy

    private float lastAttackTime = -Mathf.Infinity;
    private Transform currentTarget;
    private bool isAttacking = false;

    void Start()
    {
        // Luôn tự tìm Player bằng tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        // Teleport nếu quá xa
        if (distToPlayer > teleportDistance)
        {
            transform.position = player.position;
            currentTarget = null;
            isAttacking = false;
        }

        if (isAttacking) return;

        // Nếu xa Player hơn returnThreshold thì chạy về Player
        if (distToPlayer > returnThreshold)
        {
            MoveTowards(player.position);
            SetAnimRun(true);
            return;
        }

        // Tìm Enemy gần nhất
        AcquireTarget();

        if (currentTarget != null)
        {
            float distToEnemy = Vector2.Distance(transform.position, currentTarget.position);

            if (distToEnemy <= attackRange)
            {
                TryAttack(currentTarget);
                SetAnimRun(false);
            }
            else
            {
                MoveTowards(currentTarget.position);
                SetAnimRun(true);
            }
        }
        else
        {
            // Không có Enemy -> đứng cạnh Player
            if (distToPlayer > followDistance)
            {
                MoveTowards(player.position);
                SetAnimRun(true);
            }
            else
            {
                SetAnimRun(false); // Idle
            }
        }
    }

    void AcquireTarget()
    {
        // Lấy tất cả Collider trong detectionRadius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        if (hits.Length == 0)
        {
            currentTarget = null;
            return;
        }

        // Chỉ chọn những object có Tag = "Enemy"
        currentTarget = hits
            .Where(h => h.CompareTag("Enemy"))
            .Select(h => h.transform)
            .OrderBy(t => Vector2.Distance(transform.position, t.position))
            .FirstOrDefault();
    }

    void MoveTowards(Vector2 targetPos)
    {
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        transform.position += (Vector3)(dir * moveSpeed * Time.deltaTime);

        if (spriteRenderer != null)
        {
            if (dir.x > 0.05f) spriteRenderer.flipX = false;
            else if (dir.x < -0.05f) spriteRenderer.flipX = true;
        }
    }

    void TryAttack(Transform enemy)
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        Vector2 dir = enemy.position - transform.position;
        if (spriteRenderer != null) spriteRenderer.flipX = dir.x < 0f;

        animator.SetTrigger("Attack");
        isAttacking = true;
        lastAttackTime = Time.time;
    }

    // Gọi từ Animation Event ở frame ra đòn
    public void PetAttackHit()
    {
        if (currentTarget == null) return;
        float dist = Vector2.Distance(transform.position, currentTarget.position);
        if (dist <= attackRange)
        {
            EnemyHealth eh = currentTarget.GetComponent<EnemyHealth>();
            if (eh != null) eh.TakeDamage(attackDamage);
        }
    }

    // Gọi từ Animation Event ở cuối clip Attack
    public void PetAttackEnd()
    {
        isAttacking = false;
    }

    void SetAnimRun(bool running)
    {
        animator.SetBool("isRunning", running);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}