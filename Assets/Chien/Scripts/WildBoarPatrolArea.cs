using UnityEngine;
using System.Collections;

public class WildBoarPatrolArea : MonoBehaviour
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

    [Header("References")]
    public Animator animator;
    public Transform player;

    private Rigidbody2D rb;
    private bool isWaiting = false;
    private bool facingRight = true;
    private int moveDirection = 1;
    private float lastAttackTime = 0f;

    private enum BoarState { Patrol, Chase, Attack }
    private BoarState currentState = BoarState.Patrol;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();

        if (leftPoint != null && rightPoint != null)
        {
            float mid = (leftPoint.position.x + rightPoint.position.x) * 0.5f;
            moveDirection = (transform.position.x < mid) ? 1 : -1;
            SetRotationByDirection(moveDirection);
        }

        // Bỏ qua va chạm giữa các heo rừng và enemy khác
        Collider2D myCol = GetComponent<Collider2D>();
        if (myCol != null)
        {
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            GameObject[] allWolf = GameObject.FindGameObjectsWithTag("Enemy_Wolf");

            foreach (GameObject e in allEnemies)
            {
                if (e == gameObject) continue;
                Collider2D col = e.GetComponent<Collider2D>();
                if (col != null) Physics2D.IgnoreCollision(myCol, col);
            }

            foreach (GameObject e in allWolf)
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

        // Xác định trạng thái ưu tiên
        if (distanceToPlayer <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            currentState = BoarState.Attack;
        }
        else if (distanceToPlayer <= detectRange)
        {
            currentState = BoarState.Chase;
        }
        else
        {
            currentState = BoarState.Patrol;
        }

        // Gọi hành vi theo trạng thái
        switch (currentState)
        {
            case BoarState.Patrol:
                PatrolState();
                break;
            case BoarState.Chase:
                ChaseState();
                break;
            case BoarState.Attack:
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

    IEnumerator WaitAndTurn()
    {
        isWaiting = true;
        rb.velocity = Vector2.zero;
        animator.SetFloat("Speed", 0);
        yield return new WaitForSeconds(waitTime);
        moveDirection *= -1;
        SetRotationByDirection(moveDirection);
        isWaiting = false;
    }

    void ChaseState()
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        rb.velocity = new Vector2(dir * chaseSpeed, rb.velocity.y);

        SetRotationByDirection(dir);

        animator.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }

    void AttackState()
    {
        rb.velocity = Vector2.zero;
        animator.SetFloat("Speed", 0);

        float dir = Mathf.Sign(player.position.x - transform.position.x);
        SetRotationByDirection(dir);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            if (distanceToPlayer <= attackRange)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(50); // Sát thương heo rừng
                    Debug.Log("🐗 Heo rừng tấn công trúng người chơi!");
                }
            }
        }
    }

    void SetRotationByDirection(float dir)
    {
        if (dir > 0)
            transform.eulerAngles = new Vector3(0, 180, 0); // Sang phải
        else
            transform.eulerAngles = new Vector3(0, 0, 0);   // Sang trái
        facingRight = dir > 0;
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
