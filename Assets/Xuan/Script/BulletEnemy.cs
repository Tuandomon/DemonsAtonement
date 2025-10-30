using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    public float maxDistance = 10f;
    public float speed = 20f;
    public int damage = 20;

    private Vector2 startPosition;
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private Animator animator;
    private bool hasExploded = false;

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
        animator = GetComponent<Animator>();

        if (rb != null)
        {
            rb.velocity = moveDirection * speed;
        }
    }

    void Update()
    {
        if (hasExploded) return;

        float distanceTraveled = Vector2.Distance(transform.position, startPosition);
        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded) return;

        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Đạn enemy gây " + damage + " sát thương cho Player.");
            }

            Explode();
        }

        PlayerController playerController = collision.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.Stun(1f); // stun 1 giây
        }
    }

    void Explode()
    {
        hasExploded = true;

        // Dừng chuyển động
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        // Kích hoạt animation Boom
        if (animator != null)
        {
            animator.SetTrigger("Boom");
        }

        // Hủy sau thời gian animation Boom (ví dụ 0.5s)
        Destroy(gameObject, 0.5f);
    }
}