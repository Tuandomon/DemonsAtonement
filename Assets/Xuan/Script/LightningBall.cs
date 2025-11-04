using UnityEngine;

public class  LightningBall: MonoBehaviour
{
    public float speed = 8f;              // 🔹 Tốc độ bay
    public float lifetime = 2f;           // 🔹 Thời gian tồn tại trước khi tự hủy
    public int damage = 10;

    private Vector2 direction = Vector2.right;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        // Lật sprite nếu cần
        if (dir == Vector2.left)
            GetComponent<SpriteRenderer>().flipX = true;
    }

    void Start()
    {
        Destroy(gameObject, lifetime); // 🔸 Tự hủy sau X giây
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime); // 🔸 Di chuyển liên tục
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
                player.TakeDamage(damage);

            Destroy(gameObject); // 🔸 Hủy khi trúng Player
        }
    }
}