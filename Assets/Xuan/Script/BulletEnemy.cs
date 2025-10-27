using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    public float maxDistance = 10f;
    public float speed = 20f;
    public int damage = 20;

    private Vector2 startPosition;
    private Vector2 moveDirection;
    private Rigidbody2D rb;

    public void SetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = moveDirection * speed;
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