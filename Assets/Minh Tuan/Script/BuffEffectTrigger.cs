using UnityEngine;

public class BuffEffectTrigger2D : MonoBehaviour
{
    [Tooltip("Prefab hiệu ứng cường hóa (2D/3D Effect Prefab)")]
    public GameObject buffEffectPrefab;

    [Tooltip("Thời gian tồn tại của hiệu ứng (giây)")]
    public float effectDuration = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Tạo hiệu ứng tại vị trí của player
            GameObject effectInstance = Instantiate(buffEffectPrefab, collision.transform.position, Quaternion.identity);

            // Gắn hiệu ứng vào player để nó di chuyển theo
            effectInstance.transform.SetParent(collision.transform);

            // Hủy hiệu ứng sau một khoảng thời gian
            Destroy(effectInstance, effectDuration);

            // Gọi hàm kích hoạt buff trong PlayerShooting
            PlayerShooting shooter = collision.GetComponent<PlayerShooting>();
            if (shooter != null)
            {
                shooter.ActivateBuff(effectDuration);
            }

            // Hủy item sau khi đã được nhặt
            Destroy(gameObject);
        }
    }
}