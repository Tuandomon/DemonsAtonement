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
    public AudioClip attackSound;

    [Header("References")]
    public Animator animator;
    public Transform player;

    [Header("Distance Settings")]
    public float tooCloseDistance = 0.4f;      // Nếu player quá gần → lùi
    public float idealAttackDistance = 1.2f;   // Khoảng cách lý tưởng tấn công
    public float backstepDistance = 1f;        // Lùi bao nhiêu
    public float backstepSpeed = 4f;           // Tốc độ lùi
    private bool isBackingUp = false;

    private Rigidbody2D rb;
    private AudioSource audioSource;

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
        animator = animator != null ? animator : GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        nextHowlTime = Time.time + Random.Range(minHowlDelay, maxHowlDelay);

        if (leftPoint != null && rightPoint != null)
        {
            float mid = (leftPoint.position.x + rightPoint.position.x) * 0.5f;
            moveDirection = transform.position.x < mid ? 1 : -1;
            if ((moveDirection < 0 && facingRight) || (moveDirection > 0 && !facingRight))
                Flip();
        }

        // Ignore collision giữa các enemy khác và các sói
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
            currentState = WolfState.Attack;
        else if (distanceToPlayer <= detectRange)
            currentState = WolfState.Chase;
        else
            currentState = WolfState.Patrol;

        switch (currentState)
        {
            case WolfState.Patrol: PatrolState(); break;
            case WolfState.Chase: ChaseState(); break;
            case WolfState.Attack: AttackState(); break;
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

        UpdateFacingDirection();

        if ((moveDirection > 0 && transform.position.x >= rightPoint.position.x) ||
            (moveDirection < 0 && transform.position.x <= leftPoint.position.x))
        {
            StartCoroutine(WaitAndTurn());
        }
    }

    void ChaseState()
    {
        if (isHowling || isBackingUp) return;

        float distance = Vector2.Distance(player.position, transform.position);
        float dir = Mathf.Sign(player.position.x - transform.position.x);

        // Player quá gần → lùi
        if (distance < tooCloseDistance)
        {
            if (!isBackingUp)
                StartCoroutine(BackstepRoutine(-dir));
            return;
        }

        // Nếu xa hơn khoảng cách tấn công → chạy lại
        if (distance > idealAttackDistance)
        {
            rb.velocity = new Vector2(dir * chaseSpeed, rb.velocity.y);
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        }
        else
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
        }

        UpdateFacingDirection();
    }

    void AttackState()
    {
        float distance = Vector2.Distance(player.position, transform.position);
        float dir = Mathf.Sign(player.position.x - transform.position.x);

        // Player quá gần → lùi
        if (distance < tooCloseDistance && !isBackingUp)
        {
            StartCoroutine(BackstepRoutine(-dir));
        }

        // Nếu player trong range tấn công → attack animation
        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack"); // animation Sói tấn công
            PlayAttackSound();

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(100);
                Debug.Log("🐺 Sói tấn công trúng người chơi!");
            }

            rb.velocity = Vector2.zero; // đứng yên khi attack
            return; // không di chuyển khi attack
        }

        // Di chuyển tiến lại nếu xa hơn khoảng cách lý tưởng
        if (distance > idealAttackDistance && !isBackingUp)
        {
            rb.velocity = new Vector2(dir * chaseSpeed, rb.velocity.y);
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x)); // animation Sói di chuyển
        }
        else if (!isBackingUp)
        {
            rb.velocity = Vector2.zero;
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                animator.SetFloat("Speed", 0);
        }

        UpdateFacingDirection();
    }

    IEnumerator BackstepRoutine(float dir)
    {
        isBackingUp = true;
        animator.SetFloat("Speed", backstepSpeed); // animation Sói di chuyển khi lùi

        float startX = transform.position.x;
        float targetX = startX + dir * backstepDistance;
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * backstepSpeed;
            float newX = Mathf.Lerp(startX, targetX, t);
            rb.MovePosition(new Vector2(newX, transform.position.y));
            yield return null;
        }

        animator.SetFloat("Speed", 0);
        isBackingUp = false;
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

    void UpdateFacingDirection()
    {
        float vx = rb.velocity.x;
        if (vx > 0 && !facingRight)
            Flip();
        else if (vx < 0 && facingRight)
            Flip();
    }

    public void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null)
            audioSource.PlayOneShot(attackSound, 0.7f);
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
