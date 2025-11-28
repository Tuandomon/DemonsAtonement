using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [Header("Thời gian tồn tại sau khi nổ (giây)")]
    public float lifeTime = 1f; // chỉnh theo độ dài animation/particle

    private bool triggered = false;

    void Start()
    {
        // Khi prefab được tạo ra, chỉ chạy một lần
        if (!triggered)
        {
            triggered = true;

            // Nếu có Animator thì phát animation nổ
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("explode");
                // cần tạo trigger "explode" trong Animator
            }

            // Nếu có ParticleSystem thì phát particle
            ParticleSystem ps = GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }

            // Hủy object sau lifeTime giây
            Destroy(gameObject, lifeTime);
        }
    }
}