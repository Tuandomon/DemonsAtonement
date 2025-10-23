using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;

    private AudioSource audioSource;
    private const string VolumeKey = "GameVolume";

    void Awake()
    {
        // Đảm bảo chỉ có 1 instance tồn tại (singleton pattern)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject); // Giữ nhạc khi chuyển scene

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;

        // Gán âm lượng từ PlayerPrefs
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        audioSource.volume = savedVolume;

        audioSource.Play();
    }

    void Update()
    {
        // Cập nhật âm lượng theo PlayerPrefs mỗi frame (hoặc có thể gọi từ sự kiện nếu muốn tối ưu hơn)
        audioSource.volume = PlayerPrefs.GetFloat(VolumeKey, 1f);
    }
}