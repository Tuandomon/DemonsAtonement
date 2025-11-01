using UnityEngine;

public class LadderClimb : MonoBehaviour
{
    public float climbSpeed = 3f;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private Rigidbody2D rb;
    private bool isClimbing;
    private bool onLadderZone;
    private float vertical;
    private float horizontal;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        // Cho phép nhảy khi đang ở cầu thang
        if (isClimbing && Input.GetButtonDown("Jump"))
        {
            ExitLadder();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Nếu người chơi ở vùng cầu thang và nhấn phím lên/xuống => bắt đầu leo
        if (onLadderZone && Mathf.Abs(vertical) > 0.1f)
        {
            isClimbing = true;
        }
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(horizontal * moveSpeed, vertical * climbSpeed);
        }
        else
        {
            rb.gravityScale = 1f;
            rb.velocity = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            onLadderZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            onLadderZone = false;
            ExitLadder();
        }
    }

    private void ExitLadder()
    {
        isClimbing = false;
        rb.gravityScale = 1f;
    }
}
