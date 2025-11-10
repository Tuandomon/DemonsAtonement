using UnityEngine;
using System.Collections;

public class WolfPatrolArea : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Patrol Settings")]
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public float minHowlDelay = 6f;
    public float maxHowlDelay = 12f;
    public float howlDuration = 1.1f;

    [Header("Attack Settings")]
    public float chaseSpeed = 3.5f;
    public float detectRange = 6f;
    public float attackRange = 1.2f;
    public float attackCooldown = 0.4f;

    [Header("Audio Settings")]
    public AudioClip attackSound;    // âm thanh tấn công
    private AudioSource audioSource;

    [Header("References")]
    public Animator animator;
    public Transform player;

    private Rigidbody2D rb;
    private bool isWaiting = false;
    private bool isHowling = false;
    private bool facingRight = true;
    private int moveDirection = 1;
    private float nextHowlTime;
    private float lastAttackTime = 0f;

    private enum WolfState { Patrol, Chase, Attack }
    private WolfState currentState = WolfState.Patrol;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        nextHowlTime = Time.time + Random.Range(minHowlDelay, maxHowlDelay);

        if (leftPoint != null && rightPoint != null)
        {
            float mid = (leftPoint.position.x + rightPoint.position.x) * 0.5f;
            moveDirection = (transform.position.x < mid) ? 1 : -1;
            if (moveDirection < 0 && facingRight) Flip();
            if (moveDirection > 0 && !facingRight) Flip();
        }

        // Bỏ qua va chạm giữa các sói và enemy khác
        Collider2D myCol = GetComponent<Collider2D>();
        if (myCol != null)
        {
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject[] allWolves = GameObject.FindGameObjectsWithTag("Enemy_Wolf");

            foreach (GameObject e in allEnemies)
            {
                if (e == gameObject) continue;
                Collider2D col = e.GetComponent<Collider2D>();
                if (col != null) Physics2D.IgnoreCollision(myCol, col);
            }

            foreach (GameObject e in allWolves)
            {
                if (e == gameObject) continue;
                Collider2D col = e.GetComponent<Collider2D>();
                if (col != null) Physics2D.IgnoreCollision(myCol, col);
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (isHowling && distanceToPlayer <= detectRange)
        {
            StopCoroutine("HowlRoutine");
            animator.SetBool("IsHowling", false);
            isHowling = false;
        }

        if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            currentState = WolfState.Attack;
        }
        else if (distanceToPlayer <= detectRange)
        {
            currentState = WolfState.Chase;
        }
        else
        {
            currentState = WolfState.Patrol;
        }

        switch (currentState)
        {
            case WolfState.Patrol:
                PatrolState();
                break;
            case WolfState.Chase:
                ChaseState();
                break;
            case WolfState.Attack:
                AttackState();
                break;
        }
    }

    void PatrolState()
    {
        if (isWaiting || isHowling)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
            return;
        }

        if (Time.time >= nextHowlTime)
        {
            StartCoroutine(HowlRoutine());
            nextHowlTime = Time.time + Random.Range(minHowlDelay, maxHowlDelay);
        }

        if (leftPoint == null || rightPoint == null) return;

        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        if (moveDirection > 0 && transform.position.x >= rightPoint.position.x)
        {
            StartCoroutine(WaitAndTurn());
        }
        else if (moveDirection < 0 && transform.position.x <= leftPoint.position.x)
        {
            StartCoroutine(WaitAndTurn());
        }
    }

    void ChaseState()
    {
        if (isHowling) return;

        float dir = Mathf.Sign(player.position.x - transform.position.x);
        rb.velocity = new Vector2(dir * chaseSpeed, rb.velocity.y);
        if ((dir > 0 && !facingRight) || (dir < 0 && facingRight))
            Flip();

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        animator.SetBool("IsHowling", false);
    }

    void AttackState()
    {
        rb.velocity = Vector2.zero;
        animator.SetFloat("Speed", 0);

        float dir = Mathf.Sign(player.position.x - transform.position.x);
        if ((dir > 0 && !facingRight) || (dir < 0 && facingRight))
            Flip();

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;

            // 🔥 Gây sát thương trực tiếp nếu người chơi trong phạm vi tấn công
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(100);
                    Debug.Log("🐺 Sói tấn công trúng người chơi!");
                }
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

    IEnumerator HowlRoutine()
    {
        isHowling = true;
        rb.velocity = Vector2.zero;
        animator.SetFloat("Speed", 0);
        animator.SetBool("IsHowling", true);

        // 🔊 Xóa âm thanh hú ở đây
        yield return new WaitForSeconds(howlDuration);

        animator.SetBool("IsHowling", false);
        isHowling = false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    // 🔊 Hàm này sẽ được gọi từ Animation Event trong clip Attack
    public void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound, 0.7f);
        }
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
