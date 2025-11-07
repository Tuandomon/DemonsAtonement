using UnityEngine;

public class DashPickup : MonoBehaviour
{
    [Header("Hiệu ứng & Âm thanh")]
    public GameObject collectEffect;    // Hiệu ứng khi ăn
    public AudioClip collectSound;      // Âm thanh khi ăn

    [Header("Chuyển động")]
    public float rotateSpeed = 90f;     // Tốc độ xoay
    public float floatAmplitude = 0.1f; // Biên độ nổi thấp hơn (dễ ăn)
    public float floatFrequency = 1f;   // Tốc độ nổi

    private Vector3 startPos;
    private bool isCollected = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (isCollected) return;

        // Xoay vật phẩm quanh trục Y
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);

        // Hiệu ứng nổi nhẹ
        float offsetY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPos.x, startPos.y + offsetY, startPos.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;
        if (!other.CompareTag("Player")) return;

        isCollected = true;

        // Hiệu ứng và âm thanh khi nhặt
        if (collectEffect != null)
            Instantiate(collectEffect, transform.position, Quaternion.identity);

        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        // Kích hoạt Dash cho nhân vật
        PlayerController playerController = other.GetComponent<PlayerController>();
        if (playerController != null)
            playerController.UnlockDashPermanent();

        // Biến mất ngay lập tức
        Destroy(gameObject);

        Debug.Log("⚡ Nhặt Dash item - biến mất ngay!");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
