using UnityEngine;

public class CaveSoundDisabler : MonoBehaviour
{
    [Header("GameObject cần vô hiệu hóa tạm thời")]
    // Kéo GameObject chứa Audio Source tiếng mưa/gió vào đây
    public GameObject targetSoundObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là Player không
        if (other.CompareTag("Player"))
        {
            if (targetSoundObject != null)
            {
                // Tạm thời vô hiệu hóa toàn bộ GameObject khi Player đi vào
                targetSoundObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (targetSoundObject != null)
            {
                // Kích hoạt lại GameObject khi Player đi ra
                targetSoundObject.SetActive(true);
            }
        }
    }
}