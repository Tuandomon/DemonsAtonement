using UnityEngine;
using System.Collections;

public class EnemyFireWizard : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public Transform leftPoint;
    public Transform rightPoint;
    public float idleWaitTime = 3f; // Thời gian đứng im
    private bool movingRight = true;
    private bool isIdle = false;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("Fireball Settings")]
    public Transform firePoint;
    public GameObject fireballPrefab;
    public float fireballCooldown = 10f;
    private float lastFireballTime;

    [Header("Detection")]
    public float detectionRange = 8f;

    [Header("References")]
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public EnemyAttackHitbox attackHitbox1;
    public EnemyAttackHitbox attackHitbox2;

    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        lastFireballTime = Time.time - fireballCooldown;

        // 🔹 Bỏ qua va chạm với các enemy khác và hitbox của chúng
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

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange && distance <= detectionRange)
        {
            // Player trong tầm tấn công → Attack
            rb.velocity = Vector2.zero;
            TryAttack();
        }
        else if (distance <= detectionRange)
        {
            // Player trong tầm phát hiện → Chase
            MoveTowardsPlayerWithinBounds();
            if (Time.time >= lastFireballTime + fireballCooldown)
            {
                rb.velocity = Vector2.zero;
                TryFire();
                lastFireballTime = Time.time;
            }
        }
        else
        {
            // Player ra khỏi phạm vi phát hiện → Patrol
            Patrol();
        }

        UpdateFacing();
    }

    void Patrol()
    {
        if (isIdle) return;

        animator.SetBool("isRunning", true);

        float targetX = movingRight ? rightPoint.position.x : leftPoint.position.x;
        float step = moveSpeed * Time.deltaTime * (movingRight ? 1 : -1);
        transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, targetX, Mathf.Abs(step)), transform.position.y, transform.position.z);

        if ((movingRight && transform.position.x >= targetX) || (!movingRight && transform.position.x <= targetX))
        {
            StartCoroutine(IdleAndTurn(!movingRight));
        }
        else
        {
            if (Random.value < 0.002f)
            {
                StartCoroutine(IdleRandom());
            }
        }
    }

    IEnumerator IdleAndTurn(bool nextMoveRight)
    {
        isIdle = true;
        rb.velocity = Vector2.zero;
        animator.SetBool("isRunning", false);
        yield return new WaitForSeconds(idleWaitTime);
        movingRight = nextMoveRight;
        isIdle = false;
    }

    IEnumerator IdleRandom()
    {
        isIdle = true;
        rb.velocity = Vector2.zero;
        animator.SetBool("isRunning", false);
        yield return new WaitForSeconds(idleWaitTime);
        isIdle = false;
    }

    void MoveTowardsPlayerWithinBounds()
    {
        float targetX = Mathf.Clamp(player.position.x, leftPoint.position.x, rightPoint.position.x);
        float step = moveSpeed * Time.deltaTime * (targetX > transform.position.x ? 1 : -1);
        transform.position = new Vector3(Mathf.MoveTowards(transform.position.x, targetX, Mathf.Abs(step)), transform.position.y, transform.position.z);
        animator.SetBool("isRunning", true);
    }

    void UpdateFacing()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            transform.eulerAngles = new Vector3(0, player.position.x > transform.position.x ? 0f : 180f, 0f);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, movingRight ? 0f : 180f, 0f);
        }
    }

    void TryAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    void TryFire()
    {
        animator.SetTrigger("Fire");
    }

    public void TriggerAttackHitbox1()
    {
        if (attackHitbox1 != null) attackHitbox1.ActivateHitbox();
    }

    public void TriggerAttackHitbox2()
    {
        if (attackHitbox2 != null) attackHitbox2.ActivateHitbox();
    }

    public void ShootFireball()
    {
        if (fireballPrefab != null && firePoint != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
            Vector2 direction = (player.position.x < transform.position.x) ? Vector2.left : Vector2.right;
            fireball.GetComponent<Fireball>().SetDirection(direction);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;
        Die();
    }

    void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Death");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
}
