using UnityEngine;
using System.Collections;

public class HorizontalSpearLauncher : MonoBehaviour
{
    [Header("Spear Projectile Setup")]
    [Tooltip("Kéo Prefab ngọn giáo vào đây (phải gắn script HorizontalSpearProjectile).")]
    public GameObject spearProjectilePrefab;
    public Transform shootPoint;
    public float launchSpeed = 18f; // Tốc độ phóng ngang
    public float projectileLifetime = 4f; // Thời gian tồn tại của giáo
    public string targetTag = "Player"; // Tag của mục tiêu gây sát thương

    [Header("Launch Direction")]
    [Tooltip("Đặt -1 để bắn sang Trái, 1 để bắn sang Phải.")]
    public int horizontalDirection = 1; // Hướng bắn ngang

    [Header("Cooldown Settings")]
    public float cooldownTime = 5f; // Thời gian hồi chiêu

    [Header("Detection Zone")]
    public Collider2D activationZone; // Collider 2D (Is Trigger) kích hoạt bẫy

    [HideInInspector]
    public bool isOnCooldown = false;

    private SpriteRenderer launcherSprite;

    void Start()
    {
        // Khởi tạo và kiểm tra lỗi
        launcherSprite = GetComponent<SpriteRenderer>();
        if (launcherSprite == null) { launcherSprite = GetComponentInChildren<SpriteRenderer>(); }

        if (spearProjectilePrefab == null || shootPoint == null || activationZone == null)
        {
            Debug.LogError(gameObject.name + ": Thiếu các tham chiếu quan trọng!");
            enabled = false;
            return;
        }

        horizontalDirection = Mathf.Clamp(horizontalDirection, -1, 1);
        if (horizontalDirection == 0) horizontalDirection = 1;

        // Tùy chọn: Lật sprite của bẫy cho phù hợp với hướng bắn
        transform.localScale = new Vector3(horizontalDirection * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    public void ActivateTrap()
    {
        StartCoroutine(FireAndCooldown());
    }

    public IEnumerator FireAndCooldown()
    {
        if (isOnCooldown) yield break;

        isOnCooldown = true;

        // 1. Tạm thời ẩn bẫy và tắt vùng kích hoạt
        if (launcherSprite != null) launcherSprite.enabled = false;
        if (activationZone != null) activationZone.enabled = false;

        // 2. PHÓNG NGỌN GIÁO NGANG
        GameObject spear = Instantiate(spearProjectilePrefab, shootPoint.position, Quaternion.identity);
        Destroy(spear, projectileLifetime);

        // Lật sprite của ngọn giáo cho đúng hướng
        spear.transform.localScale = new Vector3(horizontalDirection * Mathf.Abs(spear.transform.localScale.x), spear.transform.localScale.y, spear.transform.localScale.z);

        // Gán vận tốc Rigidbody2D để bay ngang
        Rigidbody2D rb = spear.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.right * horizontalDirection * launchSpeed;
        }
        else
        {
            Debug.LogError("Ngọn giáo thiếu Rigidbody2D! Hãy đảm bảo prefab có Rigidbody2D và Gravity Scale = 0.");
        }

        // 3. Chờ hồi chiêu
        yield return new WaitForSeconds(cooldownTime);

        // 4. Hiện bẫy và bật lại vùng kích hoạt
        if (launcherSprite != null) launcherSprite.enabled = true;
        if (activationZone != null) activationZone.enabled = true;

        isOnCooldown = false;
    }
}