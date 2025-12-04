using UnityEngine;

public class LetterPickup : MonoBehaviour
{
    public Sprite icon;        // Hình mảnh chữ
    public string itemName;    // Tên mảnh chữ

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InventoryItem.instance.AddItem(icon, itemName);
            Destroy(gameObject);
        }
    }
}
