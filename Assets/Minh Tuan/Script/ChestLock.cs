using UnityEngine;

public class ChestLock : MonoBehaviour
{
    [Header("Chest Prefab mở")]
    [SerializeField] private GameObject openChestPrefab;   // Prefab rương mở

    [Header("Panel Lock")]
    [SerializeField] private GameObject panelObj;          // Kéo thả UIPassLock từ Hierarchy vào đây

    private LockPanelT9 lockPanelScript;

    private void Start()
    {
        if (panelObj != null)
        {
            lockPanelScript = panelObj.GetComponent<LockPanelT9>();
            if (lockPanelScript != null)
            {
                lockPanelScript.onUnlock.AddListener(UnlockChest);
            }

            // Ẩn panel lúc đầu
            panelObj.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Panel Lock chưa được gán trong Inspector!");
        }
    }

    // Khi người chơi lại gần và bấm E thì mở panel (dùng Collider2D)
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKeyDown(KeyCode.E))
        {
            if (panelObj != null)
            {
                panelObj.SetActive(true);
            }
        }
    }

    // Hàm gọi khi mở khóa thành công
    private void UnlockChest()
    {
        if (panelObj != null)
        {
            panelObj.SetActive(false);
        }

        if (openChestPrefab)
        {
            Instantiate(openChestPrefab, transform.position, transform.rotation);
            Destroy(gameObject); // Xóa rương cũ
        }
    }
}