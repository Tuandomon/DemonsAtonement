using UnityEngine;

public class LightBall : MonoBehaviour
{
    [Header("LightBall Settings")]
    public float speed = 6f;           // Tốc độ bay
    public float lifeTime = 5f;        // Tự hủy sau bao lâu
    public int damage = 40;            // Sát thương khi trúng Player
    public float slowAmount = 0.5f;    // Giảm tốc (%)
    public float slowDuration = 3f;    // Thời gian giảm tốc

    private Vector2 moveDirection;     // Hướng bay cố định sau khi spawn

    void Start()
    {
        // Tìm Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            Vector2 targetPos = playerObj.transform.position;
            moveDirection = (targetPos - (Vector2)transform.position).normalized;
        }
        else
        {
            moveDirection = Vector2.right;
        }

        // Xoay sprite theo hướng bay
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Tự hủy sau lifeTime
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
            // Gây sát thương
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Giảm tốc bằng ApplySlow có sẵn trong PlayerController
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ApplySlow(slowAmount, slowDuration); // giảm tốc ngay lập tức
            }

            Debug.Log($"LightBall trúng Player! Gây {damage} dame và giảm tốc.");
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                 collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
