using UnityEngine;

public class Fire : MonoBehaviour
{
    public float lifeTime = 4f;      // Tự hủy sau thời gian
    public int damage = 100;         // Sát thương gây ra cho Player

    [Header("Explosion Effect")]
    public GameObject explosionEffect;   // 👈 Gán prefab hiệu ứng ở đây

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu trúng Player
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            SpawnExplosion();
            Destroy(gameObject);
        }

        // Nếu trúng Ground
        if (collision.CompareTag("Ground"))
        {
            SpawnExplosion();
            Destroy(gameObject);
        }
    }

    // Hàm tạo hiệu ứng Explosion
    private void SpawnExplosion()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
    }
}
