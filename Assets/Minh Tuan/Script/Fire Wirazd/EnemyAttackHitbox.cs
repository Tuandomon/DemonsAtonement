using UnityEngine;
using System.Collections;

public class EnemyAttackHitbox : MonoBehaviour
{
    public int damage = 10;
    public float activeTime = 0.2f;
    private Animator enemyAnimator;


    void Start()
    {
        enemyAnimator = GetComponentInParent<Animator>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && enemyAnimator != null)
        {
            AnimatorStateInfo state = enemyAnimator.GetCurrentAnimatorStateInfo(0);

            // Kiểm tra nếu đang ở trạng thái Attack
            if (state.IsName("Attack"))
            {
                PlayerHealth player = other.GetComponent<PlayerHealth>();
                if (player != null)
                    player.TakeDamage(damage);
            }
        }
    }

    public void ActivateHitbox()
    {
        StartCoroutine(EnableTemporarily());
    }

    IEnumerator EnableTemporarily()
    {
        GetComponent<Collider2D>().enabled = true;
        yield return new WaitForSeconds(activeTime);
        GetComponent<Collider2D>().enabled = false;
    }
}