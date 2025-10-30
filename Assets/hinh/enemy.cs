using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Di chuyển")]
    public float moveSpeed = 2f;
    public float chaseRange = 5f;
    public float stopDistance = 1f;

    [Header("Tấn công")]
    public float attackRange = 1.5f;
    public int attackDamage = 10;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    [Header("Phát hiện")]
    public LayerMask playerLayer;
    private Transform detectedPlayer;
    private bool isChasing = false;

    private Rigidbody2D rb;
    private Animator animator;
    private bool facingRight = true;

    private bool isRetreating = false;
    public float retreatDuration = 1f;
    private Vector2 retreatDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        }
    }

    void Update()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, chaseRange, playerLayer);
        detectedPlayer = playerCollider != null ? playerCollider.transform : null;

        if (detectedPlayer == null) return;

        if (isRetreating)
        {
            Retreat();
            return;
        }

        float distanceX = Mathf.Abs(transform.position.x - detectedPlayer.position.x);
        float distanceY = Mathf.Abs(transform.position.y - detectedPlayer.position.y);
        float heightTolerance = 2f;

        if (distanceX <= chaseRange && distanceX > stopDistance && distanceY <= heightTolerance)
        {
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            isChasing = false;
            StopChasing();
        }

        if (distanceX <= attackRange && Time.time >= lastAttackTime + attackCooldown && distanceY <= heightTolerance)
        {
            Attack();
            lastAttackTime = Time.time;
        }

        UpdateAnimation();
    }

    void ChasePlayer()
    {
        if (detectedPlayer == null) return;

        Vector2 direction = new Vector2(detectedPlayer.position.x - transform.position.x, 0).normalized;
        Vector2 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(new Vector2(newPosition.x, rb.position.y));

        if (direction.x > 0 && !facingRight) Flip();
        else if (direction.x < 0 && facingRight) Flip();
    }

    void StopChasing()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    void Retreat()
    {
        Vector2 newPosition = rb.position + retreatDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(new Vector2(newPosition.x, rb.position.y));

        if (retreatDirection.x > 0 && !facingRight) Flip();
        else if (retreatDirection.x < 0 && facingRight) Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void Attack()
    {
        if (detectedPlayer == null) return;

        PlayerHealth playerHealth = detectedPlayer.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
            Debug.Log("Enemy tấn công! Gây " + attackDamage + " sát thương.");
        }

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("IsMoving", isChasing);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log("Enemy va chạm và gây " + attackDamage + " sát thương!");
            }

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            Vector2 directionAway = (transform.position - collision.transform.position).normalized;
            retreatDirection = new Vector2(directionAway.x, 0);
            StartCoroutine(RetreatCoroutine());
        }
    }

    IEnumerator RetreatCoroutine()
    {
        isRetreating = true;
        isChasing = false;
        yield return new WaitForSeconds(retreatDuration);
        isRetreating = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(chaseRange * 2, 2f, 0));
    }
}