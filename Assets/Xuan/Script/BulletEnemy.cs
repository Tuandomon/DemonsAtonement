using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    public float maxDistance = 10f;
    public float speed = 20f;
    public int damage = 20;

    private Vector2 startPosition;
    private float direction = 1f;
    private Rigidbody2D rb;

    public void SetDirection(float dir)
    {
        direction = Mathf.Sign(dir); // Đảm bảo chỉ là -1 hoặc 1
        transform.localScale = new Vector3(direction, 1, 1); // Lật sprite nếu cần
    }

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = Vector2.right * direction * speed;
        }
    }

    void Update()
    {
        float distanceTraveled = Vector2.Distance(transform.position, startPosition);
        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Đạn enemy gây " + damage + " sát thương cho Player.");
            }

            Destroy(gameObject);
        }
    }
}