using UnityEngine;

public class AudioDisablerZone : MonoBehaviour
{
    // Kéo thả GameObject chứa AudioSource cần tắt vào đây trong Inspector
    [Header("AudioSource Cần Vô Hiệu Hóa")]
    public AudioSource targetAudioSource;

    // Biến để lưu trạng thái ban đầu của âm thanh (có đang phát không)
    private bool wasPlayingBeforeEnter = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra Player và đảm bảo có AudioSource mục tiêu
        if (collision.CompareTag("Player") && targetAudioSource != null)
        {
            // 1. Lưu trạng thái: Kiểm tra xem âm thanh có đang phát hay không
            wasPlayingBeforeEnter = targetAudioSource.isPlaying;

            // 2. Vô hiệu hóa tạm thời: Dừng phát âm thanh
            if (wasPlayingBeforeEnter)
            {
                targetAudioSource.Pause(); // Dùng Pause() sẽ tốt hơn Stop() nếu bạn muốn tiếp tục từ vị trí đã dừng. Hoặc dùng Stop() nếu bạn muốn nó phát lại từ đầu.
                // targetAudioSource.Stop(); // Nếu muốn phát lại từ đầu khi thoát
                Debug.Log($"Âm thanh {targetAudioSource.gameObject.name} đã bị vô hiệu hóa tạm thời.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Kiểm tra Player và đảm bảo có AudioSource mục tiêu
        if (collision.CompareTag("Player") && targetAudioSource != null)
        {
            // 3. Kích hoạt lại: Nếu âm thanh ban đầu đang phát, hãy phát lại
            if (wasPlayingBeforeEnter)
            {
                targetAudioSource.UnPause(); // Nếu dùng Pause() ở trên
                // targetAudioSource.Play(); // Nếu dùng Stop() ở trên
                Debug.Log($"Âm thanh {targetAudioSource.gameObject.name} đã được kích hoạt lại.");
            }
        }
    }
}