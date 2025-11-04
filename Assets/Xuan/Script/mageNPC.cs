using UnityEngine;

public class mageNPC : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float moveSpeed = 3f;
    public float chaseRange = 10f;
    public float attackRange = 6f;
    public float attackDelay = 2f;

    private Transform playerTarget;
    private Animator animator;
    private float lastAttackTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning("Không tìm thấy Animator.");

        if (firePoint == null)
        {
            GameObject found = GameObject.Find("FirePoint");
            if (found != null)
                firePoint = found.transform;
            else
                Debug.LogWarning("Không tìm thấy FirePoint.");
        }
    }

    void Update()
    {
        FindPlayer();

        if (playerTarget != null)
        {
            float distance = Vector2.Distance(transform.position, playerTarget.position);

            FaceTarget(playerTarget);

            if (distance <= attackRange)
            {
                animator.SetBool("isRunning", false);

                if (Time.time >= lastAttackTime + attackDelay)
                {
                    animator.SetTrigger("Attack");
                    lastAttackTime = Time.time;
                }
            }
            else if (distance <= chaseRange)
            {
                animator.SetBool("isRunning", true);
                MoveTowards(playerTarget);
            }
            else
            {
                animator.SetBool("isRunning", false);
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    void FindPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, chaseRange);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                playerTarget = hit.transform;
                return;
            }
        }

        playerTarget = null;
    }

    void MoveTowards(Transform target)
    {
        Vector2 direction = (target.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    void FaceTarget(Transform target)
    {
        if (target != null)
        {
            float dir = target.position.x - transform.position.x;
            if (dir < 0)
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
    }

    // Gọi từ animation event "Attack"
    public void ShootAtPlayer()
    {
        if (playerTarget == null || Vector2.Distance(transform.position, playerTarget.position) > attackRange)
            return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Vector2 direction = (playerTarget.position - firePoint.position).normalized;

        // Xoay viên đạn theo hướng bay
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Gửi hướng đầy đủ cho BulletEnemy
        BulletEnemy bulletScript = bullet.GetComponent<BulletEnemy>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}