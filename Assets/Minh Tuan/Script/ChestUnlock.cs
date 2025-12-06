using UnityEngine;

public class ChestUnlock : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("ChestOpen"); // tên clip mở
        }
    }

    void Update()
    {
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("ChestOpen") && stateInfo.normalizedTime >= 1f)
            {
                // animation đã chạy hết → dừng lại
                animator.enabled = false;
            }
        }
    }
}