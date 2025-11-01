using UnityEngine;

public class PlayerAttackNormal : MonoBehaviour
{
    private Animator animator;
    private float lastAttackTime = 0f;
    private bool waitingForSecondAttack = false;
    public float comboWindow = 0.5f;
    public class PlayerAttackHitbox : MonoBehaviour
    {
        public GameObject hitbox1;
        public GameObject hitbox2;

        public void EnableHitbox1()
        {
            hitbox1.SetActive(true);
        }

        public void DisableHitbox1()
        {
            hitbox1.SetActive(false);
        }

        public void EnableHitbox2()
        {
            hitbox2.SetActive(true);
        }

        public void DisableHitbox2()
        {
            hitbox2.SetActive(false);
        }
    }

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float currentTime = Time.time;

        // Nếu đang chờ combo nhưng hết thời gian → reset
        if (waitingForSecondAttack && currentTime - lastAttackTime > comboWindow)
        {
            waitingForSecondAttack = false;
        }

        if (Input.GetKeyDown(KeyCode.J) && !Input.GetKey(KeyCode.S))
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsName("Idle"))
            {
                // Đánh đòn 1
                animator.SetTrigger("AttackNormalTrigger");
                waitingForSecondAttack = true;
                lastAttackTime = currentTime;
            }
            else if (state.IsName("WaitForInput") && waitingForSecondAttack)
            {
                // Đánh đòn 2
                animator.SetTrigger("AttackNormalTrigger");
                waitingForSecondAttack = false;
            }
        }
    }
}