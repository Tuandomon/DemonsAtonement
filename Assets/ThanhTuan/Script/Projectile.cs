using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float lifetime = 2f;
    public int damageAmount = 30;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Check for Player collision and apply damage
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            // Destroy after hitting Player
            Destroy(gameObject);
            return;
        }

        // 2. Check for Environment/Tilemap collision
        // Use the tags you assigned to your Tilemap or static obstacles
        else if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            // Destroy after hitting environment
            Destroy(gameObject);
        }

        // If it hits anything else (like the shooter itself), nothing happens, 
        // and the projectile continues to travel until its 'lifetime' expires.
    }
}