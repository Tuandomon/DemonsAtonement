using UnityEngine;
using UnityEngine.UI;

public class ToggleInventory : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject buttonItem;        // nút Button_Item
    public GameObject panelInventory;    // panel Panel_inventory

    private bool isInventoryOpen = false;

    void Start()
    {
        // Gắn sự kiện click vào Button_Item
        Button btn = buttonItem.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(Toggle);
        }
    }

    void Update()
    {
        // Nhấn phím M
        if (Input.GetKeyDown(KeyCode.M))
        {
            Toggle();
        }
    }

    void Toggle()
    {
        isInventoryOpen = !isInventoryOpen;

        panelInventory.SetActive(isInventoryOpen);
        buttonItem.SetActive(!isInventoryOpen);
    }
}
