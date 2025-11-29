using UnityEngine;

public class MageEnemyAttack : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;   // 👈 Thêm manual assign

    [Header("Normal Attack Settings")]
    public GameObject lightPrefab;
    public Transform firePoint;
    public float attackRange = 6f;
    public float attackCooldown = 2f;

    [Header("Triple Skill Settings")]
    public float tripleSkillCooldown = 5f;
    public float angleSpread = 15f;
    private float nextTripleSkillTime = 0f;
    private bool isUsingTripleSkill = false;

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

        nextTripleSkillTime = Time.time + tripleSkillCooldown;
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

            // Nếu đang dùng skill 3 tia → không bắn thường
            if (isUsingTripleSkill) return;

            // Skill 3 tia
            if (Time.time >= nextTripleSkillTime)
            {
                isUsingTripleSkill = true;
                animator.SetTrigger("Attack");
                Invoke(nameof(ShootTriple), 0.4f);
                nextTripleSkillTime = Time.time + tripleSkillCooldown;
                return;
            }

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

    // Bắn thường
    public void Shoot()
    {
        if (isUsingTripleSkill) return;
        if (lightPrefab == null || firePoint == null || player == null) return;

        GameObject light = Instantiate(lightPrefab, firePoint.position, Quaternion.identity);
        Vector3 dir = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        light.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rbLight = light.GetComponent<Rigidbody2D>();
        if (rbLight != null)
            rbLight.velocity = dir * 6f;
    }

    // Bắn 3 tia
    private void ShootTriple()
    {
        if (lightPrefab == null || firePoint == null || player == null) return;

        Vector3 baseDir = (player.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;

        float[] spreadAngles = { -angleSpread, 0f, angleSpread };

        foreach (float spread in spreadAngles)
        {
            GameObject light = Instantiate(lightPrefab, firePoint.position,
                Quaternion.Euler(0, 0, baseAngle + spread));

            Rigidbody2D rbLight = light.GetComponent<Rigidbody2D>();
            if (rbLight != null)
            {
                Vector2 shootDir = Quaternion.Euler(0, 0, spread) * baseDir;
                rbLight.velocity = shootDir * 6f;
            }
        }

        Invoke(nameof(ResetTripleSkill), 1f);
    }

    private void ResetTripleSkill() => isUsingTripleSkill = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
