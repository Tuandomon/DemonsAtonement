using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float shootCooldown = 2f;

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
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (firePoint != null && Time.time >= lastShootTime + shootCooldown)
            {
                Shoot();
                lastShootTime = Time.time;
            }
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Xác định hướng mặt của player
        float direction = transform.localScale.x > 0 ? 1f : -1f;

        // Truyền hướng vào Bullet
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetDirection(direction);
    }
}