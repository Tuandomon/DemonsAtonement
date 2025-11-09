using UnityEngine;

public class Lightning : MonoBehaviour
{
    public int damage = 30;                 // Sát thương của Lightning
    public AudioClip strikeSound;           // Clip âm thanh chém
    private Animator anim;
    private AudioSource audioSource;

    void Start()
    {
        anim = GetComponent<Animator>();

        // Thêm AudioSource nếu chưa có
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (strikeSound != null)
        {
            audioSource.clip = strikeSound;
            audioSource.Play();
        }

        if (anim != null)
        {
            anim.SetTrigger("Strike");
            float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, animLength);
        }
        else
        {
            Destroy(gameObject, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
