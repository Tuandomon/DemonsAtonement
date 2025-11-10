using UnityEngine;

public class MageEnemyAttack : MonoBehaviour
{
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

    [Header("FireBall Skill Settings")]
    public GameObject fireBallPrefab;
    public float fireBallCooldown = 10f;
    private float nextFireBallTime = 0f;
    private bool isUsingFireBall = false;

    [Header("References")]
    public Animator animator;
    private Transform player;
    private float nextAttackTime;
    private Rigidbody2D rb;
    private Vector2 fixedPosition;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // 🧱 Lấy Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f; // Không bị rơi
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Không xoay
            fixedPosition = transform.position; // Ghi lại vị trí ban đầu để giữ Mage đứng yên
        }

        nextFireBallTime = Time.time + fireBallCooldown;
        nextTripleSkillTime = Time.time + tripleSkillCooldown;
    }

    private void Update()
    {
        if (player == null) return;

        // 🧲 Giữ vị trí cố định, tránh bị đẩy
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            transform.position = fixedPosition;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            // 🔄 Quay mặt về phía Player
            if (player.position.x < transform.position.x)
                transform.localScale = new Vector3(-0.8f, 0.8f, 1f);
            else
                transform.localScale = new Vector3(0.8f, 0.8f, 1f);

            // 🚫 Nếu đang dùng skill đặc biệt thì không bắn gì khác
            if (isUsingTripleSkill || isUsingFireBall) return;

            // 🔥 Ưu tiên dùng FireBall nếu hồi xong
            if (Time.time >= nextFireBallTime)
            {
                isUsingFireBall = true;
                animator.SetTrigger("Attack");
                Invoke(nameof(ShootFireBall), 0.6f);
                nextFireBallTime = Time.time + fireBallCooldown;
                return;
            }

            // ⚡ Dùng skill 3 tia nếu hồi xong
            if (Time.time >= nextTripleSkillTime)
            {
                isUsingTripleSkill = true;
                animator.SetTrigger("Attack");
                nextTripleSkillTime = Time.time + tripleSkillCooldown;
                Invoke(nameof(ShootTriple), 0.4f);
                return;
            }

            // 🏹 Bắn thường
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

    // 🏹 Bắn thường
    public void Shoot()
    {
        if (isUsingTripleSkill || isUsingFireBall) return;
        if (lightPrefab == null || firePoint == null || player == null) return;

        GameObject light = Instantiate(lightPrefab, firePoint.position, Quaternion.identity);

        Vector3 dir = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        light.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rbLight = light.GetComponent<Rigidbody2D>();
        if (rbLight != null)
            rbLight.velocity = dir * 6f;
    }

    // ⚡ Skill bắn 3 tia
    private void ShootTriple()
    {
        if (lightPrefab == null || firePoint == null || player == null) return;

        Vector3 baseDir = (player.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;
        float[] spreadAngles = { -angleSpread, 0f, angleSpread };

        foreach (float spread in spreadAngles)
        {
            GameObject light = Instantiate(lightPrefab, firePoint.position, Quaternion.Euler(0, 0, baseAngle + spread));
            Rigidbody2D rbLight = light.GetComponent<Rigidbody2D>();
            if (rbLight != null)
            {
                Vector2 shootDir = Quaternion.Euler(0, 0, spread) * baseDir;
                rbLight.velocity = shootDir * 6f;
            }
        }

        Invoke(nameof(ResetTripleSkill), 1f);
        Debug.Log("Mage used Triple Light Skill!");
    }

    private void ResetTripleSkill() => isUsingTripleSkill = false;

    // 🔥 Bắn FireBall
    private void ShootFireBall()
    {
        if (fireBallPrefab == null || firePoint == null || player == null) return;

        GameObject fireBall = Instantiate(fireBallPrefab, firePoint.position, Quaternion.identity);

        Vector3 dir = (player.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        fireBall.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rbFire = fireBall.GetComponent<Rigidbody2D>();
        if (rbFire != null)
            rbFire.velocity = dir * 8f;

        Invoke(nameof(ResetFireBallSkill), 1f);
        Debug.Log("Mage used FireBall!");
    }

    private void ResetFireBallSkill() => isUsingFireBall = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
