using System.Collections;
using UnityEngine;

public class PlayerLightningHit : MonoBehaviour
{
    [Header("Effects")]
    public GameObject headEffectPrefab;     // Hiệu ứng lóe trên đầu
    public GameObject thunderEffectPrefab;  // Hiệu ứng sét xung quanh

    [Header("Hit Settings")]
    public float stunDuration = 3f;         // Thời gian đứng im

    private bool isHandlingHit = false;
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
        {
            Debug.LogWarning("PlayerController không tìm thấy trên player!");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lightning") && !isHandlingHit)
        {
            StartCoroutine(HandleLightningHit());
        }
    }

    private IEnumerator HandleLightningHit()
    {
        isHandlingHit = true;

        // 1️⃣ Hiển thị effect trên đầu
        if (headEffectPrefab != null)
        {
            GameObject headEffect = Instantiate(headEffectPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            headEffect.transform.SetParent(transform); // gắn theo player
            Destroy(headEffect, stunDuration);
        }

        // 2️⃣ Hiển thị effect sét xung quanh
        if (thunderEffectPrefab != null)
        {
            GameObject thunderEffect = Instantiate(thunderEffectPrefab, transform.position, Quaternion.identity);
            thunderEffect.transform.SetParent(transform); // gắn theo player
            Destroy(thunderEffect, stunDuration);
        }

        // 3️⃣ Gọi stun của PlayerController
        if (playerController != null)
        {
            playerController.ApplyStun(stunDuration);
        }

        // 4️⃣ Chờ hết thời gian stun
        yield return new WaitForSeconds(stunDuration);

        isHandlingHit = false;
    }
}
