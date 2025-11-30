using UnityEngine;

public class BuffItemPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Inventory inventory = collision.GetComponent<Inventory>();

            if (inventory != null)
            {
                bool added = inventory.AddBuffItem();

                if (added)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}