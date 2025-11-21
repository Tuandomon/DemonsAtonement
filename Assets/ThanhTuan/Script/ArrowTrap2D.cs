using UnityEngine;
using System.Collections;

public class ArrowTrap2D : MonoBehaviour
{
    [Header("Cài đặt Prefab")]
    public GameObject arrowPrefab;
    public Transform firePoint;

    [Header("Cài đặt Âm thanh")]
    public AudioSource audioSource;
    public AudioClip shootSound;

    [Header("Cài đặt Bắn")]
    public float minFireDelay = 3f;
    public float maxFireDelay = 5f;
    public float arrowSpeed = 15f;

    [Header("Cài đặt Vùng kích hoạt")]
    public Collider2D triggerZone; // Vùng phát hiện player
    public bool playerInZone = false;

    [Header("Cài đặt Vùng tắt âm thanh")]
    public bool enableSoundZone = true;
    public Collider2D soundDisableZone; // Vùng Collider2D để tắt âm thanh

    void Start()
    {
        // Bắt đầu coroutine nhưng chỉ bắn khi có player
        StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            // Chỉ bắn khi player trong vùng
            if (playerInZone)
            {
                float randomDelay = Random.Range(minFireDelay, maxFireDelay);
                yield return new WaitForSeconds(randomDelay);

                ShootArrow();
            }
            else
            {
                // Nếu không có player, chờ 0.5s rồi kiểm tra lại
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    void ShootArrow()
    {
        if (arrowPrefab == null || firePoint == null)
        {
            Debug.LogError("Chưa gán Arrow Prefab hoặc FirePoint!");
            return;
        }

        // Tạo mũi tên
        GameObject arrowObject = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);

        // Lấy script Arrow2D
        Arrow2D arrow = arrowObject.GetComponent<Arrow2D>();

        if (arrow == null)
        {
            Debug.LogError("Arrow Prefab KHÔNG có script Arrow2D!");
            Destroy(arrowObject);
            return;
        }

        // Gán thông số
        arrow.damageAmount = 10f;
        arrow.speed = arrowSpeed;

        // Quan trọng → hướng bay = hướng firePoint đang xoay
        arrow.direction = firePoint.right.normalized;

        // Thêm component để xử lý âm thanh khi qua vùng
        ArrowSoundController soundController = arrowObject.AddComponent<ArrowSoundController>();
        soundController.SetSoundZone(soundDisableZone, enableSoundZone);

        // Phát âm thanh
        if (audioSource != null && shootSound != null)
            audioSource.PlayOneShot(shootSound);
    }

    // Phát hiện player vào vùng
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            Debug.Log("Player đã vào vùng kích hoạt");
        }
    }

    // Phát hiện player rời vùng
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            Debug.Log("Player đã rời vùng kích hoạt");
        }
    }
}

// Script để điều khiển âm thanh của mũi tên
public class ArrowSoundController : MonoBehaviour
{
    private Collider2D soundDisableZone;
    private bool enableSoundZone;
    private AudioSource arrowAudioSource;
    private bool soundEnabled = true;

    public void SetSoundZone(Collider2D zone, bool enable)
    {
        soundDisableZone = zone;
        enableSoundZone = enable;

        // Lấy AudioSource của mũi tên
        arrowAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!enableSoundZone || soundDisableZone == null || arrowAudioSource == null)
            return;

        // Kiểm tra nếu mũi tên nằm trong vùng tắt âm thanh
        bool isInZone = soundDisableZone.OverlapPoint(transform.position);

        if (isInZone && soundEnabled)
        {
            // Tắt âm thanh khi vào vùng
            arrowAudioSource.volume = 0f;
            soundEnabled = false;
        }
        else if (!isInZone && !soundEnabled)
        {
            // Bật âm thanh khi ra khỏi vùng
            arrowAudioSource.volume = 1f;
            soundEnabled = true;
        }
    }

    void OnDestroy()
    {
        // Đảm bảo dọn dẹp
        if (arrowAudioSource != null)
            arrowAudioSource.volume = 1f;
    }
}