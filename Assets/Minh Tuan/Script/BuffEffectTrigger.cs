using UnityEngine;

public class BuffEffectTrigger2D : MonoBehaviour
{
    // Thời gian tồn tại tối đa của vật phẩm trên map
    public float lifetime = 0.5f;

    void Start()
    {
        // Tự hủy GameObject này sau 'lifetime' giây
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Inventory inventory = collision.GetComponent<Inventory>();

            if (inventory != null)
            {
                bool added = inventory.AddBuffItem();

                // Nếu nhặt thành công, hủy ngay lập tức
                if (added)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}