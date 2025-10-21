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

    [Header("Tham chiếu")]
    public Transform player;
    private bool isChasing = false;

    private Rigidbody2D rb;
    private Animator animator;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Tự động tìm player
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // Đảm bảo chỉ di chuyển trái-phải
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        }
    }

    void Update()
    {
        if (player == null) return;

        // Tính khoảng cách theo trục X (chỉ quan tâm chiều ngang)
        float distanceX = Mathf.Abs(transform.position.x - player.position.x);
        float distanceY = Mathf.Abs(transform.position.y - player.position.y);

        // Chỉ đuổi theo nếu player ở cùng độ cao (trong ngưỡng cho phép)
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

        // Kiểm tra tấn công
        if (distanceX <= attackRange && Time.time >= lastAttackTime + attackCooldown && distanceY <= heightTolerance)
        {
            Attack();
            lastAttackTime = Time.time;
        }

        UpdateAnimation();
    }

    void ChasePlayer()
    {
        if (player == null) return;

        // Tính hướng di chuyển (chỉ theo trục X)
        Vector2 direction = new Vector2(player.position.x - transform.position.x, 0).normalized;

        // Di chuyển chỉ theo trục X
        Vector2 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(new Vector2(newPosition.x, rb.position.y)); // Giữ nguyên vị trí Y

        // Xoay hình enemy theo hướng di chuyển
        if (direction.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (direction.x < 0 && facingRight)
        {
            Flip();
        }
    }

    void StopChasing()
    {
        // Dừng di chuyển
        rb.velocity = new Vector2(0, rb.velocity.y);
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
        if (player == null) return;

        PlayerHeath playerHealth = player.GetComponent<PlayerHeath>();
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

    // Phát hiện player (chỉ theo chiều ngang)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Kiểm tra xem player có cùng độ cao không
            float heightDiff = Mathf.Abs(transform.position.y - other.transform.position.y);
            if (heightDiff <= 2f) // Ngưỡng chiều cao cho phép
            {
                player = other.transform;
                Debug.Log("Phát hiện nhân vật!");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Nhân vật đã rời khỏi vùng!");
        }
    }

    // Vẽ Gizmos để debug
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Vẽ vùng chiều cao cho phép
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(chaseRange * 2, 2f, 0));
    }
}
