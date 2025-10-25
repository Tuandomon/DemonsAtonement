using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 20;    // Sát thương gây ra (20 DMG)
    public string targetTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Kiểm tra va chạm có phải là Player không
        if (other.CompareTag(targetTag))
        {
            // Cố gắng lấy script PlayerHealth
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Gây sát thương
                playerHealth.TakeDamage(damageAmount);
            }
            else
            {
                Debug.LogError("Player thiếu script PlayerHealth!");
            }

            // Hủy ngọn giáo SAU KHI gây sát thương cho Player
            Destroy(gameObject);
        }

        // 2. Tùy chọn: Hủy ngọn giáo nếu chạm vào vật thể KHÁC Player
        // Bạn có thể bỏ đoạn code này nếu muốn ngọn giáo xuyên qua các vật thể khác
        /*
        else 
        {
             Destroy(gameObject);
        }
        */
    }
}