using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackCooldown = 1f; // Thời gian hồi chiêu giữa các lần đánh
    private float lastAttackTime = -Mathf.Infinity;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Kiểm tra nhấn chuột trái và đủ thời gian hồi chiêu
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            FlipToMouse();     // Xoay hướng theo chuột
            Attack();          // Gọi animation và logic tấn công
        }
    }

    void FlipToMouse()
    {
        // Lấy vị trí chuột trong thế giới
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // So sánh vị trí chuột với vị trí nhân vật
        float direction = mouseWorldPos.x - transform.position.x;

        // Lật sprite nếu chuột bên trái
        spriteRenderer.flipX = direction < 0;
    }

    void Attack()
    {
        // Kích hoạt animation tấn công
        animator.SetTrigger("attack");

        // Ghi lại thời gian tấn công
        lastAttackTime = Time.time;

        // Viết thêm logic gây sát thương nếu cần
        Debug.Log("Player tấn công!");
    }
}