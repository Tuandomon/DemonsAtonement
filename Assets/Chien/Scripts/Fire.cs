using UnityEngine;

public class Fire : MonoBehaviour
{
    public float lifeTime = 4f;  // Tự hủy sau thời gian
    public int damage = 100;     // Sát thương gây ra cho Player

    private void Start()
    {
        // Tự huỷ sau lifeTime giây nếu không trúng gì
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

            Destroy(gameObject);
        }

        // Nếu trúng vật có tag Ground → tự hủy
        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
