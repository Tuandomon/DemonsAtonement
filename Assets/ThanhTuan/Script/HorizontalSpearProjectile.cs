using UnityEngine;

// Đã chỉnh sửa tên thành HorizontalSpearProjectile để phản ánh đúng chức năng đạn
public class HorizontalSpearProjectile : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 20;    // Sát thương gây ra
    public string targetTag = "Player"; // Tag của mục tiêu

    // KHÔNG CÓ HÀM UPDATE()

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Kiểm tra va chạm có phải là mục tiêu (Player) không
        if (other.CompareTag(targetTag))
        {
            // Cố gắng lấy script PlayerHealth để xử lý sát thương
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
            else
            {
                Debug.LogError($"Va chạm với {other.name} (Tag: {targetTag}) nhưng đối tượng này thiếu script PlayerHealth!");
            }

            // Hủy ngọn giáo SAU KHI va chạm với mục tiêu
            Destroy(gameObject);
        }

        // 2. Hủy ngọn giáo nếu chạm vào vật thể KHÁC mục tiêu
        else
        {
            // Ngọn giáo tự hủy khi chạm vào bất cứ thứ gì khác
            Destroy(gameObject);
        }
    }
}