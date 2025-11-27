using UnityEngine;

public class StarBoomSkill : MonoBehaviour
{
    public float speed = 0.5f;          // tốc độ di chuyển
    public float moveDistance = 5f;   // khoảng cách tối đa
    public int damage = 30;           // sát thương gây ra

    [Header("Prefab hiệu ứng nổ")]
    public GameObject explosionPrefab; // Prefab nổ riêng

    private Vector2 direction;
    private Vector2 startPos;
    private bool exploded = false;

    void Start()
    {
        startPos = transform.position;
    }

    // Hàm khởi tạo hướng từ Player
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        if (exploded) return; // nếu đã nổ thì không di chuyển nữa

        // Di chuyển theo hướng
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        // Nếu đi quá khoảng cách cho phép thì tự hủy (không cần va chạm Enemy)
        if (Vector2.Distance(startPos, transform.position) >= moveDistance)
        {
            Destroy(gameObject); // chỉ biến mất, không nổ
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (exploded) return;

        if (collision.CompareTag("Enemy"))
        {
            // Gây damage
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Kích hoạt nổ ngay tại vị trí va chạm
            TriggerExplosion();
        }
    }

    void TriggerExplosion()
    {
        exploded = true;

        // Triệu hồi prefab nổ tại vị trí hiện tại
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Hủy StarBoom sau khi tạo hiệu ứng nổ
        Destroy(gameObject);
    }
}