using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorpionPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Patrol Settings")]
    public float moveSpeed = 2f;
    public float waitTime = 2f;

    [Header("Attack Settings")]
    public float chaseSpeed = 4f;
    public float detectRange = 6f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;

    [Header("References")]
    public Animator animator;
    public Transform player;
    public AudioClip attackSound;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private bool isWaiting = false;

    // ⭐ Sprite mặc định quay TRÁI
    private bool facingRight = false;

    private int moveDirection = 1;
    private float lastAttackTime = 0f;

    private enum ScorpionState { Patrol, Chase, Attack }
    private ScorpionState currentState = ScorpionState.Patrol;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        if (animator == null) animator = GetComponent<Animator>();

        // Sprite mặc định quay trái
        facingRight = false;

        if (leftPoint != null && rightPoint != null)
        {
            float mid = (leftPoint.position.x + rightPoint.position.x) * 0.5f;
            moveDirection = (transform.position.x < mid) ? 1 : -1;

            if ((moveDirection > 0 && !facingRight) || (moveDirection < 0 && facingRight))
                Flip();
        }

        Collider2D myCol = GetComponent<Collider2D>();
        if (myCol != null)
        {
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject[] allWolfs = GameObject.FindGameObjectsWithTag("Enemy_Wolf");

            foreach (GameObject e in allEnemies)
            {
                if (e == gameObject) continue;
                Collider2D col = e.GetComponent<Collider2D>();
                if (col != null) Physics2D.IgnoreCollision(myCol, col);
            }

            foreach (GameObject e in allWolfs)
            {
                if (e == gameObject) continue;
                Collider2D col = e.GetComponent<Collider2D>();
                if (col != null) Physics2D.IgnoreCollision(myCol, col);
            }
        }
    }

    void Update()
    {
        if (player == null || leftPoint == null || rightPoint == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool playerWithinPatrol = player.position.x >= leftPoint.position.x && player.position.x <= rightPoint.position.x;

        if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackCooldown)
            currentState = ScorpionState.Attack;
        else if (distanceToPlayer <= detectRange && playerWithinPatrol)
            currentState = ScorpionState.Chase;
        else
            currentState = ScorpionState.Patrol;

        switch (currentState)
        {
            case ScorpionState.Patrol:
                PatrolState();
                break;
            case ScorpionState.Chase:
                ChaseState();
                break;
            case ScorpionState.Attack:
                AttackState();
                break;
        }
    }

    void PatrolState()
    {
        if (isWaiting)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
            return;
        }

        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        if ((moveDirection > 0 && !facingRight) || (moveDirection < 0 && facingRight))
            Flip();

        if (moveDirection > 0 && transform.position.x >= rightPoint.position.x)
            StartCoroutine(WaitAndTurn());
        else if (moveDirection < 0 && transform.position.x <= leftPoint.position.x)
            StartCoroutine(WaitAndTurn());
    }

    void ChaseState()
    {
        if (player.position.x < leftPoint.position.x || player.position.x > rightPoint.position.x)
        {
            currentState = ScorpionState.Patrol;
            return;
        }

        float dir = Mathf.Sign(player.position.x - transform.position.x);
        rb.velocity = new Vector2(dir * chaseSpeed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        if ((dir > 0 && !facingRight) || (dir < 0 && facingRight))
            Flip();
    }

    void AttackState()
    {
        // Đứng yên
        rb.velocity = Vector2.zero;
        animator.SetFloat("Speed", 0);

        // Xoay mặt theo player
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        if ((dir > 0 && !facingRight) || (dir < 0 && facingRight))
            Flip();

        // Nếu còn cooldown → KHÔNG chạy tiếp attack
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        // ⭐ CHẠY ANIMATION TẤN CÔNG MỖI KHI ĐỦ COOLDOWN
        animator.SetTrigger("Attack");

        lastAttackTime = Time.time;

        // ⭐ Gây damage nếu player còn trong phạm vi
        if (Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(50);

                // Âm thanh tấn công
                if (audioSource != null && attackSound != null)
                    audioSource.PlayOneShot(attackSound);
            }
        }
    }


    IEnumerator WaitAndTurn()
    {
        isWaiting = true;
        rb.velocity = Vector2.zero;
        animator.SetFloat("Speed", 0);
        yield return new WaitForSeconds(waitTime);
        moveDirection *= -1;
        Flip();
        isWaiting = false;
    }

    // ⭐ Sprite gốc quay TRÁI → scale.x dương = trái  
    // Khi quay phải → scale.x âm
    void Flip()
    {
        facingRight = !facingRight;

        Vector3 s = transform.localScale;
        s.x = Mathf.Abs(s.x) * (facingRight ? -1 : 1);
        transform.localScale = s;
    }

    void OnDrawGizmosSelected()
    {
        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftPoint.position, rightPoint.position);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
