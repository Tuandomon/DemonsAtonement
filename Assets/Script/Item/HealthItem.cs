using UnityEngine;

public class HealthItem : MonoBehaviour
{
    public int healAmount = 20;
    public GameObject collectEffect;
    public AudioClip collectSound;

    public void OnCollected()
    {

        // Bi?n m?t sau khi ?n
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Khi Player ch?m vào item
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.AddHealth(healAmount);
            }

            // G?i hi?u ?ng r?i bi?n m?t
            OnCollected();
        }
    }
}
