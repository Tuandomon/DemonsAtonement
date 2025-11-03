using UnityEngine;

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

    public class PlayerAttackHitbox : MonoBehaviour
    {
        public GameObject hitbox1;
        public GameObject hitbox2;

        public void EnableHitbox1() => hitbox1.SetActive(true);
        public void DisableHitbox1() => hitbox1.SetActive(false);
        public void EnableHitbox2() => hitbox2.SetActive(true);
        public void DisableHitbox2() => hitbox2.SetActive(false);
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
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

                // Phát âm thanh đòn 1 với âm lượng SFXVolume
                if (attackSound1 != null)
                {
                    audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
                    audioSource.PlayOneShot(attackSound1);
                }
            }
            else if (state.IsName("WaitForInput") && waitingForSecondAttack)
            {
                animator.SetTrigger("AttackNormalTrigger");
                waitingForSecondAttack = false;

                // Phát âm thanh đòn 2 với âm lượng SFXVolume
                if (attackSound2 != null)
                {
                    audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
                    audioSource.PlayOneShot(attackSound2);
                }
            }
        }
    }
}