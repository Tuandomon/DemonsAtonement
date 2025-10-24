using UnityEngine;

public class BombTrap : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 50;
    public string targetTag = "Player";

    [Header("Visual Settings")]
    public Sprite explosionSprite; // Gán Sprite hình ngọn lửa vào đây
    public float explosionDuration = 0.5f; // Thời gian hiển thị ngọn lửa

    private SpriteRenderer spriteRenderer;
    private Collider2D trapCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        trapCollider = GetComponent<Collider2D>();

        if (spriteRenderer == null)
        {
            Debug.LogError("BombTrap: Thiếu SpriteRenderer Component!");
        }
        if (trapCollider == null)
        {
            Debug.LogError("BombTrap: Thiếu Collider2D Component!");
        }
    }

    // Xử lý va chạm (Trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra đối tượng va chạm
        if (other.CompareTag(targetTag))
        {
            // Gây sát thương
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            // Bắt đầu chuỗi kích hoạt
            StartExplosionSequence();
        }
    }

    private void StartExplosionSequence()
    {
        // 1. Tắt Collider để không gây sát thương lần nữa
        trapCollider.enabled = false;

        // 2. Thay đổi Sprite sang hình ngọn lửa
        if (explosionSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = explosionSprite;
        }

        // 3. Hủy quả bom sau 0.5 giây (hoặc giá trị explosionDuration)
        Invoke("DestroyBomb", explosionDuration);

        // Vô hiệu hóa script này để không chạy thêm OnTriggerEnter2D nào nữa
        enabled = false;
    }

    private void DestroyBomb()
    {
        // Hủy GameObject quả bom
        Destroy(gameObject);
    }
}