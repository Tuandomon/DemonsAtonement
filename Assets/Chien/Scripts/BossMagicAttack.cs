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

    [Header("Magic Prefab")]
    public GameObject magicPrefab;    // Prefab LightBall
    public Transform magicSpawnPoint; // vị trí spawn LightBall

    void Update()
    {
        if (player == null || leftPoint == null || rightPoint == null)
            return;

        magicTimer += Time.deltaTime;

        // Kiểm tra player còn trong phạm vi hoạt động (vòng đỏ + left-right)
        bool playerInDetection = Vector2.Distance(transform.position, player.position) <= detectionRadius;
        bool playerInRange = player.position.x > leftPoint.position.x && player.position.x < rightPoint.position.x;
        bool playerInMagicRange = Vector2.Distance(transform.position, player.position) <= magicRadius;

        // Player không gần (ngoài attackRadius) nhưng còn trong vùng → cast phép
        if (playerInDetection && playerInRange && playerInMagicRange && Vector2.Distance(transform.position, player.position) > attackRadius)
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

        // Bật animation Tan cong phep thuat
        if (anim != null)
        {
            anim.SetBool("isRunning", false);
            anim.SetBool("isAttacking", false);
            anim.SetTrigger("MagicAttack"); // trigger của Tan cong phep thuat
        }

        // Spawn LightBall
        if (magicPrefab != null && magicSpawnPoint != null)
        {
            Instantiate(magicPrefab, magicSpawnPoint.position, Quaternion.identity);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (transform == null) return;

        // Vòng phát hiện
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Vòng tấn công gần
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        // Vòng phép thuật
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, magicRadius);

        // Phạm vi trái–phải
        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftPoint.position + Vector3.up, leftPoint.position + Vector3.down);
            Gizmos.DrawLine(rightPoint.position + Vector3.up, rightPoint.position + Vector3.down);
        }
    }
}
