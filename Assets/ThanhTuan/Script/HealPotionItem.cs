using UnityEngine;

public class HealPotionItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory playerInventory = other.GetComponent<Inventory>();

            if (playerInventory != null)
            {
                if (playerInventory.AddPotion())
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    void Start()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null && !collider.isTrigger)
        {
            Debug.LogWarning("HealPotionItem cần Collider2D được đặt là Is Trigger!");
        }
    }
}