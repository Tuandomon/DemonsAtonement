using UnityEngine;

public class LightSkill : MonoBehaviour
{
    public float lifeTime = 4f;  // Thời gian tự huỷ nếu không trúng gì
    public int damage = 100;     // Sát thương gây ra cho Player

    private void Start()
    {
        // Sau 4 giây tự huỷ nếu không trúng ai
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

        // Nếu trúng đất hoặc tường
        if (collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
