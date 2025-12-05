using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BossMusicController : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Âm thanh Boss")]
    public AudioClip bossBGM;

    [Header("Khoảng cách nghe tối đa")]
    [Tooltip("Người chơi phải ở trong khoảng cách này mới nghe thấy nhạc. Đơn vị Unity.")]
    public float maxListenDistance = 30f;

    [Header("Thời gian nhạc nhỏ dần khi Boss Die")]
    public float fadeOutTime = 2f;

    private float startVolume;
    private bool isFadingOut = false;

    void Awake()
    {
        // 1. Chuẩn bị AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("BossMusicController yêu cầu AudioSource component!");
            return;
        }

        audioSource.clip = bossBGM;
        audioSource.loop = true; // Phát lặp lại liên tục
        audioSource.playOnAwake = false;

        // Quan trọng: Thiết lập 3D Sound để kiểm soát khoảng cách
        audioSource.spatialBlend = 1f; // Bắt buộc phải là 3D
        audioSource.maxDistance = maxListenDistance; // Khoảng cách nghe tối đa
        audioSource.minDistance = 5f; // Giả sử âm lượng tối đa trong vòng 5 đơn vị

        startVolume = audioSource.volume;
    }

    void Start()
    {
        // Bắt đầu phát ngay khi game object (Boss) được tạo
        // Nếu muốn bật/tắt theo sự kiện, bạn có thể gọi .Play() từ nơi khác
        audioSource.Play();
    }

    // Hàm gọi khi Boss chết
    public void OnBossDefeated()
    {
        if (audioSource.isPlaying && !isFadingOut)
        {
            isFadingOut = true;
            // Bắt đầu Coroutine để làm nhỏ dần âm lượng
            StartCoroutine(FadeOut());
        }
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float timer = 0f;

        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            // Tính toán âm lượng mới (giảm từ startVolume về 0)
            audioSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeOutTime);
            yield return null; // Chờ 1 frame
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // Đặt lại âm lượng nếu Boss này được tái sử dụng
        isFadingOut = false;

        // Tùy chọn: Xóa object Boss nếu đây là nơi xử lý cái chết cuối cùng
        // Destroy(gameObject); 
    }
}