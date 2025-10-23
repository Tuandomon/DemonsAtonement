using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float maxDistance = 10f;
    public float speed = 20f;
    public int damage = 20; // sát thương của viên đạn

    private Vector2 startPosition;
    private float direction = 1f;

    public void SetDirection(float dir)
    {
        direction = dir;
    }

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

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
            // Gây sát thương nếu enemy có EnemyHealth
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}