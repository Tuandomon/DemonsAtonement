using UnityEngine;
using System.Collections;

public class PlayerAttackNormal : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private PlayerController playerController; // Tham chiếu tới PlayerController

    private float lastAttackTime = 0f;
    private bool waitingForSecondAttack = false;
    public float comboWindow = 0.5f;

    [Header("Attack Sounds")]
    public AudioClip attackSound1;
    public AudioClip attackSound2;

    [Header("Hitbox")]
    public GameObject hitbox1;
    public GameObject hitbox2;

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerController = GetComponent<PlayerController>();

        // Tắt hitbox ban đầu
        if (hitbox1 != null) hitbox1.SetActive(false);
        if (hitbox2 != null) hitbox2.SetActive(false);
    }

    void Update()
    {
        float currentTime = Time.time;

        // Reset combo nếu quá thời gian
        if (waitingForSecondAttack && currentTime - lastAttackTime > comboWindow)
        {
            waitingForSecondAttack = false;
        }

        // Nhấn J để tấn công
        if (Input.GetKeyDown(KeyCode.J) && !Input.GetKey(KeyCode.S))
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            // Đòn đánh 1
            if (state.IsName("Idle"))
            {
                animator.SetTrigger("AttackNormalTrigger");
                waitingForSecondAttack = true;
                lastAttackTime = currentTime;

                PlaySound(attackSound1);
                LockMovement();
            }
            // Đòn đánh 2
            else if (state.IsName("WaitForInput") && waitingForSecondAttack)
            {
                animator.SetTrigger("AttackNormalTrigger");
                waitingForSecondAttack = false;

                PlaySound(attackSound2);
                LockMovement();
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            audioSource.PlayOneShot(clip);
        }
    }

    // 🥊 Bật/tắt hitbox cho đòn 1
    public void EnableHitbox1() => hitbox1?.SetActive(true);
    public void DisableHitbox1() => hitbox1?.SetActive(false);

    // 🥊 Bật/tắt hitbox cho đòn 2
    public void EnableHitbox2() => hitbox2?.SetActive(true);
    public void DisableHitbox2() => hitbox2?.SetActive(false);

    // 🚫 Khóa di chuyển
    public void LockMovement()
    {
        if (playerController != null)
            playerController.enabled = false;
    }

    // ✅ Mở lại di chuyển (gọi ở cuối animation bằng Animation Event)
    public void UnlockMovement()
    {
        if (playerController != null)
            playerController.enabled = true;
    }
}