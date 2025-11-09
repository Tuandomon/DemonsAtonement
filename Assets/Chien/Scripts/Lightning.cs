using UnityEngine;

public class Lightning : MonoBehaviour
{
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim != null)
        {
            // Bật trigger hoặc bool để animation chạy
            anim.SetTrigger("Strike");

            // Lấy thời lượng animation hiện tại và hủy object sau đó
            float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
            Destroy(gameObject, animLength);
        }
        else
        {
            // Nếu không có Animator thì tự hủy sau 1 giây
            Destroy(gameObject, 1f);
        }
    }
}
