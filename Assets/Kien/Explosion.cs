using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float lifeTime = 0.6f;      // thời gian tồn tại
    public AudioClip explosionSound;   // âm thanh nổ
    public float volume = 1f;

    void Start()
    {
        // Phát âm thanh
        if (explosionSound != null)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, volume);

        // Tự hủy sau thời gian nổ
        Destroy(gameObject, lifeTime);
    }
}
