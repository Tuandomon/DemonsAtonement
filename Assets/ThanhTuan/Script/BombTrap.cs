using UnityEngine;

public class BombTrap : MonoBehaviour
{
    // Kéo thả animation clip nổ vào đây trong Inspector
    public Animator bombAnimator;

    // Kéo thả hiệu ứng hạt nổ (Particle System) vào đây (Tùy chọn)
    public GameObject explosionEffectPrefab;

    // Hàm này được gọi khi một Collider 2D khác đi vào vùng Trigger của bom
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Giả sử người chơi của bạn có tag là "Player"
        if (other.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    void Explode()
    {
        // 1. Kích hoạt Animation Nổ
        if (bombAnimator != null)
        {
            // Đặt biến Trigger trong Animator để chuyển sang trạng thái "Explode"
            bombAnimator.SetTrigger("Explode");
        }

        // 2. Tạo hiệu ứng hạt nổ (nếu có)
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        // 3. Xử lý sát thương người chơi (Tùy thuộc vào game của bạn)
        // Ví dụ: other.GetComponent<PlayerHealth>().TakeDamage(100);

        // 4. Hủy đối tượng bom sau khi nổ một thời gian ngắn (để animation nổ kịp chạy)
        // Ví dụ: Bom sẽ biến mất sau 0.5 giây
        Destroy(gameObject, 0.5f);
    }
}