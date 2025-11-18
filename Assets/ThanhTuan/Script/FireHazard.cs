using UnityEngine;

public class FireHazard : MonoBehaviour
{
    [Header("Damage Settings")]
    // Đổi tên để phản ánh sát thương mỗi frame (nên đặt giá trị nhỏ)
    public int damagePerHit = 5;
    public string targetTag = "Player";

    // Loại bỏ biến private float nextDamageTime;
    // Loại bỏ hàm OnTriggerEnter2D

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Gây sát thương ngay lập tức mỗi frame
                playerHealth.TakeDamage(damagePerHit);
            }
        }
    }
}