using UnityEngine;

public class SandwormTrap : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float activationDelay = 0.1f;
    [SerializeField] private AudioClip activationSound;
    [SerializeField] private float lifeTime = 1f; // Thời gian tối thiểu cho hoạt ảnh/tồn tại

    private Animator animator;
    private AudioSource audioSource;
    private bool hasTriggered = false;

    [HideInInspector] public GameObject triggerObject;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void ActivateTrap(GameObject player)
    {
        if (hasTriggered) return;

        hasTriggered = true;

        animator.SetTrigger("Activate");

        if (audioSource != null && activationSound != null)
        {
            audioSource.PlayOneShot(activationSound);
        }

        Invoke(nameof(DealDamage), activationDelay);

        // Hủy Prefab Sandworm (Cát Trái) sau thời gian lifeTime
        // và gọi hàm hiện lại Cát Phải ngay trước khi hủy
        Invoke(nameof(ReturnTriggerObject), lifeTime);
        Destroy(gameObject, lifeTime);
    }

    private void DealDamage()
    {
        // Gây sát thương một lần
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }

    private void ReturnTriggerObject()
    {
        // Hiện lại Cát Phải (vùng trigger) sau khi bẫy cát trái đã xong việc
        if (triggerObject != null)
        {
            triggerObject.GetComponent<SpriteRenderer>().enabled = true;
            triggerObject.GetComponent<Collider2D>().enabled = true;

            // Bây giờ Cát Phải đã hiện lại và đang trong thời gian Cooldown (5s)
        }
    }

    // (Tùy chọn: Sử dụng Animation Event để gọi ReturnTriggerObject() và DestroyTrap() thay cho Invoke)
    public void DestroyTrap()
    {
        Destroy(gameObject);
    }
}