using UnityEngine;
using System.Collections; // Cần thiết để sử dụng Coroutine

public class TemporaryPlatform : MonoBehaviour
{
    [Header("Timing Settings")]
    public float timeBeforeDrop = 0.5f;     // Thời gian chờ trước khi sàn rơi (3 giây)
    public float timeToRestore = 5f;      // Thời gian chờ để sàn hồi phục (5 giây)

    [Header("Component Settings")]
    private Vector3 originalPosition;       // Vị trí ban đầu của sàn
    private Rigidbody2D rb;
    private Collider2D platformCollider;

    void Start()
    {
        // Lưu lại vị trí ban đầu
        originalPosition = transform.position;
        
        // Lấy các Components cần thiết
        rb = GetComponent<Rigidbody2D>();
        platformCollider = GetComponent<Collider2D>();

        // Đảm bảo Rigidbody2D không bị ảnh hưởng bởi vật lý ban đầu
        if (rb != null)
        {
            rb.isKinematic = true;  // Không bị ảnh hưởng bởi lực bên ngoài
            rb.gravityScale = 0;    // Không bị ảnh hưởng bởi trọng lực
        }

        // Đảm bảo Platform có Tag là "Ground"
        if (gameObject.tag != "Ground")
        {
            Debug.LogWarning("Platform thiếu Tag 'Ground' hoặc Tag bị đặt sai!");
        }
    }

    // Hàm này được gọi khi một vật thể chạm vào (Collision)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Chỉ kích hoạt khi Player chạm vào
        if (collision.gameObject.CompareTag("Player"))
        {
            // Quan trọng: Kiểm tra xem Player có đang đứng trên đỉnh sàn không
            // Giả sử Player có Collider nằm ngay trên sàn
            if (collision.contacts.Length > 0)
            {
                // Nếu người chơi chạm từ phía trên (Vector lên của sàn gần giống Vector va chạm)
                Vector2 contactPoint = collision.contacts[0].point;
                Vector2 platformUp = transform.up;
                
                // Kiểm tra đơn giản: nếu điểm va chạm gần phía trên của sàn
                // Nếu bạn đang dùng 2D, một kiểm tra đơn giản là đủ
                // Chúng ta sẽ bỏ qua kiểm tra hướng để đơn giản hóa, chỉ cần Player chạm
                
                // Bắt đầu đếm ngược nếu chưa rơi
                StartCoroutine(DropAndRestore());
            }
        }
    }

    private IEnumerator DropAndRestore()
    {
        // *** TRẠNG THÁI 1: CHỜ RƠI (3 giây) ***
        
        // Chờ 3 giây trước khi rơi
        yield return new WaitForSeconds(timeBeforeDrop); 

        // 1. Kích hoạt vật lý: Bắt đầu rơi
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.gravityScale = 1; // Áp dụng trọng lực để sàn rơi
        }

        // Tắt Collider để Player không thể đứng trên sàn khi nó rơi
        platformCollider.enabled = false; 

        // *** TRẠNG THÁI 2: CHỜ HỒI PHỤC (5 giây) ***
        
        // Chờ 5 giây để hồi phục
        yield return new WaitForSeconds(timeToRestore); 

        // 2. Hủy Rigidbody2D và đặt lại vị trí
        if (rb != null)
        {
            // Đặt lại các thuộc tính vật lý
            rb.velocity = Vector2.zero; // Dừng mọi chuyển động
            rb.isKinematic = true;
            rb.gravityScale = 0;
            
            // Đặt lại vị trí ban đầu
            transform.position = originalPosition;
        }
        
        // 3. Kích hoạt lại Collider
        platformCollider.enabled = true; 
    }
}