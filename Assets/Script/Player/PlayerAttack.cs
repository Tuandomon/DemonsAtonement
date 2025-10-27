using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackCooldown = 1f;
    private float lastAttackTime = -Mathf.Infinity;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private bool isAttacking = false;
    private float moveInput;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Nếu đang tấn công thì không đọc phím trái/phải
        if (!isAttacking)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            moveInput = 0f; // Khóa di chuyển trái/phải
        }

        // Di chuyển trái/phải
        rb.velocity = new Vector2(moveInput * 5f, rb.velocity.y);

        // Tấn công nếu nhấn chuột trái và đủ thời gian hồi chiêu
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            FlipToMouse();
            Attack();
        }
    }

    void FlipToMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float direction = mouseWorldPos.x - transform.position.x;
        spriteRenderer.flipX = direction < 0;
    }

    void Attack()
    {
        animator.SetTrigger("attack");
        lastAttackTime = Time.time;
        isAttacking = true;
        Debug.Log("Player tấn công!");
    }

    // Gọi từ animation event khi kết thúc animation
    public void EndAttack()
    {
        isAttacking = false;
    }
}