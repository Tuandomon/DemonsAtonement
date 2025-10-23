using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float lifetime = 2f;
    public int damageAmount = 30;

    void Start()
    {
        // Hủy đạn sau 2 giây nếu không va chạm với gì
        Destroy(gameObject, lifetime);
    }

    // Xử lý va chạm
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ... Logic gây sát thương ...
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }

        // Dòng lệnh này đảm bảo đạn bị hủy sau khi va chạm với BẤT KỲ THỨ GÌ
        // (bao gồm cả Player và Tường/Đất).
        Destroy(gameObject);
    }
}