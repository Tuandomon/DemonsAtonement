using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float maxDistance = 10f;
    public float speed = 20f;

    private Vector2 startPosition;
    private float direction = 1f; // mặc định bên phải

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
        // Di chuyển theo hướng mặt
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        // Kiểm tra khoảng cách
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
            Destroy(gameObject);
        }
    }
}