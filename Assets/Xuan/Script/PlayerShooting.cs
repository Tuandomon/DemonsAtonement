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
    public bool isShootingKeyWActive = false;
    public bool isBuffActive = false;

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
        if (isShootingKeyWActive)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                TryShoot();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                TryShoot();
            }
        }
    }

    void TryShoot()
    {
        if (firePoint != null && Time.time >= lastShootTime + shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }

    void Shoot()
    {
        GameObject selectedBullet = isBuffed ? buffedBulletPrefab : normalBulletPrefab;

        GameObject bullet = Instantiate(selectedBullet, firePoint.position, Quaternion.identity);

        float direction = GetComponent<SpriteRenderer>().flipX ? -1f : 1f;

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction);
        }
    }

    public void ActivateBuff(float duration, bool changeShootKey)
    {
        StartCoroutine(BuffCoroutine(duration, changeShootKey));
    }

    private System.Collections.IEnumerator BuffCoroutine(float duration, bool changeShootKey)
    {
        isBuffActive = true;
        isBuffed = true;
        if (changeShootKey)
        {
            isShootingKeyWActive = true;
        }

        yield return new WaitForSeconds(duration);

        isBuffed = false;
        if (changeShootKey)
        {
            isShootingKeyWActive = false;
        }
        isBuffActive = false;
    }
}