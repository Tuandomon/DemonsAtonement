using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Thiết lập bắn đạn (phím U)")]
    public GameObject bulletPrefab;           // Prefab viên đạn
    public Transform firePoint;               // Vị trí bắn đạn
    public float bulletSpeed = 20f;           // Tốc độ bay của đạn
    public float shootCooldown = 0.5f;        // Thời gian giữa 2 lần bắn
    public AudioSource shootSound;            // Âm thanh khi bắn (tuỳ chọn)

    private float lastShootTime = -Mathf.Infinity;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Tự tìm FirePoint nếu chưa gán
        if (firePoint == null)
        {
            GameObject found = GameObject.Find("FirePoint");
            if (found != null)
                firePoint = found.transform;
            else
                Debug.LogWarning("⚠ Không tìm thấy GameObject tên 'FirePoint'.");
        }
    }

    void Update()
    {
        // Bấm phím U để bắn (chỉ khi không giữ W)
        if (!Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.U))
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
        // Tạo viên đạn
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Xác định hướng bắn theo hướng nhìn của nhân vật
        float direction = spriteRenderer != null && spriteRenderer.flipX ? -1f : 1f;

        // Cấu hình viên đạn
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction);
            bulletScript.speed = bulletSpeed;
        }

        // Phát âm thanh khi bắn (nếu có)
        if (shootSound != null)
            shootSound.Play();
    }
}
