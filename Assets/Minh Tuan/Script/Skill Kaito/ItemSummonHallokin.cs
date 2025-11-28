using UnityEngine;

public class ItemSummonHallokin : MonoBehaviour
{
    [Header("Pet được triệu hồi")]
    public GameObject petPrefab;      // Prefab pet để phụ đánh

    [Header("Thời gian tồn tại (giây)")]
    public float summonDuration = 30f;  // Thời gian pet tồn tại

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Triệu hồi pet tại vị trí Player
            if (petPrefab != null)
            {
                GameObject pet = Instantiate(petPrefab, collision.transform.position, Quaternion.identity);
                // Pet sẽ tự xử lý logic đi theo và đánh phụ bằng script gắn trên prefab
                Destroy(pet, summonDuration); // hủy pet sau thời gian tồn tại
            }

            // Xóa item sau khi nhặt
            Destroy(gameObject);
        }
    }
}