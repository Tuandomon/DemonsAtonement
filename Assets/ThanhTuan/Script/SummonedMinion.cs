using UnityEngine;

public class SummonedMinion : MonoBehaviour
{
    // Cài đặt Di chuyển
    [Header("Movement")]
    public float moveSpeed = 3f;           
    public float hoverHeight = 1f;

    // Cài đặt AoE Damage
    [Header("AoE Damage Settings")]
    public float damagePerSecond = 2f;
    public float damageRadius = 2.5f;  
    public float damageInterval = 0.5f;

    // Cài đặt Tồn tại
    [Header("Lifetime")]
    public float lifetime = 15f;

    // Tham chiếu
    [HideInInspector]
    public EnemyController bossController;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float damageTimer;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // CÀI ĐẶT BAY LƠ LỬNG
        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.drag = 0f;
        }

        Destroy(gameObject, lifetime);

        damageTimer = damageInterval;
    }

    void OnDestroy()
    {
        if (bossController != null)
        {
            bossController.MinionDestroyed(gameObject);
        }
    }

    void Update()
    {
        damageTimer -= Time.deltaTime;

        if (damageTimer <= 0f)
        {
            DealAoEDamage();
            damageTimer = damageInterval;
        }
    }

    void FixedUpdate()
    {
        if (player == null || rb == null) return;

        // 1. Xác định vị trí mục tiêu
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y + hoverHeight, transform.position.z);

        // 2. Tính toán vector hướng và khoảng cách
        Vector2 direction = (targetPosition - transform.position);
        float distanceToTarget = direction.magnitude;

        Vector2 velocity = Vector2.zero;

        // 3. LOGIC DI CHUYỂN VÀ DỪNG LẠI
        if (distanceToTarget > damageRadius)
        {
            // Bám theo: Di chuyển với tốc độ cố định
            velocity = direction.normalized * moveSpeed;
        }
        else
        {
            // Dừng lại: Giảm tốc độ
            velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.fixedDeltaTime * 10f);
        }

        rb.velocity = velocity;

        // 4. Lật Sprite theo hướng Player
        if (spriteRenderer != null)
        {
            float directionToPlayer = player.position.x - transform.position.x;

            if (directionToPlayer > 0.05f)
            {
                spriteRenderer.flipX = false;
            }
            else if (directionToPlayer < -0.05f)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    void DealAoEDamage()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= damageRadius)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                int damage = Mathf.RoundToInt(damagePerSecond * damageInterval);
                playerHealth.TakeDamage(damage);
            }
        }
    }
}