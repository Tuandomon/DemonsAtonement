using UnityEngine;
using System.Collections;

public class TemporaryPlatform : MonoBehaviour
{
    [Header("Timing Settings")]
    public float timeBeforeDrop = 0.5f;     // Thời gian chờ trước khi sàn rơi
    public float timeToRestore = 5f;        // Thời gian chờ để sàn hồi phục

    [Header("Audio Settings")]
    public AudioClip dropSound;             // Tệp âm thanh RƠI (Kéo file .wav/.mp3 vào Inspector)
    private AudioSource audioSource;        // Thành phần Audio Source

    [Header("Component Settings")]
    private Vector3 originalPosition;       // Vị trí ban đầu của sàn
    private Rigidbody2D rb;
    private Collider2D platformCollider;

    void Start()
    {
        originalPosition = transform.position;

        rb = GetComponent<Rigidbody2D>();
        platformCollider = GetComponent<Collider2D>();

        // Lấy Audio Source (Phải có Audio Source trên Game Object này)
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Thiếu Audio Source! Hãy thêm Audio Source Component vào Game Object này.");
        }

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.gravityScale = 0;
        }

        // Tùy chọn: Đặt sẵn Clip để đảm bảo Audio Source luôn sẵn sàng
        if (audioSource != null && dropSound != null)
        {
            audioSource.clip = dropSound;
        }
    }

    // CHÚ Ý: Hàm này chỉ gọi StartCoroutine MỘT LẦN!
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem Player có chạm vào và bẫy đang ở trạng thái sẵn sàng (Kinematic = true)
        if (collision.gameObject.CompareTag("Player") && rb != null && rb.isKinematic == true)
        {
            // *Lưu ý: Bạn có thể thêm kiểm tra hướng va chạm ở đây để đảm bảo Player đứng trên bẫy*
            StartCoroutine(DropAndRestore());
        }
    }

    private IEnumerator DropAndRestore()
    {
        // *** TRẠNG THÁI 1: CHỜ RƠI ***

        // Chờ thời gian đếm ngược
        yield return new WaitForSeconds(timeBeforeDrop);

        // ----------------------------------------------------
        // ⭐ KÍCH HOẠT ÂM THANH RƠI (Sử dụng PlayOneShot)
        // PlayOneShot sẽ phát âm thanh MỘT LẦN duy nhất và 
        // không làm hỏng logic lặp lại hay làm các bẫy khác phát theo
        // ----------------------------------------------------
        if (audioSource != null && dropSound != null)
        {
            // Lỗi 1: Khắc phục. Dùng PlayOneShot để phát âm thanh cục bộ.
            audioSource.PlayOneShot(dropSound);
        }

        // 1. Kích hoạt vật lý: Bắt đầu rơi
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.gravityScale = 1;
        }

        platformCollider.enabled = false;

        // *** TRẠNG THÁI 2: CHỜ HỒI PHỤC ***

        // Chờ thời gian hồi phục
        yield return new WaitForSeconds(timeToRestore);

        // 2. Hủy Rigidbody2D và đặt lại vị trí
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true; // Sẵn sàng cho lần rơi tiếp theo
            rb.gravityScale = 0;

            transform.position = originalPosition;
        }

        // 3. Kích hoạt lại Collider
        platformCollider.enabled = true;

        // Lỗi 2: Khắc phục. Không cần gán lại clip vì đã dùng PlayOneShot, 
        // nhưng Audio Source luôn sẵn sàng để nhận lệnh PlayOneShot tiếp theo.
    }
}