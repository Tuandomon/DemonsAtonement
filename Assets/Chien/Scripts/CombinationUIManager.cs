using UnityEngine;
using UnityEngine.UI;

public class CombinationUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panelInventory;            // Panel Inventory
    public GameObject buttonItemCombination;     // Button mở/tắt combination
    public GameObject panelItemCombination;      // Panel combination

    private bool isCombinationOpen = false;

    void Start()
    {
        if (buttonItemCombination != null)
        {
            Button btn = buttonItemCombination.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners(); // loại bỏ listener cũ nếu có
                btn.onClick.AddListener(ToggleCombination);
            }
        }

        // Khởi tạo panel combination tắt luôn
        if (panelItemCombination != null)
            panelItemCombination.SetActive(false);
    }


    void Update()
    {
        // Nhấn phím N để toggle
        if (Input.GetKeyDown(KeyCode.N))
        {
            ToggleCombination();
        }

        // Nếu panelInventory tắt → ẩn panel combination
        if (!panelInventory.activeSelf && isCombinationOpen)
        {
            CloseCombination();
        }
    }

    public void ToggleCombination()
    {
        isCombinationOpen = !isCombinationOpen;
        panelItemCombination.SetActive(isCombinationOpen);
        Debug.Log("ToggleCombination called, isCombinationOpen = " + isCombinationOpen);
    }


    public void CloseCombination()
    {
        isCombinationOpen = false;
        if (panelItemCombination != null)
            panelItemCombination.SetActive(false);
    }
}
