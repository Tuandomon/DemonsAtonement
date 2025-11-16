using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Phím U")]
    public GameObject normalBulletPrefab;
    public GameObject buffedBulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float shootCooldown = 2f;

    [Header("Trạng thái buff")]
    public bool isBuffed = false;

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
        GameObject selectedBullet = isBuffed ? buffedBulletPrefab : normalBulletPrefab;

        GameObject bullet = Instantiate(selectedBullet, firePoint.position, Quaternion.identity);

        float direction = GetComponent<SpriteRenderer>().flipX ? -1f : 1f;

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetDirection(direction);
    }

    // Gọi hàm này từ script buff để kích hoạt trạng thái buff
    public void ActivateBuff(float duration)
    {
        StartCoroutine(BuffCoroutine(duration));
    }

    private System.Collections.IEnumerator BuffCoroutine(float duration)
    {
        isBuffed = true;
        yield return new WaitForSeconds(duration);
        isBuffed = false;
    }
}