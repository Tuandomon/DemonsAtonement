using UnityEngine;
using UnityEngine.UI;

public class ToggleInventory : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject buttonItem;      // nút Button_Item
    public GameObject panelInventory;  // panel Panel_inventory

    private bool isInventoryOpen = false;

    void Start()
    {
        // Kiểm tra buttonItem và panelInventory
        if (buttonItem == null)
        {
            Debug.LogError("ToggleInventory: buttonItem chưa được gán trong Inspector!");
            return;
        }
        if (panelInventory == null)
        {
            Debug.LogError("ToggleInventory: panelInventory chưa được gán trong Inspector!");
            return;
        }

        // Gắn sự kiện click vào Button_Item
        Button btn = buttonItem.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners(); // xóa listener cũ để tránh lỗi khi scene reload
            btn.onClick.AddListener(Toggle);
        }
        else
        {
            Debug.LogError("ToggleInventory: buttonItem không có component Button!");
        }

        // Đảm bảo panel và button khởi tạo đúng trạng thái
        panelInventory.SetActive(isInventoryOpen);
        buttonItem.SetActive(!isInventoryOpen);
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
