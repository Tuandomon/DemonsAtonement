using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashPickup : MonoBehaviour
{
    [Header("Visual Effects")]
    public GameObject collectEffect;
    public float rotateSpeed = 90f;

    [Header("Sound")]
    public AudioClip collectSound;

    [Header("Animation")]
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1f;

    private Vector3 startPos;
    private bool isCollected = false;

    void Start()
    {
        // Lưu vị trí ban đầu để tạo hiệu ứng nổi
        startPos = transform.position;
    }

    void Update()
    {
        if (isCollected) return;

        // Xoay item để thu hút người chơi
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

        // Hiệu ứng nổi lên xuống
        FloatAnimation();
    }

    void FloatAnimation()
    {
        // Tạo hiệu ứng nổi bằng Mathf.Sin
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            CollectItem(other.gameObject);
        }
    }

    void CollectItem(GameObject player)
    {
        isCollected = true;

        // Hiệu ứng khi nhặt item
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // Âm thanh khi nhặt item
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // Tự động kích hoạt dash vĩnh viễn cho player
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.UnlockDashPermanent();
        }
        else
        {
            Debug.LogWarning("❌ Không tìm thấy PlayerController trên player!");
        }

        // Tự động biến mất
        Destroy(gameObject);

        Debug.Log("🎯 Item dash đã được thu thập!");
    }

    // Hiển thị phạm vi trigger trong Scene (tuỳ chọn)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}