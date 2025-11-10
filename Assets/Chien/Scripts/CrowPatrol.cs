using UnityEngine;
using System.Collections;

public class CrowPatrol : MonoBehaviour
{
    [Header("Patrol Range")]
    public Transform leftPoint;
    public Transform rightPoint;
    public float moveSpeed = 2f;
    public float waitTime = 2f;

    [Header("Hover Settings")]
    public float floatAmplitude = 0.2f;
    public float floatFrequency = 2f;

    [Header("Player Detection & Attack")]
    public Transform player;
    public Transform playerCheckPoint;
    public float detectionRange = 6f;
    public float attackRange = 1.5f;
    public float attackSpeed = 4f;
    public float descendSpeed = 2f;
    public float initialHeight = 3f;

    [Header("Attack Settings")]
    public int damage = 20;
    public AudioClip crowAttackSound;
    public float attackCooldown = 3f;       // dừng 3 giây giữa các lần tấn công
    private AudioSource audioSource;
    private float lastAttackTime = -Mathf.Infinity; // thời gian tấn công lần cuối

    [Header("Animation")]
    public Animator animator;

    private bool isWaiting = false;
    private bool facingRight = true;
    private int moveDirection = 1;
    private Vector3 startPos;
    private bool isChasing = false;
    private bool lockYPosition = false;
    private float lockedYPosition;
    private bool returningToPatrol = false;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        startPos = transform.position;
        initialHeight = transform.position.y;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (leftPoint != null && rightPoint != null)
        {
            float mid = (leftPoint.position.x + rightPoint.position.x) * 0.5f;
            moveDirection = (transform.position.x < mid) ? 1 : -1;
            FaceDirection(moveDirection);
        }
    }

    void Update()
    {
        if (returningToPatrol) return;

        if (player == null || playerCheckPoint == null)
        {
            Patrol();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Nếu player ra ngoài patrol → bắt đầu quay về patrol
        if (!IsWithinPatrolBounds())
        {
            if (!returningToPatrol)
            {
                StartCoroutine(ReturnToPatrol());
            }
            return;
        }

        // Nếu player cao hơn hoặc thấp hơn Y đã lock → mở khóa để quạ bay tới Y mới
        if (lockYPosition && Mathf.Abs(playerCheckPoint.position.y - lockedYPosition) > 0.01f)
        {
            lockYPosition = false;
        }

        // Player vào attackRange → tấn công, khóa Y nhưng chỉ attack sau cooldown
        if (distanceToPlayer <= attackRange)
        {
            isChasing = true;
            if (!lockYPosition)
            {
                lockedYPosition = playerCheckPoint.position.y;
                lockYPosition = true;
            }

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                animator?.Play("Qua tan cong");
                lastAttackTime = Time.time; // cập nhật thời gian tấn công mới
                DealDamage();
            }
        }
        // Player trong detectionRange → chase nhưng chưa vào attackRange
        else if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
            if (!lockYPosition)
            {
                ChasePlayer();
            }
            else
            {
                ChasePlayerLockedY();
            }
        }
        else
        {
            isChasing = false;
            lockYPosition = false;
            Patrol();
        }
    }

    void DealDamage()
    {
        var playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }

        if (crowAttackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(crowAttackSound);
        }
    }

    void Patrol()
    {
        if (isWaiting || leftPoint == null || rightPoint == null)
        {
            animator?.SetFloat("Speed", 0);
            return;
        }

        Vector3 move = new Vector3(moveDirection * moveSpeed * Time.deltaTime, 0, 0);
        transform.position += move;

        float newY = initialHeight + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        animator?.Play("Qua di chuyen");
        FaceDirection(moveDirection);

        if (moveDirection > 0 && transform.position.x >= rightPoint.position.x)
            StartCoroutine(WaitAndTurn());
        else if (moveDirection < 0 && transform.position.x <= leftPoint.position.x)
            StartCoroutine(WaitAndTurn());
    }

    void ChasePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += new Vector3(dir.x * attackSpeed * Time.deltaTime, 0, 0);

        float targetY = playerCheckPoint.position.y;
        float newY = Mathf.MoveTowards(transform.position.y, targetY, descendSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        animator?.Play("Qua di chuyen");
        FaceDirection(dir.x);
    }

    void ChasePlayerLockedY()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += new Vector3(dir.x * attackSpeed * Time.deltaTime, 0, 0);

        transform.position = new Vector3(transform.position.x, lockedYPosition, transform.position.z);

        animator?.Play("Qua di chuyen");
        FaceDirection(dir.x);
    }

    IEnumerator ReturnToPatrol()
    {
        returningToPatrol = true;
        isChasing = false;
        lockYPosition = false;

        animator?.SetFloat("Speed", 0);
        yield return new WaitForSeconds(2f);

        while (Mathf.Abs(transform.position.y - initialHeight) > 0.01f)
        {
            float newY = Mathf.MoveTowards(transform.position.y, initialHeight, descendSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            animator?.Play("Qua di chuyen");
            yield return null;
        }

        returningToPatrol = false;
        Patrol();
    }

    bool IsWithinPatrolBounds()
    {
        if (leftPoint == null || rightPoint == null) return true;
        return transform.position.x >= leftPoint.position.x && transform.position.x <= rightPoint.position.x;
    }

    IEnumerator WaitAndTurn()
    {
        isWaiting = true;
        animator?.SetFloat("Speed", 0);
        yield return new WaitForSeconds(waitTime);
        moveDirection *= -1;
        FaceDirection(moveDirection);
        isWaiting = false;
    }

    void FaceDirection(float dirX)
    {
        if (dirX > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            facingRight = true;
        }
        else if (dirX < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            facingRight = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(leftPoint.position, rightPoint.position);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
