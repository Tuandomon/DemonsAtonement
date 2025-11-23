using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMagicSimple : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    public float detectionRadius = 12f;
    public float magicRadius = 10f;
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Components")]
    public Animator anim;
    public Rigidbody2D rb;

    [Header("Magic Attack Settings")]
    public GameObject magicPrefab;
    public Transform magicSpawnPoint;

    public float magicCooldown = 6f;
    private float magicTimer = 0f;

    [Header("Summon Settings")]
    public GameObject summonedPrefab;
    public Transform summonPoint;
    public float summonCooldown = 10f;
    private float summonTimer = 0f;
    public float summonedLifeTime = 5f;

    private List<GameObject> summonedSpirits = new List<GameObject>();

    private bool isPlayerDetected = false;
    private bool isCasting = false;

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Player trong vùng trái/phải
        bool inArea = player.position.x > leftPoint.position.x &&
                      player.position.x < rightPoint.position.x;

        // ⭐ CHỈ phát hiện khi vào đúng vùng + khoảng cách đúng
        isPlayerDetected = inArea && distanceToPlayer <= detectionRadius;

        // ⛔ Nếu chưa vào phạm vi phát hiện: không bắn, không summon
        if (!isPlayerDetected)
            return;

        // Timer
        magicTimer += Time.deltaTime;
        summonTimer += Time.deltaTime;

        // ⭐ Chỉ bắn khi đã phát hiện player + đang nằm trong magicRadius
        if (distanceToPlayer <= magicRadius)
        {
            TryCastMagic();
        }

        // Summon
        if (summonTimer >= summonCooldown)
        {
            SummonSpirit();
        }
    }

    void TryCastMagic()
    {
        if (!isPlayerDetected) return;       // CHẶN 100% nếu player chưa detect
        if (isCasting) return;
        if (magicTimer < magicCooldown) return;

        StartCoroutine(CastMagic());
    }

    IEnumerator CastMagic()
    {
        isCasting = true;
        magicTimer = 0f;

        if (rb != null) rb.velocity = Vector2.zero;

        if (anim != null)
        {
            anim.SetBool("isRunning", false);
            anim.SetTrigger("MagicAttack");
        }

        yield return new WaitForSeconds(0.3f);

        if (magicPrefab != null && magicSpawnPoint != null)
            Instantiate(magicPrefab, magicSpawnPoint.position, Quaternion.identity);

        yield return new WaitForSeconds(0.5f);
        isCasting = false;
    }

    void SummonSpirit()
    {
        if (summonedPrefab == null || summonPoint == null) return;

        GameObject spirit = Instantiate(summonedPrefab, summonPoint.position, Quaternion.identity);

        BossSummonedSpirit spiritScript = spirit.GetComponent<BossSummonedSpirit>();
        if (spiritScript != null)
        {
            spiritScript.leftPoint = leftPoint;
            spiritScript.rightPoint = rightPoint;
            spiritScript.player = player;
        }

        Destroy(spirit, summonedLifeTime);
        summonedSpirits.Add(spirit);
        summonTimer = 0f;
    }

    public void DespawnAllSummoned()
    {
        foreach (var spirit in summonedSpirits)
        {
            if (spirit != null)
                Destroy(spirit);
        }
        summonedSpirits.Clear();
    }

    void OnDrawGizmosSelected()
    {
        // Vòng phát hiện
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Vòng bắn skill
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, magicRadius);

        // Left - Right
        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftPoint.position, rightPoint.position);
        }
    }
}
