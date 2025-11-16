using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f;
    public int damage = 10; // ✅ Sát thương gây ra

    private float direction;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(float dir)
    {
        direction = dir;
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.velocity = new Vector2(direction * speed, 0);
        transform.localScale = new Vector3(direction, 1, 1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ✅ Nếu trúng Enemy
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject); // Đạn biến mất sau khi trúng
        }

        // ✅ Nếu trúng tường hoặc vật thể khác
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
