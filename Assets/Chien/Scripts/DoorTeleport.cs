using UnityEngine;

public class DoorTeleport : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform targetDoor;      // Cửa đích để dịch chuyển tới
    public GameObject player;         // Nhân vật
    public GameObject button;         // Nút ở cửa này

    private bool canTeleport = false; // Kiểm tra nhân vật có đang ở gần cửa không

    void Update()
    {
        if (canTeleport && Input.GetKeyDown(KeyCode.E)) // hoặc bạn có thể gọi từ nút UI
        {
            Teleport();
        }
    }

    public void Teleport()
    {
        if (targetDoor != null && player != null)
        {
            player.transform.position = targetDoor.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canTeleport = true;
            if (button != null)
                button.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            canTeleport = false;
            if (button != null)
                button.SetActive(false);
        }
    }
}
