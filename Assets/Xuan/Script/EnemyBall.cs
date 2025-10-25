using UnityEngine;

public class EnemyBall : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float shootCooldown = 2f;
    public float detectionRange = 10f;
    public LayerMask playerLayer;
    public GameObject shootEffect;

    private float lastShootTime = -Mathf.Infinity;

    void Start()
    {
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
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (player != null && Time.time >= lastShootTime + shootCooldown)
        {
            Shoot(player.transform);
            lastShootTime = Time.time;
        }
    }

    void Shoot(Transform target)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Tính hướng từ Enemy đến Player
        Vector2 direction = (target.position - firePoint.position).normalized;

        // Truyền hướng vào Bullet
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetDirection(direction.x);

        // Gắn hiệu ứng nếu có
        if (shootEffect != null)
        {
            Instantiate(shootEffect, firePoint.position, Quaternion.identity);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}