using System.Collections;
using UnityEngine;

public class FireTrap : MonoBehaviour
{
    [Header("Thiết lập thời gian")]
    public float fireDuration = 2f;      // Thời gian phun lửa
    public float cooldownDuration = 3f;  // Thời gian nghỉ giữa các lần phun

    [Header("Gây sát thương")]
    public int damage = 20;

    [Header("Hiệu ứng phun lửa")]
    public GameObject fireEffect;        // Prefab hoặc animation lửa
    private bool isActive = false;

    private Collider2D fireCollider;     // Collider vùng lửa

    void Start()
    {
        fireCollider = GetComponent<Collider2D>();
        fireCollider.enabled = false;   // Ban đầu tắt vùng sát thương
        fireEffect.SetActive(false);    // Tắt hiệu ứng lửa

        StartCoroutine(FireCycle());
    }

    private IEnumerator FireCycle()
    {
        while (true)
        {
            // Bắt đầu phun lửa
            isActive = true;
            fireCollider.enabled = true;
            fireEffect.SetActive(true);

            yield return new WaitForSeconds(fireDuration);

            // Dừng phun
            isActive = false;
            fireCollider.enabled = false;
            fireEffect.SetActive(false);

            yield return new WaitForSeconds(cooldownDuration);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isActive && collision.CompareTag("Player"))
        {
            // Gọi hàm nhận sát thương (nếu có)
            PlayerHealth player = collision.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }
}
