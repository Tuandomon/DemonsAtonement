using UnityEngine;

public class PickupPotion : MonoBehaviour
{
    public HealthPotion potion;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (InventoryManager.Instance.AddItem(potion))
            {
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory đầy!");
            }
        }
    }
}
