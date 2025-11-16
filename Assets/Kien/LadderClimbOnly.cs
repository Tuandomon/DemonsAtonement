using UnityEngine;

public class LadderClimbOnly : MonoBehaviour
{
    [Header("Cài đặt leo cầu thang")]
    public float climbSpeed = 3f;  // Tốc độ leo

    private Rigidbody2D rb;
    private bool isClimbing;
    private bool onLadderZone;
    private float vertical;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        vertical = Input.GetAxisRaw("Vertical");

        // Khi ở vùng cầu thang và nhấn phím lên/xuống => bắt đầu leo
        if (onLadderZone && Mathf.Abs(vertical) > 0.1f)
        {
            isClimbing = true;
        }
        else if (!onLadderZone)
        {
            isClimbing = false;
        }
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(0f, vertical * climbSpeed);
        }
        else
        {
            rb.gravityScale = 1f;
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
            isClimbing = false;
            rb.gravityScale = 1f;
        }
    }
}
