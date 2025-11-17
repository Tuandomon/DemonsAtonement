using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public ItemSlot[] slots = new ItemSlot[10];

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < slots.Length; i++)
            slots[i] = new ItemSlot();
    }

    public bool AddItem(HealthPotion item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].item = item;
                slots[i].amount = 1;
                return true;
            }
        }
        return false;
    }

    public void UseItem(int index)
    {
        if (slots[index].item == null) return;

        // === DÙNG ĐÚNG AddHealth() của PlayerHealth của bạn ===
        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null)
            player.AddHealth(slots[index].item.healAmount);

        RemoveItem(index);
    }

    public void DropItem(int index, Vector3 dropPosition)
    {
        var slot = slots[index];
        if (slot.item == null) return;

        Instantiate(slot.item.droppedPrefab, dropPosition, Quaternion.identity);

        RemoveItem(index);
    }

    public void RemoveItem(int index)
    {
        slots[index].item = null;
        slots[index].amount = 0;
    }
}
