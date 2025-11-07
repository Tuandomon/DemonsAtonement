using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ruong : MonoBehaviour
{
    [Header("Vật phẩm rơi ra")]
    public GameObject itemPrefab;       // vật phẩm sẽ xuất hiện
    public Transform spawnPoint;        // vị trí spawn (có thể để trống -> lấy vị trí rương)

    [Header("Trạng thái rương")]
    public Sprite closedChest;          // hình rương đóng
    public Sprite openChest;            // hình rương mở

    private bool isOpened = false;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null && closedChest != null)
            sr.sprite = closedChest;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Khi vũ khí (tag PlayerWeapon) chạm vào
        if (!isOpened && other.CompareTag("Player"))
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpened = true;

        // đổi sprite sang rương mở
        if (sr != null && openChest != null)
            sr.sprite = openChest;

        // tạo vật phẩm
        Vector3 spawnPos = spawnPoint ? spawnPoint.position : transform.position + Vector3.up * 0.5f;
        if (itemPrefab != null)
            Instantiate(itemPrefab, spawnPos, Quaternion.identity);

        // có thể thêm hiệu ứng âm thanh hoặc particle ở đây
        // Destroy(gameObject, 1.5f); // nếu muốn rương biến mất sau khi mở
    }
}
