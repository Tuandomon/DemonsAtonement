using UnityEngine;

public class ItemBuffAttackEffect : MonoBehaviour
{
    [Header("Hiệu ứng đi theo Player")]
    public GameObject effectPrefab;   // Prefab hiệu ứng đi theo Player

    [Header("Thời gian hiệu ứng (giây)")]
    public float effectDuration = 30f;  // Thời gian tồn tại hiệu ứng

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Tạo hiệu ứng đi theo Player
            if (effectPrefab != null)
            {
                GameObject effect = Instantiate(effectPrefab, collision.transform.position, Quaternion.identity);
                effect.transform.SetParent(collision.transform);
                Destroy(effect, effectDuration); // Hủy hiệu ứng sau khi hết thời gian
            }

            // Xóa item sau khi nhặt
            Destroy(gameObject);
        }
    }
}