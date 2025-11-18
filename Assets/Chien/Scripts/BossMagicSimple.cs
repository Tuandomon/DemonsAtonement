using UnityEngine;
using System.Collections;

public class BossMagicSimple : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    public float detectionRadius = 12f;
    public float magicRadius = 25f;
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Components")]
    public Animator anim;
    public Rigidbody2D rb;

    [Header("Magic Attack Settings")]
    public GameObject magicPrefab;
    public Transform magicSpawnPoint;
    public float magicCooldown = 4f;
    private float magicTimer = 0f;

    private bool isPlayerDetected = false;
    private bool isCasting = false;

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        bool inArea = player.position.x > leftPoint.position.x &&
                      player.position.x < rightPoint.position.x;

        isPlayerDetected = (distanceToPlayer <= detectionRadius && inArea);

        if (!isPlayerDetected)
            return;

        magicTimer += Time.deltaTime;

        if (distanceToPlayer <= magicRadius)
        {
            TryCastMagic();
        }
    }

    void TryCastMagic()
    {
        if (isCasting) return;                     // tránh spam animation
        if (magicTimer < magicCooldown) return;

        StartCoroutine(CastMagic());
    }

    IEnumerator CastMagic()
    {
        isCasting = true;
        magicTimer = 0f;

        // đứng yên
        if (rb != null)
            rb.velocity = Vector2.zero;

        // chạy animation cast
        if (anim != null)
        {
            anim.SetBool("isRunning", false);
            anim.SetTrigger("MagicAttack");
        }

        // chờ animation bắt đầu rồi mới spawn
        yield return new WaitForSeconds(0.3f);

        // Spawn Magic Ball
        if (magicPrefab != null && magicSpawnPoint != null)
        {
            Instantiate(magicPrefab, magicSpawnPoint.position, Quaternion.identity);
        }

        // đợi animation cast kết thúc
        yield return new WaitForSeconds(0.5f);

        isCasting = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, magicRadius);

        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(leftPoint.position + Vector3.up, leftPoint.position + Vector3.down);
            Gizmos.DrawLine(rightPoint.position + Vector3.up, rightPoint.position + Vector3.down);
        }
    }
}
