using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 20;    // Sát thương gây ra (20 DMG)
    public string targetTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {

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
    }
}