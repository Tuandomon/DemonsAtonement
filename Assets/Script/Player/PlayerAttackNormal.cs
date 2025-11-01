using UnityEngine;

public class PlayerAttackNormal : MonoBehaviour
{
    private Animator animator;
    private int attackStep = 1;
    private float lastAttackTime = 0f;
    public float comboWindow = 0.5f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float currentTime = Time.time;

        if (currentTime - lastAttackTime > comboWindow)
        {
            attackStep = 1;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.SetInteger("AttackNormalStep", attackStep);
            animator.SetTrigger("AttackNormalTrigger");

            lastAttackTime = currentTime;
            attackStep++;
            if (attackStep > 2) attackStep = 1;
        }
    }
}