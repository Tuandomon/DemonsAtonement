using UnityEngine;

public class ItemChangeStarBoom : MonoBehaviour
{
    [Header("Hiệu ứng đi theo Player")]
    public GameObject effectPrefab;   // Prefab hiệu ứng đi theo Player

    [Header("Thời gian buff (giây)")]
    public float buffDuration = 30f;  // Thời gian buff

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerAttack attack = collision.GetComponent<PlayerAttack>();
            if (attack != null)
            {
                // Kích hoạt buff đổi skill
                attack.ActivateStarBoom(buffDuration);
            }

            // Tạo hiệu ứng đi theo Player
            if (effectPrefab != null)
            {
                GameObject effect = Instantiate(effectPrefab, collision.transform.position, Quaternion.identity);
                effect.transform.SetParent(collision.transform);
                Destroy(effect, buffDuration);
            }

            // Xóa item sau khi nhặt
            Destroy(gameObject);
        }
    }
}