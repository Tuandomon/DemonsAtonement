using UnityEngine;
using System.Collections;

public class BossAnimatorDie : MonoBehaviour
{
    [Header("Components")]
    public EnemyHealth enemyHealth;
    public Animator anim;
    private bool deathAnimPlayed = false;

    [Header("Thời gian destroy sau chết")]
    public float totalDeathTime = 5f;
    public float fadeStartTime = 3f;

    [Header("Prefabs hiệu ứng khi chết")]
    public GameObject deathEffect1;
    public GameObject deathEffect2;

    [Header("Vòng triệu hồi")]
    public GameObject summonCirclePrefab; // 🔹 Prefab vòng triệu hồi
    private GameObject summonCircleInstance;
    private SpriteRenderer summonCircleSR;

    [Header("Cài đặt spawn hiệu ứng")]
    public float spawnRadius = 2f;
    public float spawnInterval = 0.1f;

    private bool hasDied = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        if (enemyHealth == null)
            enemyHealth = GetComponent<EnemyHealth>();

        if (enemyHealth != null)
            enemyHealth.allowDestroy = false;

        if (anim == null)
            anim = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (hasDied) return;

        if (enemyHealth != null && enemyHealth.GetCurrentHealth() <= 0)
        {
            hasDied = true;
            StartCoroutine(PlayDeathSequence());
        }
    }

    IEnumerator PlayDeathSequence()
    {
        // ⭐ Chạy animation chết
        if (!deathAnimPlayed && anim != null)
        {
            anim.SetTrigger("Die");
            deathAnimPlayed = true;
        }

        // ❗ Tắt các script khác
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this && script.enabled)
                script.enabled = false;
        }

        // 🔹 Spawn vòng triệu hồi
        if (summonCirclePrefab != null)
        {
            Vector3 footPosition = transform.position + new Vector3(0f, 0.3f, 0f); // +0.3f Y để lên cao hơn
            summonCircleInstance = Instantiate(summonCirclePrefab, footPosition, Quaternion.identity);
            summonCircleSR = summonCircleInstance.GetComponent<SpriteRenderer>();
            if (summonCircleSR != null)
            {
                summonCircleSR.color = new Color(1f, 1f, 1f, 1f); // full alpha
            }
        }


        float elapsed = 0f;

        while (elapsed < totalDeathTime)
        {
            elapsed += spawnInterval;

            // ⭐ Spawn effect chết
            SpawnDeathEffect(deathEffect1);
            SpawnDeathEffect(deathEffect2);

            // ⭐ Fade boss
            if (spriteRenderer != null && elapsed >= fadeStartTime)
            {
                float fadeElapsed = elapsed - fadeStartTime;
                float fadeDuration = totalDeathTime - fadeStartTime;
                float alpha = Mathf.Lerp(1f, 0f, fadeElapsed / fadeDuration);
                Color c = spriteRenderer.color;
                spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);

                // 🔹 Fade vòng triệu hồi theo boss
                if (summonCircleSR != null)
                {
                    Color c2 = summonCircleSR.color;
                    summonCircleSR.color = new Color(c2.r, c2.g, c2.b, alpha);
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        // Destroy vòng triệu hồi sau khi boss chết
        if (summonCircleInstance != null)
            Destroy(summonCircleInstance);

        // Destroy boss
        Destroy(gameObject);
    }

    void SpawnDeathEffect(GameObject effectPrefab)
    {
        if (effectPrefab == null) return;

        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPos = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

        GameObject effect = Instantiate(effectPrefab, spawnPos, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
        float randomScale = Random.Range(0.2f, 1f);
        effect.transform.localScale = Vector3.one * randomScale;

        SpriteRenderer sr = effect.GetComponent<SpriteRenderer>();
        if (sr != null)
            effect.AddComponent<DeathEffectBehavior>();
    }
}

// ⭐ Script cho hiệu ứng bay lên + fade
public class DeathEffectBehavior : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float lifeTime = 2f;
    public float fadeDuration = 1f;

    private float elapsed = 0f;
    private Vector3 moveDirection;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        moveDirection = new Vector3(Random.Range(-0.3f, 0.3f), 1f, 0f).normalized;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        transform.position += moveDirection * floatSpeed * Time.deltaTime;

        if (sr != null && elapsed >= lifeTime - fadeDuration)
        {
            float fadeElapsed = elapsed - (lifeTime - fadeDuration);
            float alpha = Mathf.Lerp(1f, 0f, fadeElapsed / fadeDuration);
            Color c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, alpha);
        }

        if (elapsed >= lifeTime)
            Destroy(gameObject);
    }
}
