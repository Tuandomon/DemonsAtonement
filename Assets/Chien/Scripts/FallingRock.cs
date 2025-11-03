using UnityEngine;

public class FallingRock : MonoBehaviour
{
    public int damage = 50;           // đổi thành int cho khớp với PlayerHealth
    public float lifetime = 5f;
    private bool hasHit = false;
    private FallingRockSpawner spawner;

    public void SetSpawner(FallingRockSpawner rockSpawner)
    {
        spawner = rockSpawner;
    }

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.bodyType = RigidbodyType2D.Dynamic;

        float randomDelay = Random.Range(0f, 0.5f);
        Invoke(nameof(DestroyIfNoHit), lifetime + randomDelay);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;

        if (collision.CompareTag("Player"))
        {
            // ✅ Lấy PlayerHealth và gọi TakeDamage(int)
            var playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"Player trúng đá! Mất {damage} máu.");
            }

            hasHit = true;
            spawner?.StartRespawn();
            Destroy(gameObject);
        }
    }

    void DestroyIfNoHit()
    {
        if (!hasHit)
        {
            spawner?.StartRespawn();
            Destroy(gameObject);
        }
    }
}
