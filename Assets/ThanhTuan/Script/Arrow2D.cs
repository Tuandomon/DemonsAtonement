using UnityEngine;

public class Arrow2D : MonoBehaviour
{
    // Dữ liệu được gán từ ArrowTrap2D.cs
    [HideInInspector] public float damageAmount;
    [HideInInspector] public float speed;
    [HideInInspector] public Vector3 direction; // Vẫn dùng Vector3 cho dễ tương thích với Transform.right

    private Rigidbody2D rb;
    private float lifeTime = 4f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // Hủy vật thể sau lifeTime
        Destroy(gameObject, lifeTime);

        // Gán vận tốc ban đầu cho Rigidbody2D
        if (rb != null)
        {
            // direction.x sẽ quyết định bắn sang trái (-1) hay phải (1)
            rb.velocity = new Vector2(direction.x * speed, 0f);
        }
    }

    // Xử lý va chạm 2D
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem va chạm có phải là Player không
        if (other.gameObject.CompareTag("Player"))
        {
            // Gây sát thương
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }

            // Mũi tên biến mất sau khi trúng Player
            Destroy(gameObject);
        }
        // Kiểm tra va chạm với Tường/Môi trường (sử dụng tag tương ứng)
        else if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Environment"))
        {
            // Mũi tên biến mất khi đụng tường
            Destroy(gameObject);
        }
    }

    // Nếu bạn muốn mũi tên biến mất khi chạm bất kỳ vật thể nào KHÔNG phải là Player
    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
    */
}