using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Thêm để dùng List
using UnityEngine.UI;

public class BossMagicAttack : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform player;
    public float attackRadius = 4f;
    public float detectionRadius = 14f;
    public float magicRadius = 31f;
    public Transform leftPoint;
    public Transform rightPoint;

    [Header("Components")]
    public Animator anim;
    public Rigidbody2D rb;

    [Header("Magic Settings")]
    public GameObject magicPrefab;
    public Transform magicSpawnPoint;
    public float magicCooldown = 5f;
    private float magicTimer = 0f;

    [Header("Lightning Skill Settings")]
    public GameObject lightningPrefab;
    public float lightningCooldown = 10f;
    public float chaseDelayBeforeLightning = 1.5f;
    private float chaseTimer = 0f;
    private bool canCastLightning = true;
    private bool lightningActive = false;

    [Header("Cài đặt Triệu hồi Linh hồn")]
    public GameObject summonCirclePrefab;
    public GameObject spiritPrefab;
    public float summonCooldown = 30f;
    public float summonDelay = 2f;
    public float summonRadius = 2.5f;
    private bool canSummon = true;
    private bool isSummoning = false;

    [Header("Cài đặt Kỹ năng Dịch chuyển")]
    public GameObject teleportCirclePrefab;
    public float teleportCooldown = 60f;
    private bool canTeleport = true;
    private bool isTeleporting = false;

    private bool isPlayerDetected = false;
    private bool hasStartedChasing = false;

    [Header("Cài đặt Linh hồn")]
    public GameObject spiritFireballPrefab;

    [Header("Boss Health UI")]
    public Slider bossHealthSlider;
    private bool skillsLocked = false;

    [Header("Spirit Fireball Audio")]
    public AudioClip fireballSound;
    public AudioSource audioSource;

    // 🌀 Thêm danh sách quản lý toàn bộ Spirit đang tồn tại
    private List<GameObject> activeSpirits = new List<GameObject>();

    void Update()
    {
        if (player == null || leftPoint == null || rightPoint == null)
            return;

        // --- Kiểm tra máu Boss ---
        if (bossHealthSlider != null)
        {
            float healthPercent = bossHealthSlider.value / bossHealthSlider.maxValue;

            // 🔒 Khóa skill khi máu đầy
            if (healthPercent >= 1f) skillsLocked = true;
            // 🔓 Mở skill khi máu <= 50%
            else if (healthPercent <= 0.5f) skillsLocked = false;

            // 💀 Khi máu Boss = 0 → Hủy toàn bộ Spirit
            if (bossHealthSlider.value <= 0f)
            {
                DestroyAllSpirits();
                return; // Dừng Update luôn để boss không còn hoạt động
            }
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool inDetection = distanceToPlayer <= detectionRadius;
        bool inRange = player.position.x > leftPoint.position.x && player.position.x < rightPoint.position.x;
        bool inMagicRange = distanceToPlayer <= magicRadius;
        bool inAttackRange = distanceToPlayer <= attackRadius;

        if (inDetection && inRange)
        {
            isPlayerDetected = true;
            hasStartedChasing = true;
            chaseTimer += Time.deltaTime;
        }
        else
        {
            isPlayerDetected = false;
            hasStartedChasing = false;
            chaseTimer = 0f;
        }

        if (!hasStartedChasing) return;

        // ⚡ Lightning
        if (isPlayerDetected && canCastLightning && !lightningActive
            && chaseTimer >= chaseDelayBeforeLightning
            && !inAttackRange && !skillsLocked)
        {
            StartCoroutine(CastLightning());
        }

        // 🔮 Magic Ball
        magicTimer += Time.deltaTime;
        if (isPlayerDetected && !inAttackRange && !lightningActive && !isSummoning && !isTeleporting)
        {
            if (magicTimer >= magicCooldown)
            {
                CastMagicBall();
                magicTimer = 0f;
            }
        }

        // 👁 Triệu hồi Spirit
        if (isPlayerDetected && canSummon && !isSummoning && !lightningActive && !isTeleporting && !skillsLocked)
        {
            StartCoroutine(SummonSpirits());
        }

        // 🌀 Teleport
        if (isPlayerDetected && canTeleport && !isTeleporting && !lightningActive && !isSummoning
            && !skillsLocked && !inAttackRange)
        {
            StartCoroutine(TeleportToPlayer());
        }
    }

    // ===================== MAGIC ATTACK ====================
    void CastMagicBall()
    {
        if (rb != null) rb.velocity = Vector2.zero;
        if (anim != null)
        {
            anim.SetBool("isRunning", false);
            anim.SetTrigger("MagicAttack");
        }

        if (magicPrefab != null && magicSpawnPoint != null)
        {
            if (Vector2.Distance(transform.position, player.position) > attackRadius)
                Instantiate(magicPrefab, magicSpawnPoint.position, Quaternion.identity);
        }
    }

    // ==================== LIGHTNING ATTACK ===================
    IEnumerator CastLightning()
    {
        canCastLightning = false;
        lightningActive = true;

        if (rb != null) rb.velocity = Vector2.zero;

        if (anim != null)
        {
            anim.SetBool("isRunning", false);
            anim.SetTrigger("MagicAttack");
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < 2; i++)
        {
            SpawnLightning();
            yield return new WaitForSeconds(0.5f);
        }

        lightningActive = false;
        yield return new WaitForSeconds(lightningCooldown);
        canCastLightning = true;
        chaseTimer = 0f;
    }

    void SpawnLightning()
    {
        if (lightningPrefab != null && player != null)
        {
            Vector3 strikePos = new Vector3(player.position.x, player.position.y + 2f, 0f);
            GameObject lightningObj = Instantiate(lightningPrefab, strikePos, Quaternion.identity);
            Destroy(lightningObj, 1.5f);
        }
    }

    // ====================== TRIỆU HỒN LINH HỒN =====================
    IEnumerator SummonSpirits()
    {
        canSummon = false;
        isSummoning = true;

        if (rb != null) rb.velocity = Vector2.zero;
        if (anim != null)
        {
            anim.SetBool("isRunning", false);
            anim.SetTrigger("MagicAttack");
        }

        yield return new WaitForSeconds(0.5f);

        Vector3 forwardDir = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
        Vector3 basePos = transform.position + forwardDir * 1.5f;

        Vector3[] summonPositions = new Vector3[3];
        for (int i = 0; i < 3; i++)
        {
            float angle = (i - 1) * 30f * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * summonRadius;
            summonPositions[i] = basePos + offset;
        }

        GameObject[] circles = new GameObject[3];
        for (int i = 0; i < 3; i++)
            circles[i] = Instantiate(summonCirclePrefab, summonPositions[i], Quaternion.identity);

        yield return new WaitForSeconds(summonDelay);

        for (int i = 0; i < 3; i++)
        {
            if (spiritPrefab != null)
            {
                GameObject spirit = Instantiate(spiritPrefab, summonPositions[i], Quaternion.identity);
                activeSpirits.Add(spirit); // 🌀 thêm vào danh sách để quản lý

                SpiritFollowBoss follow = spirit.AddComponent<SpiritFollowBoss>();
                follow.boss = this.transform;
                follow.offset = summonPositions[i] - transform.position;
                follow.fireballPrefab = spiritFireballPrefab;
                follow.firePoint = spirit.transform;
                follow.shootInterval = 2f;
                follow.fireballSpeed = 5f;

                if (follow.fireballPrefab != null)
                    StartCoroutine(SpawnSpiritFireballWithDelay(follow.firePoint, follow.fireballPrefab, 4f));

                Destroy(spirit, 10f);
            }

            if (circles[i] != null) Destroy(circles[i]);
        }

        isSummoning = false;
        yield return new WaitForSeconds(summonCooldown);
        canSummon = true;
    }

    // 🧨 Hủy toàn bộ Spirit khi Boss chết
    void DestroyAllSpirits()
    {
        foreach (GameObject spirit in activeSpirits)
        {
            if (spirit != null)
                Destroy(spirit);
        }
        activeSpirits.Clear();
    }

    // ================== KỸ NĂNG DỊCH CHUYỂN ====================
    IEnumerator TeleportToPlayer()
    {
        canTeleport = false;
        isTeleporting = true;

        if (rb != null) rb.velocity = Vector2.zero;

        if (anim != null)
        {
            anim.SetBool("isRunning", false);
            anim.SetTrigger("MagicAttack");
        }

        GameObject startCircle = null;
        if (teleportCirclePrefab != null)
            startCircle = Instantiate(teleportCirclePrefab, new Vector3(transform.position.x, -19f, 0f), Quaternion.identity);

        TeleportCircleFollower follower = null;
        if (startCircle != null)
        {
            follower = startCircle.AddComponent<TeleportCircleFollower>();
            follower.target = transform;
        }

        yield return new WaitForSeconds(2f);

        Vector3 teleportPos = transform.position;
        if (player != null)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            teleportPos = player.position - dirToPlayer * 2f;
        }

        transform.position = new Vector3(teleportPos.x, -19f, 0f);

        if (follower != null) Destroy(follower);
        if (startCircle != null) Destroy(startCircle);

        GameObject endCircle = null;
        if (teleportCirclePrefab != null)
            endCircle = Instantiate(teleportCirclePrefab, new Vector3(teleportPos.x, -19f, 0f), Quaternion.identity);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;

        yield return new WaitForSeconds(1.5f);
        if (endCircle != null) Destroy(endCircle);

        isTeleporting = false;
        yield return new WaitForSeconds(teleportCooldown);
        canTeleport = true;
    }

    public class TeleportCircleFollower : MonoBehaviour
    {
        public Transform target;
        void Update()
        {
            if (target != null)
                transform.position = new Vector3(target.position.x, -19f, 0f);
        }
    }

    IEnumerator SpawnSpiritFireballWithDelay(Transform firePoint, GameObject fireballPrefab, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (firePoint != null && fireballPrefab != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

            if (fireballSound != null)
            {
                AudioSource tempAudio = fireball.AddComponent<AudioSource>();
                tempAudio.clip = fireballSound;
                tempAudio.spatialBlend = 1f;
                tempAudio.Play();
                Destroy(tempAudio, fireballSound.length);
            }
        }
    }
}
