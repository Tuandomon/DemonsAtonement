using UnityEngine;

public class BossMagicAttack : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    public float attackRadius = 1.5f; // phạm vi tấn công gần
    public float detectionRadius = 5f; // phạm vi phát hiện (vòng đỏ)
    public float magicRadius = 4f;     // phạm vi cast MagicAttack (vòng xanh dương)
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Components")]
    public Animator anim;
    public Rigidbody2D rb;

    [Header("Magic Settings")]
    public float magicCooldown = 5f; // thời gian giữa các lần cast phép
    private float magicTimer = 0f;
    private bool isPlayerDetected = false; // boss chỉ cast khi đã rượt

    [Header("Magic Prefab")]
    public GameObject magicPrefab;    // Prefab LightBall
    public Transform magicSpawnPoint; // vị trí spawn LightBall

    void Update()
    {
        if (player == null || leftPoint == null || rightPoint == null)
            return;

        magicTimer += Time.deltaTime;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Kiểm tra player trong vùng detection
        bool inDetection = distanceToPlayer <= detectionRadius;
        bool inRange = player.position.x > leftPoint.position.x && player.position.x < rightPoint.position.x;
        bool inMagicRange = distanceToPlayer <= magicRadius;
        bool inAttackRange = distanceToPlayer <= attackRadius;

        // Khi player vào phạm vi phát hiện → bắt đầu rượt
        if (inDetection && inRange)
        {
            isPlayerDetected = true;
        }
        else if (distanceToPlayer > detectionRadius * 1.5f)
        {
            // Nếu player ra quá xa → reset
            isPlayerDetected = false;
        }

        // Chỉ cast phép nếu boss đã rượt và player ở trong vùng phép
        if (isPlayerDetected && inMagicRange && !inAttackRange)
        {
            if (magicTimer >= magicCooldown)
            {
                CastMagic();
                magicTimer = 0f;
            }
        }
    }

    void CastMagic()
    {
        // Dừng di chuyển boss
        if (rb != null)
            rb.velocity = Vector2.zero;

        // Gọi animation Tan cong phep thuat
        if (anim != null)
        {
            anim.SetBool("isRunning", false);
            anim.SetBool("isAttacking", true);
            anim.SetTrigger("MagicAttack");
        }

        // ❌ KHÔNG spawn LightBall ở đây nữa
        // Quả cầu sẽ spawn bằng Animation Event
    }

    // ✅ Hàm này sẽ được gọi từ Animation Event trong animation "Tan cong phep thuat"
    public void SpawnMagicProjectile()
    {
        if (magicPrefab != null && magicSpawnPoint != null)
        {
            Instantiate(magicPrefab, magicSpawnPoint.position, Quaternion.identity);
            Debug.Log("Boss cast LightBall!");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (transform == null) return;

        // Vòng phát hiện (đỏ)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Vòng phép thuật (xanh dương)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, magicRadius);

        // Vòng tấn công gần (xanh lá)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        // Phạm vi trái–phải (vàng)
        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftPoint.position + Vector3.up, leftPoint.position + Vector3.down);
            Gizmos.DrawLine(rightPoint.position + Vector3.up, rightPoint.position + Vector3.down);
        }
    }
}
