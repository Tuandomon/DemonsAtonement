using UnityEngine;
using System.Collections;

public class PlayerAttackNormal : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;

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

        // Tắt hitbox ban đầu
        if (hitbox1 != null) hitbox1.SetActive(false);
        if (hitbox2 != null) hitbox2.SetActive(false);
    }

    void Update()
    {
        float currentTime = Time.time;

        if (waitingForSecondAttack && currentTime - lastAttackTime > comboWindow)
        {
            waitingForSecondAttack = false;
        }

        if (Input.GetKeyDown(KeyCode.J) && !Input.GetKey(KeyCode.S))
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName("Idle"))
            {
                animator.SetTrigger("AttackNormalTrigger");
                waitingForSecondAttack = true;
                lastAttackTime = currentTime;

                PlaySound(attackSound1);
            }
            else if (state.IsName("WaitForInput") && waitingForSecondAttack)
            {
                animator.SetTrigger("AttackNormalTrigger");
                waitingForSecondAttack = false;

                PlaySound(attackSound2);
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
}