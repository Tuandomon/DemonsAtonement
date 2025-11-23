using UnityEngine;
using System.Collections;

public class MageEnemyAI : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform targetPlayer;   // <-- Player chỉ định thủ công

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;
    private bool movingRight = true;
    private bool isIdle = false;
    public float idleTime = 2f;

    [Header("Attack Settings")]
    public float attackRange = 6f;
    public float attackCooldown = 2f;

    [Header("Triple Skill Settings")]
    public float tripleSkillCooldown = 5f;
    public float angleSpread = 15f;
    private bool isUsingTripleSkill = false;
    private float nextTripleSkillTime = 0f;

    [Header("FireBall Skill Settings")]
    public GameObject fireBallPrefab;
    public float fireBallCooldown = 10f;
    private bool isUsingFireBall = false;
    private float nextFireBallTime = 0f;

    [Header("Normal Attack Settings")]
    public GameObject lightPrefab;
    public Transform firePoint;

    [Header("References")]
    public Animator animator;
    private Transform player;
    private Rigidbody2D rb;
    private float nextAttackTime;

    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ưu tiên dùng Player được kéo vào Inspector
        if (targetPlayer != null)
            player = targetPlayer;
        else
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        nextFireBallTime = Time.time + fireBallCooldown;
        nextTripleSkillTime = Time.time + tripleSkillCooldown;

        // Bỏ qua va chạm với Enemy, EnemyAttack, Enemy_Wolf
        Collider2D myCollider = GetComponent<Collider2D>();
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();

        foreach (Collider2D col in allColliders)
        {
            if (col == myCollider) continue;
            if (col.CompareTag("Enemy") || col.CompareTag("EnemyAttack") || col.CompareTag("Enemy_Wolf"))
                Physics2D.IgnoreCollision(myCollider, col);
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (!isIdle && distance > attackRange)
            Patrol();

        if (distance <= attackRange)
            FacePlayer();

        if (distance <= attackRange)
        {
            rb.velocity = Vector2.zero;
            TryAttackOrSkill();
        }
    }

    void Patrol()
    {
        animator.SetBool("isRunning", true);

        float targetX = movingRight ? rightPoint.position.x : leftPoint.position.x;
        float step = moveSpeed * Time.deltaTime;
        transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, targetX, step), transform.position.y, transform.position.z);

        transform.eulerAngles = new Vector3(0, movingRight ? 0f : 180f, 0);

        if ((movingRight && transform.position.x >= targetX) || (!movingRight && transform.position.x <= targetX))
        {
            StartCoroutine(IdleAndTurn(!movingRight));
        }
    }

    IEnumerator IdleAndTurn(bool nextMoveRight)
    {
        isIdle = true;
        animator.SetBool("isRunning", false);
        yield return new WaitForSeconds(idleTime);
        movingRight = nextMoveRight;
        isIdle = false;
    }

    void FacePlayer()
    {
        transform.eulerAngles = new Vector3(0, player.position.x > transform.position.x ? 0f : 180f, 0);
    }

    void TryAttackOrSkill()
    {
        if (isUsingTripleSkill || isUsingFireBall) return;

        if (Time.time >= nextFireBallTime)
        {
            isUsingFireBall = true;
            animator.SetTrigger("Attack");
            Invoke(nameof(ShootFireBall), 0.6f);
            nextFireBallTime = Time.time + fireBallCooldown;
            return;
        }

        if (Time.time >= nextTripleSkillTime)
        {
            isUsingTripleSkill = true;
            animator.SetTrigger("Attack");
            Invoke(nameof(ShootTriple), 0.4f);
            nextTripleSkillTime = Time.time + tripleSkillCooldown;
            return;
        }

        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack");
            Invoke(nameof(Shoot), 0.3f);   // ✔ Đã đổi từ ShootNormal → Shoot
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // ✔ Hàm bắn thường đổi tên thành Shoot()
    void Shoot()
    {
        if (!lightPrefab || !firePoint || !player) return;

        GameObject light = Instantiate(lightPrefab, firePoint.position, Quaternion.identity);
        Vector2 dir = (player.position - firePoint.position).normalized;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        light.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rbLight = light.GetComponent<Rigidbody2D>();
        if (rbLight != null) rbLight.velocity = dir * 6f;
    }

    void ShootTriple()
    {
        if (!lightPrefab || !firePoint || !player) return;

        Vector2 baseDir = (player.position - firePoint.position).normalized;
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
    }

    void ResetTripleSkill() => isUsingTripleSkill = false;

    void ShootFireBall()
    {
        if (!fireBallPrefab || !firePoint || !player) return;

        Vector2 dir = (player.position - firePoint.position).normalized;

        GameObject fireBall = Instantiate(fireBallPrefab, firePoint.position, Quaternion.identity);

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        fireBall.transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rbFire = fireBall.GetComponent<Rigidbody2D>();
        if (rbFire != null)
            rbFire.velocity = dir * 8f;

        Invoke(nameof(ResetFireBallSkill), 1f);
    }

    void ResetFireBallSkill() => isUsingFireBall = false;

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        Die();
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
}
