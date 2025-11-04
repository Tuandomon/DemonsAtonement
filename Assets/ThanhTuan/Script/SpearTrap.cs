using UnityEngine;
using System.Collections;

public class SpearTrap : MonoBehaviour
{
    [Header("Spear Settings")]
    public GameObject spearProjectilePrefab;
    public Transform shootPoint;
    public float launchSpeed = 15f;
    public float projectileLifetime = 4f;
    public float cooldownTime = 10f;
    public string targetTag = "Player";

    [Header("Detection Settings")]
    public Collider2D detectionZone;

    [HideInInspector]
    public bool isOnCooldown = false;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // Tìm SpriteRenderer trên đối tượng này HOẶC bất kỳ đối tượng con nào
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spearProjectilePrefab == null || shootPoint == null || detectionZone == null)
        {
            Debug.LogError("SpearTrap: Thiếu các tham chiếu quan trọng!");
            enabled = false;
        }
    }

    public IEnumerator FireAndCooldown()
    {
        if (isOnCooldown) yield break;

        isOnCooldown = true;

        // 1. ẨN BẪY
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        detectionZone.enabled = false;

        // 2. Phóng ngọn giáo
        GameObject spear = Instantiate(spearProjectilePrefab, shootPoint.position, Quaternion.identity);
        Destroy(spear, projectileLifetime); // Tự hủy sau 4s

        // Áp dụng vận tốc ban đầu (Rocket fly)
        Rigidbody2D rb = spear.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.up * launchSpeed;
        }
        // 3. Chờ hồi phục (10s)
        yield return new WaitForSeconds(cooldownTime);

        // 4. HIỆN BẪY
        if (spriteRenderer != null)
        {

            spriteRenderer.enabled = true;
        }
        detectionZone.enabled = true;

        // 5. Kết thúc Cooldown
        isOnCooldown = false;
    }
}