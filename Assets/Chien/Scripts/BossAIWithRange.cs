using UnityEngine;
using System.Collections;

public class BossAI_RangeAndCircle : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    public float detectionRadius = 5f;
    public float attackRadius = 1.5f;
    public float moveSpeed = 2f;
    public float returnSpeed = 2f;
    public float stopDistance = 0.2f;

    [Header("Movement Range")]
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Components")]
    private Animator anim;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Vector3 startPosition;

    private bool isChasing = false;
    private bool isReturning = false;
    private bool facingRight = true;
    private bool isAttacking = false;

    [Header("Attack Variables")]
    private float attackCooldown = 1.2f;
    private float attackTimer = 0f;
    private float attack3Interval = 4f;
    private float attack3Timer = 0f;
    private bool nextAttack3 = false;

    [Header("Attack Damage")]
    public int attackDamage = 40;
    public int attack3Damage = 50;

    [Header("Audio")]
    public AudioClip slashSound;

    private EnemyHealth enemyHealth;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        enemyHealth = GetComponent<EnemyHealth>();

        startPosition = transform.position;
        anim.SetBool("isRunning", false);
    }

    void Update()
    {
        // ✅ Dừng mọi hành động ngay khi HP <= 0
        if (enemyHealth != null && enemyHealth.GetCurrentHealth() <= 0)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("isRunning", false);
            anim.SetBool("isAttacking", false);
            return; // bỏ qua chase/attack/return
        }

        if (player == null || leftPoint == null || rightPoint == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        bool playerInRange =
            player.position.x > leftPoint.position.x &&
            player.position.x < rightPoint.position.x;

        attackTimer -= Time.deltaTime;
        attack3Timer += Time.deltaTime;

        if (isAttacking)
        {
            // Trong lúc attack vẫn được phép xoay
            FlipSprite(player.position.x - transform.position.x);
            return;
        }

        if (playerInRange && distanceToPlayer <= detectionRadius)
        {
            isChasing = true;
            isReturning = false;
        }
        else if (playerInRange && isChasing)
        {
            isChasing = true;
            isReturning = false;
        }
        else if (!playerInRange && isChasing)
        {
            isChasing = false;
            isReturning = true;
        }

        if (isChasing)
            ChasePlayer(distanceToPlayer);
        else if (isReturning)
            ReturnToStart();
        else
            Idle();
    }

    void ChasePlayer(float distanceToPlayer)
    {
        Vector2 dir = (player.position - transform.position).normalized;

        anim.SetBool("isRunning", true);
        anim.SetBool("isAttacking", false);

        if (distanceToPlayer > attackRadius)
        {
            rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
            FlipSprite(dir.x);
            return;
        }

        rb.velocity = Vector2.zero;
        TryAttack();
    }

    void TryAttack()
    {
        if (attackTimer > 0f) return;

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        anim.SetBool("isRunning", false);
        anim.SetBool("isAttacking", true);

        bool useAttack3 = nextAttack3;
        if (useAttack3) nextAttack3 = false;

        anim.SetTrigger(useAttack3 ? "Attack3" : "Attack");
        PlaySlashSound();

        float attackAnimDuration = 1f; // chỉnh theo Animator
        float timer = 0f;
        bool damageDealt = false;

        while (timer < attackAnimDuration)
        {
            timer += Time.deltaTime;

            // ✅ Nếu boss chết giữa chừng thì dừng coroutine
            if (enemyHealth != null && enemyHealth.GetCurrentHealth() <= 0)
            {
                rb.velocity = Vector2.zero;
                anim.SetBool("isRunning", false);
                anim.SetBool("isAttacking", false);
                yield break;
            }

            FlipSprite(player.position.x - transform.position.x);

            if (!damageDealt && timer >= attackAnimDuration / 2f)
            {
                float dist = Vector2.Distance(transform.position, player.position);
                if (dist <= attackRadius)
                {
                    DealDamage(useAttack3 ? attack3Damage : attackDamage);
                }
                damageDealt = true;
            }

            yield return null;
        }

        anim.SetBool("isAttacking", false);
        isAttacking = false;

        attackTimer = attackCooldown;
        if (useAttack3)
            attack3Timer = 0f;
        else if (attack3Timer >= attack3Interval)
            nextAttack3 = true;
    }

    void DealDamage(int amount)
    {
        if (player == null) return;

        PlayerHealth hp = player.GetComponent<PlayerHealth>();
        if (hp != null) hp.TakeDamage(amount);
    }

    void PlaySlashSound()
    {
        if (audioSource != null && slashSound != null)
            audioSource.PlayOneShot(slashSound);
    }

    void ReturnToStart()
    {
        anim.SetBool("isRunning", true);
        anim.SetBool("isAttacking", false);

        Vector2 dir = (startPosition - transform.position).normalized;

        rb.velocity = new Vector2(dir.x * returnSpeed, rb.velocity.y);
        FlipSprite(dir.x);

        if (Vector2.Distance(transform.position, startPosition) <= stopDistance)
        {
            rb.velocity = Vector2.zero;
            isReturning = false;
            Idle();
        }
    }

    void Idle()
    {
        rb.velocity = Vector2.zero;

        if (!isChasing && !isAttacking)
            anim.SetBool("isRunning", false);

        anim.SetBool("isAttacking", false);
        isAttacking = false;
    }

    void FlipSprite(float moveX)
    {
        if (moveX > 0 && !facingRight) Flip();
        else if (moveX < 0 && facingRight) Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftPoint.position + Vector3.up, leftPoint.position + Vector3.down);
            Gizmos.DrawLine(rightPoint.position + Vector3.up, rightPoint.position + Vector3.down);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
