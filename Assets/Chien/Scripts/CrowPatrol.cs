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
    public float attackCooldown = 3f;
    private AudioSource audioSource;
    private float lastAttackTime = -Mathf.Infinity;

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
    private float returnSpeed = 3f;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        startPos = transform.position;
        initialHeight = transform.position.y;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        if (leftPoint != null && rightPoint != null)
        {
            float mid = (leftPoint.position.x + rightPoint.position.x) * 0.5f;
            moveDirection = (transform.position.x < mid) ? 1 : -1;
            FaceDirection(moveDirection);
        }
    }

    void Update()
    {
        if (returningToPatrol)
        {
            ReturnToPatrolArea();
            return;
        }

        if (player == null || playerCheckPoint == null)
        {
            PatrolWithHover();
            return;
        }

        // Kiểm tra player nằm trong phạm vi Left-Right
        bool playerInPatrolRange = IsWithinPatrolBounds(player.position.x);
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Nếu player chưa vào phạm vi → chỉ tuần tra
        if (!playerInPatrolRange)
        {
            isChasing = false;
            lockYPosition = false;
            PatrolWithHover();
            return;
        }

        // Player trong attackRange → tấn công
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
                lastAttackTime = Time.time;
                DealDamage();
            }
        }
        // Player trong detectionRange → chase
        else if (distanceToPlayer <= detectionRange)
        {
            isChasing = true;
            if (!lockYPosition)
                ChasePlayer();
            else
                ChasePlayerLockedY();
        }
        else
        {
            isChasing = false;
            lockYPosition = false;
            PatrolWithHover();
        }
    }

    // --- Patrol nhưng vừa di chuyển vừa bay lên xuống ---
    void PatrolWithHover()
    {
        if (isWaiting || leftPoint == null || rightPoint == null)
        {
            animator?.SetFloat("Speed", 0);
            return;
        }

        Vector3 move = new Vector3(moveDirection * moveSpeed * Time.deltaTime, 0, 0);
        transform.position += move;

        float newY = Mathf.MoveTowards(transform.position.y,
            initialHeight + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude,
            descendSpeed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        animator?.Play("Qua di chuyen");
        FaceDirection(moveDirection);

        if (moveDirection > 0 && transform.position.x >= rightPoint.position.x)
            StartCoroutine(WaitAndTurn());
        else if (moveDirection < 0 && transform.position.x <= leftPoint.position.x)
            StartCoroutine(WaitAndTurn());
    }

    void ReturnToPatrolArea()
    {
        float centerX = (leftPoint.position.x + rightPoint.position.x) * 0.5f;

        float newX = Mathf.MoveTowards(transform.position.x, centerX, returnSpeed * Time.deltaTime);

        float targetY = initialHeight + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        float newY = Mathf.MoveTowards(transform.position.y, targetY, descendSpeed * Time.deltaTime);

        transform.position = new Vector3(newX, newY, transform.position.z);

        animator?.Play("Qua di chuyen");
        FaceDirection(centerX - transform.position.x);

        if (Mathf.Abs(transform.position.x - centerX) < 0.01f)
        {
            returningToPatrol = false;
            PatrolWithHover();
        }
    }

    void DealDamage()
    {
        var playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.TakeDamage(damage);

        if (crowAttackSound != null && audioSource != null)
            audioSource.PlayOneShot(crowAttackSound);
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

    bool IsWithinPatrolBounds()
    {
        if (leftPoint == null || rightPoint == null) return true;
        return transform.position.x >= leftPoint.position.x && transform.position.x <= rightPoint.position.x;
    }

    bool IsWithinPatrolBounds(float xPos)
    {
        if (leftPoint == null || rightPoint == null) return true;
        return xPos >= leftPoint.position.x && xPos <= rightPoint.position.x;
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
