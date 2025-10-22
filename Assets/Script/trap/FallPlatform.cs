using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class FallPlatform : MonoBehaviour
{
    [Header("Thời gian")]
    public float fallDelay = 0.25f;        // Thời gian chờ trước khi rơi
    public float destroyDelay = 0.2f;      // Thời gian chờ trước khi biến mất sau khi chạm đất

    [Header("Vật lý")]
    public float fallGravityScale = 2f;    // Độ rơi nhanh của platform
    public bool useImpulse = false;        // Nếu muốn platform bị “giật” nhẹ khi rơi
    public Vector2 fallImpulse = new Vector2(0, -2f);

    [Header("Hiệu ứng (tuỳ chọn)")]
    public bool fadeOutOnDestroy = true;   // Mờ dần trước khi biến mất
    public float fadeSpeed = 2f;           // Tốc độ mờ dần

    private Rigidbody2D rb;
    private Collider2D col;
    private Vector3 startPos;
    private Quaternion startRot;
    private bool isFalling = false;
    private bool hasTouchedGround = false;
    private Transform currentPlayerParent = null;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        startPos = transform.position;
        startRot = transform.rotation;

        // Bắt đầu ở trạng thái đứng yên
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Khi player chạm vào platform
        if (collision.transform.CompareTag("Player"))
        {
            // Gắn player làm con để rơi cùng platform
            collision.transform.SetParent(transform);
            currentPlayerParent = collision.transform;

            if (!isFalling)
                StartCoroutine(FallRoutine());
        }

        // Khi platform đang rơi và chạm đất
        if (isFalling && collision.transform.CompareTag("Ground") && !hasTouchedGround)
        {
            hasTouchedGround = true;
            StartCoroutine(DestroyAfterDelay());
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Khi player rời khỏi platform → gỡ parent
        if (collision.transform.CompareTag("Player") && collision.transform == currentPlayerParent)
        {
            collision.transform.SetParent(null);
            currentPlayerParent = null;
        }
    }

    IEnumerator FallRoutine()
    {
        isFalling = true;

        // Đợi trước khi rơi
        yield return new WaitForSeconds(fallDelay);

        // Bắt đầu rơi
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravityScale;

        if (useImpulse)
            rb.AddForce(fallImpulse, ForceMode2D.Impulse);
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destroyDelay);

        // Gỡ player nếu vẫn còn parent
        if (currentPlayerParent != null)
        {
            currentPlayerParent.SetParent(null);
            currentPlayerParent = null;
        }

        if (fadeOutOnDestroy)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                float alpha = 1f;
                while (alpha > 0f)
                {
                    alpha -= Time.deltaTime * fadeSpeed;
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
                    yield return null;
                }
            }
        }

        Destroy(gameObject); // Biến mất hoàn toàn
    }
}
