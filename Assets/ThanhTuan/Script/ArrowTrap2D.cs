using UnityEngine;
using System.Collections;

public class ArrowTrap2D : MonoBehaviour
{
    [Header("Cài đặt Prefab")]
    // Dùng để gắn prefab mũi tên từ Inspector
    public GameObject arrowPrefab;

    // Nơi mũi tên sẽ được tạo ra
    public Transform firePoint;

    [Header("Cài đặt Âm thanh")]
    public AudioSource audioSource;
    public AudioClip shootSound; // Kéo thả file âm thanh vào đây

    [Header("Cài đặt Bắn")]
    // Thời gian chờ tối thiểu và tối đa giữa các lần bắn
    public float minFireDelay = 3f;
    public float maxFireDelay = 5f;

    // Tốc độ bay của mũi tên
    public float arrowSpeed = 15f;

    void Start()
    {
        // Bắt đầu chu kỳ bắn tên liên tục
        StartCoroutine(ShootRoutine());
    }

    // Coroutine xử lý việc bắn liên tục với thời gian chờ ngẫu nhiên
    IEnumerator ShootRoutine()
    {
        while (true) // Lặp lại vô tận
        {
            // 1. Tính toán thời gian chờ ngẫu nhiên (3 - 5 giây)
            float randomDelay = Random.Range(minFireDelay, maxFireDelay);

            // 2. Chờ đợi
            yield return new WaitForSeconds(randomDelay);

            // 3. Thực hiện bắn
            ShootArrow();
        }
    }

    void ShootArrow()
    {
        if (arrowPrefab == null || firePoint == null)
        {
            Debug.LogError("Chưa gán Arrow Prefab hoặc Fire Point cho ArrowTrap!");
            return;
        }

        // 1. Tạo ra mũi tên tại vị trí và hướng quay của firePoint
        GameObject arrowObject = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);

        // 2. Lấy component Arrow2D và truyền dữ liệu
        Arrow2D arrowScript = arrowObject.GetComponent<Arrow2D>();
        if (arrowScript != null)
        {
            // Sát thương cố định: 10 máu
            arrowScript.damageAmount = 10f;
            arrowScript.speed = arrowSpeed;

            // Hướng bay ngang (firePoint.right hoạt động tốt trong 2D cho ngang)
            arrowScript.direction = firePoint.right;
        }
        else
        {
            Debug.LogError("Prefab mũi tên thiếu script 'Arrow2D'!");
            Destroy(arrowObject);
            return;
        }

        // 3. Phát âm thanh
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}