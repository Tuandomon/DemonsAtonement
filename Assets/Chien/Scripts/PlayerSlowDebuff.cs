using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlowDebuffs : MonoBehaviour
{
    private float originalSpeed = 0f;
    private float slowAmount = 0f;
    private float duration = 0f;
    private bool isSlowed = false;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyDebuff(float amount, float time)
    {
        // Nếu chưa bị giảm tốc, lấy tốc độ hiện tại
        if (!isSlowed)
        {
            originalSpeed = rb.velocity.magnitude; // dùng velocity tạm
        }

        slowAmount = amount;
        duration = time;

        if (!isSlowed)
        {
            StartCoroutine(SlowCoroutine());
        }
    }

    private IEnumerator SlowCoroutine()
    {
        isSlowed = true;

        // Giảm tốc
        rb.velocity *= (1 - slowAmount);

        yield return new WaitForSeconds(duration);

        // Phục hồi
        rb.velocity /= (1 - slowAmount);

        isSlowed = false;

        // Xóa component sau khi hết hiệu ứng
        Destroy(this);
    }
}
