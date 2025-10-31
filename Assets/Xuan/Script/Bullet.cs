using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float maxDistance = 10f;
    public float speed = 20f;
    public int damage = 20;

    private Vector2 startPosition;
    private Rigidbody2D rb;

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(float direction)
    {
        direction = Mathf.Sign(direction); // -1 hoặc 1
        transform.localScale = new Vector3(direction, 1f, 1f);

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(direction * speed, 0f);
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
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}