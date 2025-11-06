using UnityEngine;

public class EnemyFireWizard : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 8f;
    public float attackRange = 2f;
    public LayerMask playerLayer;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("References")]
    public Transform firePoint;
    public GameObject fireballPrefab;

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public EnemyAttackHitbox attackHitbox1;
    public EnemyAttackHitbox attackHitbox2;
    public Transform attackHitboxTransform1; // Gán cho AttackHitbox1
    public Transform attackHitboxTransform2; // Gán cho AttackHitbox2

    public Vector3 firePointOffsetRight = new Vector3(1f, 0f, 0f);
    public Vector3 firePointOffsetLeft = new Vector3(-1f, 0f, 0f);

    [Header("Fireball Settings")]
    public float fireballCooldown = 10f;
    private float lastFireballTime;

    private bool isFiring = false;

    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        lastFireballTime = Time.time - fireballCooldown; // Cho phép bắn ngay lần đầu
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            if (distance > attackRange)
            {
                MoveTowardsPlayer();
                animator.SetBool("isRunning", true);

                // Chỉ bắn nếu đủ thời gian cooldown
                if (Time.time >= lastFireballTime + fireballCooldown)
                {
                    rb.velocity = Vector2.zero;
                    animator.SetBool("isRunning", false);
                    TryFire();
                    lastFireballTime = Time.time;
                }
            }
            else
            {
                rb.velocity = Vector2.zero;
                animator.SetBool("isRunning", false);
                TryAttack();
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isRunning", false);
        }

        // Lật hướng enemy theo vị trí Player
        if (player.position.x < transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        // Cập nhật vị trí hitbox theo hướng
        if (spriteRenderer.flipX)
        {
            attackHitboxTransform1.localPosition = new Vector3(-2f, 0f, 0f); // trái
            attackHitboxTransform2.localPosition = new Vector3(-3f, 0f, 0f); // trái
            firePoint.localPosition = new Vector3(-1f, 0f, 0f); // trái
        }
        else
        {
            attackHitboxTransform1.localPosition = new Vector3(1f, 0f, 0f); // phải
            attackHitboxTransform2.localPosition = new Vector3(1f, 0f, 0f); // phải
            firePoint.localPosition = new Vector3(1f, 0f, 0f); // phải
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
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
        if (attackHitbox1 != null)
        {
            attackHitbox1.ActivateHitbox();

            Collider2D[] hits = Physics2D.OverlapBoxAll(attackHitboxTransform1.position, new Vector2(1f, 1f), 0f, playerLayer);
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    PlayerController playerController = hit.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.ApplyStun(2f); // Gây choáng 2 giây
                    }
                }
            }
        }
    }

    public void TriggerAttackHitbox2()
    {
        if (attackHitbox2 != null)
        {
            attackHitbox2.ActivateHitbox();

            Collider2D[] hits = Physics2D.OverlapBoxAll(attackHitboxTransform2.position, new Vector2(1f, 1f), 0f, playerLayer);
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    PlayerController playerController = hit.GetComponent<PlayerController>();
                    if (playerController != null)
                    {
                        playerController.ApplyStun(2f); // Gây choáng 2 giây
                    }
                }
            }
        }
    }



    // Gọi từ animation event
    public void ShootFireball()
    {
        if (fireballPrefab != null && firePoint != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

            // Xác định hướng bay dựa vào flipX
            Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;

            fireball.GetComponent<Fireball>().SetDirection(direction);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        // Giả sử có hệ thống máu, ở đây ta chỉ xử lý chết
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