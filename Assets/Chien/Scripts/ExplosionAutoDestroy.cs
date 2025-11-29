using UnityEngine;

public class ExplosionAutoDestroy : MonoBehaviour
{
    [Header("Sound")]
    public AudioClip explosionSound;     // 👈 Gán âm thanh vào đây
    public float volume = 1f;

    private void Start()
    {
        // 🔊 Phát âm thanh Explosion
        PlaySound();

        // Lấy thời gian dài nhất trong tất cả animation clip của Animator
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            float time = anim.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, time);
        }
        else
        {
            Destroy(gameObject, 1f); // fallback
        }
    }

    private void PlaySound()
    {
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, volume);
        }
    }
}
