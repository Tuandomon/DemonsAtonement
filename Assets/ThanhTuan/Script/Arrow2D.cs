using UnityEngine;

public class Arrow2D : MonoBehaviour
{
    [Header("Thiết lập mũi tên")]
    public float speed = 10f;
    public float damageAmount = 10f;
    public Vector2 direction;

    [Header("Hiệu ứng")]
    public GameObject hitEffect;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Bắn theo hướng đã nhận
        rb.velocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra player
        if (collision.CompareTag("Player"))
        {
            // Lấy script máu player
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damageAmount);
            }

            // Tạo hiệu ứng va chạm
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }

        // Nếu trúng tường thì biến mất
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}