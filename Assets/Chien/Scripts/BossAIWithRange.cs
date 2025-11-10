using UnityEngine;

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
    private Vector3 startPosition;
    private bool isChasing = false;
    private bool isReturning = false;
    private bool facingRight = true;

    [Header("Attack Variables")]
    private bool isAttacking = false;
    private float attackCooldown = 1.2f;
    private float attackTimer = 0f;
    private float attack3Interval = 4f;
    private float attack3Timer = 0f;
    private bool nextAttack3 = false;

    [Header("Attack Damage")]
    public int attackDamage = 40;
    public int attack3Damage = 50;

    [Header("Audio")]
    public AudioClip slashSound;   // âm thanh chém
    private AudioSource audioSource;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>(); // cần AudioSource trên boss
        startPosition = transform.position;
        anim.SetBool("isRunning", false);
    }

    void Update()
    {
        if (player == null || leftPoint == null || rightPoint == null)
            return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool playerInRange = player.position.x > leftPoint.position.x && player.position.x < rightPoint.position.x;

        // ===== Chase Logic =====
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

        // ===== Attack Timers =====
        attackTimer -= Time.deltaTime;
        attack3Timer += Time.deltaTime;

        if (isChasing)
            ChasePlayer(distanceToPlayer);
        else if (isReturning)
            ReturnToStart();
        else
            Idle();
    }

    void ChasePlayer(float distanceToPlayer)
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        FlipSprite(direction.x);

        if (distanceToPlayer <= attackRadius)
            Attack();
        else
        {
            anim.SetBool("isRunning", true);
            anim.SetBool("isAttacking", false);
        }
    }

    void Attack()
    {
        if (attackTimer <= 0f)
        {
            isAttacking = true;
            rb.velocity = Vector2.zero;
            anim.SetBool("isAttacking", true);
            anim.SetBool("isRunning", false);

            // Kiểm tra khoảng cách trước khi gây damage
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            bool playerInAttackRange = distanceToPlayer <= attackRadius;

            if (nextAttack3)
            {
                anim.SetTrigger("Attack3");
                if (playerInAttackRange) DealDamage(attack3Damage);
                PlaySlashSound(); // phát âm thanh
                nextAttack3 = false;
            }
            else
            {
                anim.SetTrigger("Attack");
                if (playerInAttackRange) DealDamage(attackDamage);
                PlaySlashSound(); // phát âm thanh
            }

            attackTimer = attackCooldown;

            if (attack3Timer >= attack3Interval)
            {
                nextAttack3 = true;
                attack3Timer = 0f;
            }
        }
    }

    void DealDamage(int damageAmount)
    {
        if (player != null)
        {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damageAmount);
        }
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

        Vector2 direction = (startPosition - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * returnSpeed, rb.velocity.y);
        FlipSprite(direction.x);

        if (Vector2.Distance(transform.position, startPosition) <= stopDistance)
        {
            rb.velocity = Vector2.zero;
            isReturning = false;
            Idle();
        }
    }

    void Idle()
    {
        anim.SetBool("isRunning", false);
        anim.SetBool("isAttacking", false);
        rb.velocity = Vector2.zero;
        isAttacking = false;
    }

    public void EndAttack()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", false);
    }

    void FlipSprite(float moveX)
    {
        if (moveX > 0 && !facingRight)
            Flip();
        else if (moveX < 0 && facingRight)
            Flip();
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
