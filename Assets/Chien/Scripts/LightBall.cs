using UnityEngine;

public class LightBall : MonoBehaviour
{
    [Header("LightBall Settings")]
    public float speed = 6f;           // Tốc độ bay
    public float lifeTime = 5f;        // Tự hủy sau bao lâu
    public int damage = 40;            // Sát thương khi trúng Player

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
            // Nếu không tìm thấy player thì bay thẳng sang phải
            moveDirection = Vector2.right;
        }

        // Xoay sprite theo hướng bay
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Tự hủy sau thời gian lifeTime
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Bay thẳng về hướng đã tính
        transform.position += (Vector3)moveDirection * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi trúng Player
        if (collision.CompareTag("Player"))
        {
            // Gọi TakeDamage của PlayerHealth
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            Debug.Log("LightBall trúng Player! Gây " + damage + " dame.");
            Destroy(gameObject);
        }
        // Khi trúng Ground (hoặc các vật thể có layer Ground)
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                 collision.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
