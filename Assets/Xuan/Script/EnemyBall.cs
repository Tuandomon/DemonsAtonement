using UnityEngine;

public class EnemyBall : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float shootCooldown = 2f;
    public float detectionRange = 10f;

    private float lastShootTime;
    private bool hasShot = false;

    void Start()
    {
        lastShootTime = Time.time; // Khởi tạo thời gian bắn để tránh bắn ngay khi bắt đầu

        if (firePoint == null)
        {
            GameObject found = GameObject.Find("FirePoint");
            if (found != null)
            {
                firePoint = found.transform;
            }
            else
            {
                Debug.LogWarning("Không tìm thấy GameObject tên 'FirePoint'.");
            }
        }
    }

    void Update()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange);
        bool playerDetected = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                playerDetected = true;

                if (!hasShot)
                {
                    Shoot(hit.transform);
                    hasShot = true;
                    lastShootTime = Time.time;
                }

                break;
            }
        }

        // Reset trạng thái sau khi cooldown kết thúc
        if (hasShot && Time.time >= lastShootTime + shootCooldown)
        {
            hasShot = false;
        }
    }


    void Shoot(Transform target)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Vector2 direction = (target.position - firePoint.position).normalized;

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetDirection(direction.x);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}