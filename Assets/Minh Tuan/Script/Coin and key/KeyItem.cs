using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Gọi xử lý nhặt key từ Player nếu cần
            Debug.Log("Người chơi đã nhặt chìa khóa!");

            // Xóa chìa khóa khỏi scene
            Destroy(gameObject);
        }
    }
}