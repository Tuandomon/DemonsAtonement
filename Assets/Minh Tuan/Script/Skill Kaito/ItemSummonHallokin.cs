using UnityEngine;

public class ItemSummonHallokin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Inventory playerInventory = collision.GetComponent<Inventory>();

            if (playerInventory != null)
            {
                if (playerInventory.AddSummonItem())
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}