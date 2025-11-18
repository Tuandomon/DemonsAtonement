using UnityEngine;

public class HealPotionItem : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip pickupSound;
    [Range(0f, 1f)]
    public float volume = 1f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory playerInventory = other.GetComponent<Inventory>();

            if (playerInventory != null)
            {
                if (playerInventory.AddPotion())
                {
                    if (pickupSound != null)
                    {
                        AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
                    }

                    Destroy(gameObject);
                }
            }
        }
    }

    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning("HealPotionItem Collider2D needs to be Is Trigger!");
        }
    }
}