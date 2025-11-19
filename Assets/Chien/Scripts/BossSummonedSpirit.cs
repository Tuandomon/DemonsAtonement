using UnityEngine;
using System.Collections;

public class BossSummonedSpirit : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Patrol Settings")]
    public float moveSpeed = 2f;
    public float waitTime = 2f;

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
    public float tooCloseDistance = 0.4f;
    public float idealAttackDistance = 1.2f;
    public float backstepDistance = 1f;
    public float backstepSpeed = 4f;
    private bool isBackingUp = false;

    private Rigidbody2D rb;
    private AudioSource audioSource;

    private bool isWaiting = false;
    private bool facingRight = true;
    private int moveDirection = 1;
    private float lastAttackTime = 0f;

    private enum SpiritState { Patrol, Chase, Attack }
    private SpiritState currentState = SpiritState.Patrol;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = animator != null ? animator : GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (leftPoint != null && rightPoint != null)
        {
            float mid = (leftPoint.position.x + rightPoint.position.x) * 0.5f;
            moveDirection = transform.position.x < mid ? 1 : -1;
        }

        // Ignore collision giữa các enemy và Boss
        Collider2D myCol = GetComponent<Collider2D>();
        if (myCol != null)
        {
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject e in allEnemies)
            {
                if (e == gameObject) continue;
                Collider2D col = e.GetComponent<Collider2D>();
                if (col != null) Physics2D.IgnoreCollision(myCol, col);
            }

            GameObject[] allBosses = GameObject.FindGameObjectsWithTag("Boss");
            foreach (GameObject b in allBosses)
            {
                if (b == gameObject) continue;
                Collider2D col = b.GetComponent<Collider2D>();
                if (col != null) Physics2D.IgnoreCollision(myCol, col);
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackRange && Time.time - lastAttackTime >= attackCooldown)
            currentState = SpiritState.Attack;
        else if (dist <= detectRange)
            currentState = SpiritState.Chase;
        else
            currentState = SpiritState.Patrol;

        switch (currentState)
        {
            case SpiritState.Patrol: PatrolState(); break;
            case SpiritState.Chase: ChaseState(); break;
            case SpiritState.Attack: AttackState(); break;
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

        if (leftPoint == null || rightPoint == null) return;

        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);
        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        UpdateFacingDirection(rb.velocity.x);

        if ((moveDirection > 0 && transform.position.x >= rightPoint.position.x) ||
            (moveDirection < 0 && transform.position.x <= leftPoint.position.x))
        {
            StartCoroutine(WaitAndTurn());
        }
    }

    void ChaseState()
    {
        if (isBackingUp) return;

        float distance = Vector2.Distance(transform.position, player.position);
        float dir = Mathf.Sign(player.position.x - transform.position.x);

        if (distance < tooCloseDistance)
        {
            if (!isBackingUp)
                StartCoroutine(BackstepRoutine(-dir));
            return;
        }

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

        UpdateFacingDirection(rb.velocity.x);
    }

    void AttackState()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        float dir = Mathf.Sign(player.position.x - transform.position.x);

        // Backstep nếu quá gần
        if (distance < tooCloseDistance && !isBackingUp)
        {
            StartCoroutine(BackstepRoutine(-dir));
        }

        // Kiểm tra cooldown trước khi attack
        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;       // ⭐ cập nhật cooldown
            animator.SetTrigger("Attack");    // chạy animation attack
            rb.velocity = Vector2.zero;       // đứng yên khi đánh
            return;
        }

        // Di chuyển tới hoặc đứng yên nếu ở khoảng idealAttackDistance
        if (distance > idealAttackDistance && !isBackingUp)
        {
            rb.velocity = new Vector2(dir * chaseSpeed, rb.velocity.y);
            animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        }
        else if (!isBackingUp)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0);
        }

        UpdateFacingDirection(rb.velocity.x);
    }

    // ---------- Animation Event sẽ gọi hàm này ----------
    public void DealDamage()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackRange)
        {
            PlayerHealth hp = player.GetComponent<PlayerHealth>();
            if (hp != null) hp.TakeDamage(100);

            PlayAttackSound();
        }
    }



    IEnumerator BackstepRoutine(float dir)
    {
        isBackingUp = true;
        animator.SetFloat("Speed", backstepSpeed);

        float startX = transform.position.x;
        float target = startX + dir * backstepDistance;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * backstepSpeed;
            float newX = Mathf.Lerp(startX, target, t);
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
        isWaiting = false;
    }

    void UpdateFacingDirection(float moveDir)
    {
        if (moveDir > 0.05f) transform.rotation = Quaternion.Euler(0, 180f, 0);
        else if (moveDir < -0.05f) transform.rotation = Quaternion.Euler(0, 0f, 0);
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
