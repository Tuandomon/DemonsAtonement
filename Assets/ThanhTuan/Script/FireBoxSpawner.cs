using UnityEngine;
using System.Collections;

public class FireBoxSpawner : MonoBehaviour
{
    [Header("Thiết lập Prefab và Vị trí")]
    // Kéo thả prefab FireHazard đã tạo vào đây
    public GameObject fireHazardPrefab;
    // Vị trí FireHazard sẽ được tạo (thường là ngay trên Fire Box)
    public Transform spawnPoint;

    [Header("Cài đặt Chu kỳ")]
    // Thời gian ngọn lửa tồn tại (Có thể setting)
    public float fireDuration = 2f;
    // Thời gian bẫy nghỉ giữa các lần phun lửa
    public float cooldownDuration = 3f;

    private GameObject currentFire;

    void Start()
    {
        // Bắt đầu chu kỳ hoạt động của bẫy
        StartCoroutine(FireCycle());
    }

    IEnumerator FireCycle()
    {
        while (true) // Lặp vô hạn
        {
            // 1. CHỜ: Thời gian nghỉ (Cooldown)
            Debug.Log("Bẫy đang trong thời gian Cooldown...");
            yield return new WaitForSeconds(cooldownDuration);

            // 2. KÍCH HOẠT: Triệu hồi Nguy hiểm Lửa
            if (fireHazardPrefab != null && spawnPoint != null)
            {
                Debug.Log("Bẫy được KÍCH HOẠT!");
                // Triệu hồi ngọn lửa tại vị trí đã định
                currentFire = Instantiate(fireHazardPrefab, spawnPoint.position, Quaternion.identity);

                // Nếu có animation/hiệu ứng trên Fire Box, bạn có thể kích hoạt ở đây.
            }

            // 3. THỜI GIAN LỬA TỒN TẠI: Gây sát thương
            yield return new WaitForSeconds(fireDuration);

            // 4. TẮT: Hủy Ngọn Lửa
            if (currentFire != null)
            {
                Destroy(currentFire);
                Debug.Log("Bẫy đã TẮT!");
            }
        }
    }
}