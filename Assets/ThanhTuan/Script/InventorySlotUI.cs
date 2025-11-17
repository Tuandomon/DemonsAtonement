using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public int index;
    public Image icon;

    public GameObject actionMenu;

    void Update()
    {
        var slot = InventoryManager.Instance.slots[index];

        if (slot.item != null)
        {
            icon.enabled = true;
            icon.sprite = slot.item.icon;
        }
        else
        {
            icon.enabled = false;
        }
    }

    public void OnClick()
    {
        actionMenu.SetActive(true);
    }

    public void Use()
    {
        InventoryManager.Instance.UseItem(index);
        actionMenu.SetActive(false);
    }

    public void Drop()
    {
        PlayerHealth player = FindObjectOfType<PlayerHealth>();

        Vector3 pos = player.transform.position + Vector3.right * 0.5f;
        InventoryManager.Instance.DropItem(index, pos);

        actionMenu.SetActive(false);
    }
}
