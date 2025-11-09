using UnityEngine;

public class FireBall : MonoBehaviour
{
    [Header("FireBall Settings")]
    public float speed = 6f;
    public float lifeTime = 5f;
    public int damage = 10;

    private Vector2 moveDirection;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            Vector2 targetPos = playerObj.transform.position;
            moveDirection = (targetPos - (Vector2)transform.position).normalized;

            // Xoay FireBall về hướng player
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Nếu sprite mặc định quay về phải, đứng bên trái cần flip
            if (moveDirection.x < 0)
            {
                Vector3 scale = transform.localScale;
                scale.y *= -1;  // hoặc scale.x *= -1 tùy sprite
                transform.localScale = scale;
            }
        }
        else
        {
            moveDirection = Vector2.right;
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)moveDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);

            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                 collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
