using UnityEngine;
using System.Collections;

public class MinionLifetime : MonoBehaviour
{
    [Header("Lifetime Settings")]
    public float lifetime = 15f; // Thời gian Minion tồn tại (15 giây theo yêu cầu)

    // Tùy chọn: Hiệu ứng khi Minion biến mất
    public GameObject disappearEffectPrefab;

    void Start()
    {
        // Bắt đầu đếm ngược ngay khi Minion được sinh ra
        StartCoroutine(MinionDeathTimer());
    }

    IEnumerator MinionDeathTimer()
    {
        // Chờ hết thời gian tồn tại
        yield return new WaitForSeconds(lifetime);

        // Kích hoạt hiệu ứng biến mất (nếu có)
        if (disappearEffectPrefab != null)
        {
            // Sinh ra hiệu ứng tại vị trí Minion
            Instantiate(disappearEffectPrefab, transform.position, Quaternion.identity);
        }

        Debug.Log(gameObject.name + " đã tự động biến mất sau " + lifetime + " giây.");

        // Hủy đối tượng Minion
        Destroy(gameObject);
    }
}