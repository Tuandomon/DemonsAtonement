using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Thêm nếu dùng Slider

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
    public GameObject spiritFireballPrefab; // Prefab Fireball riêng cho Linh hồn

    [Header("Boss Health UI")]
    public Slider bossHealthSlider; // Slider máu boss
    private bool skillsLocked = false; // khóa Lightning, Teleport, Spirit khi máu đầy

    [Header("Spirit Fireball Audio")]
    public AudioClip fireballSound;        // Kéo âm thanh Fireball vào đây
    public AudioSource audioSource;        // Kéo AudioSource lên Boss hoặc tự GetComponent

    void Update()
    {
        if (player == null || leftPoint == null || rightPoint == null)
            return;

        // --- Cập nhật trạng thái khóa skill theo máu ---
        if (bossHealthSlider != null)
        {
            float healthPercent = bossHealthSlider.value / bossHealthSlider.maxValue;
            if (healthPercent >= 1f) skillsLocked = true;   // máu đầy → khóa skill
            else if (healthPercent <= 0.5f) skillsLocked = false; // máu ≤50% → mở khóa
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool inDetection = distanceToPlayer <= detectionRadius;
        bool inRange = player.position.x > leftPoint.position.x && player.position.x < rightPoint.position.x;
        bool inMagicRange = distanceToPlayer <= magicRadius;
        bool inAttackRange = distanceToPlayer <= attackRadius;

        // Player trong detection → bắt đầu rượt
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

        // ⚡ Lightning (khóa khi máu đầy hoặc trong phạm vi tấn công gần)
        if (isPlayerDetected && canCastLightning && !lightningActive
            && chaseTimer >= chaseDelayBeforeLightning
            && !inAttackRange && !skillsLocked)
        {
            StartCoroutine(CastLightning());
        }

        // 🔮 Magic Ball (animation vẫn chạy, spawn chỉ khi ngoài phạm vi gần)
        magicTimer += Time.deltaTime;
        if (isPlayerDetected && !inAttackRange && !lightningActive && !isSummoning && !isTeleporting)
        {
            if (magicTimer >= magicCooldown)
            {
                CastMagicBall(); // animation vẫn chạy
                magicTimer = 0f;
            }
        }

        // 👁 Triệu hồi Spirit (khóa khi máu đầy, Spirit luôn có thể spawn trong phạm vi gần)
        if (isPlayerDetected && canSummon && !isSummoning && !lightningActive && !isTeleporting && !skillsLocked)
        {
            StartCoroutine(SummonSpirits());
        }

        // 🌀 Teleport (khóa khi máu đầy hoặc trong phạm vi tấn công gần)
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

        // spawn khi ngoài phạm vi tấn công gần
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
            anim.SetTrigger("MagicAttack"); // animation triệu hồi Spirit
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

        // Tạo vòng bắt đầu
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

        // Xóa follower + startCircle sau khi teleport
        if (follower != null) Destroy(follower);
        if (startCircle != null) Destroy(startCircle);

        // Tạo vòng kết thúc
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


    void OnDrawGizmosSelected()
    {
        if (transform == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, magicRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(leftPoint.position + Vector3.up, leftPoint.position + Vector3.down);
            Gizmos.DrawLine(rightPoint.position + Vector3.up, rightPoint.position + Vector3.down);
        }
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

            // Tạo AudioSource tạm trên Fireball để phát âm thanh tại vị trí Fireball
            if (fireballSound != null)
            {
                AudioSource tempAudio = fireball.AddComponent<AudioSource>();
                tempAudio.clip = fireballSound;
                tempAudio.spatialBlend = 1f; // âm thanh 3D, phát theo vị trí Fireball
                tempAudio.Play();
                Destroy(tempAudio, fireballSound.length); // tự hủy AudioSource sau khi phát xong
            }
        }
    }
}
