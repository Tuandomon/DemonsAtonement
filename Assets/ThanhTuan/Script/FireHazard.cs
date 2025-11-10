using UnityEngine;

public class FireHazard : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damagePerSecond = 100;
    public string targetTag = "Player";

    private float nextDamageTime;
    private void OnTriggerStay2D(Collider2D other)
    {

        if (other.CompareTag(targetTag))
        {

            if (Time.time >= nextDamageTime)
            {

                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    // Gây sát thương
                    playerHealth.TakeDamage(damagePerSecond);
                }
                nextDamageTime = Time.time + 1f;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            nextDamageTime = Time.time;
        }
    }
}