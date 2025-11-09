using UnityEngine;

public class SpiritFollowBoss : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform boss;              // Boss để bám theo
    public Vector3 offset = new Vector3(1.5f, 0, 0); // Khoảng cách so với boss
    public float moveSpeed = 5f;

    [Header("Fireball Settings")]
    public GameObject fireballPrefab;
    public Transform firePoint;
    public float shootInterval = 2f;
    public float fireballSpeed = 5f;

    private Transform player;
    private float shootTimer = 0f;
    private bool facingRight = true;

    void Start()
    {
        if (boss == null)
            Debug.LogError("Chưa gán boss cho Spirit!");

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("Không tìm thấy Player!");
    }

    void Update()
    {
        if (boss == null || player == null) return;

        // ================== Bám theo boss ==================
        Vector3 targetPos = boss.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);

        // Xoay sprite theo hướng boss
        if (boss.localScale.x > 0 && !facingRight)
            Flip();
        else if (boss.localScale.x < 0 && facingRight)
            Flip();

        // ================== Bắn Fireball ==================
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootInterval)
        {
            ShootFireball();
            shootTimer = 0f;
        }
    }

    void ShootFireball()
    {
        if (fireballPrefab == null || firePoint == null || player == null) return;

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        // Tính hướng bay về player
        Vector3 direction = (player.position - firePoint.position).normalized;

        Rigidbody2D rb2d = fireball.GetComponent<Rigidbody2D>();
        if (rb2d != null)
            rb2d.velocity = direction * fireballSpeed;
        else
        {
            Rigidbody rb3d = fireball.GetComponent<Rigidbody>();
            if (rb3d != null)
                rb3d.velocity = direction * fireballSpeed;
        }

        // --- CHỈNH HƯỚNG FIREBALL LUÔN 0 ---
        Vector3 scale = fireball.transform.localScale;
        scale.y = Mathf.Abs(scale.y); // ép scale Y dương
        fireball.transform.localScale = scale;

        // Tự hủy sau 5 giây
        Destroy(fireball, 5f);
    }




    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
