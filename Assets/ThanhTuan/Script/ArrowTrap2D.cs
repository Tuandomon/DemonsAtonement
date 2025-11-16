using UnityEngine;
using System.Collections;

public class ArrowTrap2D : MonoBehaviour
{
    [Header("Cài đặt Prefab")]
    public GameObject arrowPrefab;
    public Transform firePoint;

    [Header("Cài đặt Âm thanh")]
    public AudioSource audioSource;
    public AudioClip shootSound;

    [Header("Cài đặt Bắn")]
    public float minFireDelay = 3f;
    public float maxFireDelay = 5f;
    public float arrowSpeed = 15f;

    void Start()
    {
        StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            float randomDelay = Random.Range(minFireDelay, maxFireDelay);
            yield return new WaitForSeconds(randomDelay);

            ShootArrow();
        }
    }

    void ShootArrow()
    {
        if (arrowPrefab == null || firePoint == null)
        {
            Debug.LogError("Chưa gán Arrow Prefab hoặc FirePoint!");
            return;
        }

        // Tạo mũi tên
        GameObject arrowObject = Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);

        // Lấy script Arrow2D
        Arrow2D arrow = arrowObject.GetComponent<Arrow2D>();

        if (arrow == null)
        {
            Debug.LogError("Arrow Prefab KHÔNG có script Arrow2D!");
            Destroy(arrowObject);
            return;
        }

        // Gán thông số
        arrow.damageAmount = 10f;
        arrow.speed = arrowSpeed;

        // Quan trọng → hướng bay = hướng firePoint đang xoay
        arrow.direction = firePoint.right.normalized;

        // Phát âm thanh
        if (audioSource != null && shootSound != null)
            audioSource.PlayOneShot(shootSound);
    }
}
