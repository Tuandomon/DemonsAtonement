using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    [Header("Hiệu ứng bay lên")]
    public float floatSpeed = 1f;          // tốc độ bay lên
    public float lifeTime = 2f;            // thời gian tồn tại
    public float fadeDuration = 1f;        // thời gian fade cuối

    private SpriteRenderer spriteRenderer;
    private float elapsed = 0f;
    private Vector3 moveDirection;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        // Scale ngẫu nhiên từ 0.2 → 1
        float randomScale = Random.Range(0.2f, 1f);
        transform.localScale = Vector3.one * randomScale;

        // Rotation ngẫu nhiên
        float randomRotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);

        // Hướng bay lên (kèm x ngẫu nhiên nhỏ)
        moveDirection = new Vector3(Random.Range(-0.3f, 0.3f), 1f, 0f).normalized;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        // Bay lên
        transform.position += moveDirection * floatSpeed * Time.deltaTime;

        // Fade dần
        if (elapsed >= lifeTime - fadeDuration)
        {
            float fadeElapsed = elapsed - (lifeTime - fadeDuration);
            float alpha = Mathf.Lerp(1f, 0f, fadeElapsed / fadeDuration);
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                spriteRenderer.color = new Color(c.r, c.g, c.b, alpha);
            }
        }

        // Destroy khi hết thời gian
        if (elapsed >= lifeTime)
            Destroy(gameObject);
    }
}
