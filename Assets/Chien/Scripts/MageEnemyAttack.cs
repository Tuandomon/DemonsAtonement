using UnityEngine;

public class MageEnemyAttack : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;   // 👈 Manual assign

    [Header("Normal Attack Settings")]
    public GameObject lightPrefab;
    public Transform firePoint;
    public float attackRange = 6f;
    public float attackCooldown = 2f;

    [Header("References")]
    public Animator animator;

    private float nextAttackTime;
    private Rigidbody2D rb;
    private Vector2 fixedPosition;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        rb = GetComponent<Rigidbody2D>();

        // GIỮ MAGE ĐỨNG YÊN
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            fixedPosition = transform.position;
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Không cho di chuyển
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            transform.position = fixedPosition;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            // Quay hướng
            if (player.position.x < transform.position.x)
                transform.localScale = new Vector3(-0.8f, 0.8f, 1f);
            else
                transform.localScale = new Vector3(0.8f, 0.8f, 1f);

            // Tấn công thường
            if (Time.time >= nextAttackTime)
            {
                animator.SetTrigger("Attack");
                nextAttackTime = Time.time + attackCooldown;
            }
        }
        else
        {
            animator.ResetTrigger("Attack");
            animator.Play("Idle");
        }
    }

    // *** Bắn thường ***
    public void Shoot()
    {
        if (lightPrefab == null || firePoint == null || player == null) return;

        GameObject light = Instantiate(lightPrefab, firePoint.position, Quaternion.identity);
        Vector3 dir = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        light.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rbLight = light.GetComponent<Rigidbody2D>();
        if (rbLight != null)
            rbLight.velocity = dir * 6f;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
