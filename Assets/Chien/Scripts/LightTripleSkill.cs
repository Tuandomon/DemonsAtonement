using UnityEngine;

public class LightTripleSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    public GameObject lightProjectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float angleSpread = 15f; // góc lệch 15 độ trái/phải

    public void CastTripleLight()
    {
        if (lightProjectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("Thiếu prefab hoặc firePoint!");
            return;
        }

        // Bắn 3 quả cầu: giữa - trái - phải
        ShootProjectile(0f);                  // Giữa
        ShootProjectile(-angleSpread);        // Trái
        ShootProjectile(angleSpread);         // Phải
    }

    private void ShootProjectile(float angleOffset)
    {
        // Tạo viên đạn tại firePoint
        GameObject projectile = Instantiate(lightProjectilePrefab, firePoint.position, Quaternion.identity);

        // Tính hướng bắn dựa trên hướng firePoint
        Vector2 direction = firePoint.right; // hướng bắn mặc định là bên phải

        // Quay hướng theo góc lệch
        direction = Quaternion.Euler(0, 0, angleOffset) * direction;

        // Gán vận tốc cho projectile
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = direction.normalized * projectileSpeed;
        }
    }
}
