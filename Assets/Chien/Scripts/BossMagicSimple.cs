using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Summon Settings")]
    public GameObject summonedPrefab;           // Prefab BossSummonedSpirit
    public Transform summonPoint;               // Vị trí spawn
    public float summonCooldown = 30f;
    private float summonTimer = 0f;
    public float summonedLifeTime = 20f;

    // ⭐ Lưu tất cả quái đã spawn
    private List<GameObject> summonedSpirits = new List<GameObject>();

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

        // Timer
        magicTimer += Time.deltaTime;
        summonTimer += Time.deltaTime;

        // Magic attack
        if (distanceToPlayer <= magicRadius)
        {
            TryCastMagic();
        }

        // Summon BossSummonedSpirit
        if (summonTimer >= summonCooldown)
        {
            SummonSpirit();
        }
    }

    void TryCastMagic()
    {
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

        // Thiết lập phạm vi di chuyển giống Boss
        BossSummonedSpirit spiritScript = spirit.GetComponent<BossSummonedSpirit>();
        if (spiritScript != null)
        {
            spiritScript.leftPoint = leftPoint;
            spiritScript.rightPoint = rightPoint;
            spiritScript.player = player;
        }

        // Destroy sau thời gian sống
        Destroy(spirit, summonedLifeTime);

        // ⭐ Lưu spirit vào danh sách
        summonedSpirits.Add(spirit);

        summonTimer = 0f;
    }

    // ⭐ Hàm hủy tất cả quái khi Boss chết
    public void DespawnAllSummoned()
    {
        foreach (var spirit in summonedSpirits)
        {
            if (spirit != null)
                Destroy(spirit);
        }
        summonedSpirits.Clear();
    }
}
