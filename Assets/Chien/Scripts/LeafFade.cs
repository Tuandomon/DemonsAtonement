using UnityEngine;

public class LeafFade : MonoBehaviour
{
    public float fallSpeed = 1f;
    public float fadeSpeed = 1f;

    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        // Random xoay nhẹ cho tự nhiên
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-25f, 25f));

        // Random tốc độ rơi
        fallSpeed *= Random.Range(0.8f, 1.3f);
    }

    void Update()
    {
        // Rơi xuống
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // Tự mờ dần
        originalColor.a -= fadeSpeed * Time.deltaTime;
        sr.color = originalColor;

        // Nếu mờ hết → xóa
        if (originalColor.a <= 0f)
            Destroy(gameObject);
    }
}
