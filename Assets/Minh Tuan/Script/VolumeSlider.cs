using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider volumeSlider;
    public Button increaseButton;
    public Button decreaseButton;
    public float step = 0.1f;

    public Button soundOnButton;   // Nút hình bật âm thanh
    public Button soundOffButton;  // Nút hình tắt âm thanh

    private const string VolumeKey = "GameVolume";
    private float previousVolume = 1f; // Lưu âm lượng trước khi mute

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;

        UpdateSoundIcons(savedVolume);

        increaseButton.onClick.AddListener(IncreaseVolume);
        decreaseButton.onClick.AddListener(DecreaseVolume);
        volumeSlider.onValueChanged.AddListener(OnSliderChanged);

        soundOnButton.onClick.AddListener(MuteSound);
        soundOffButton.onClick.AddListener(UnmuteSound);
    }

    void IncreaseVolume()
    {
        volumeSlider.value = Mathf.Clamp(volumeSlider.value + step, 0f, 1f);
    }

    void DecreaseVolume()
    {
        volumeSlider.value = Mathf.Clamp(volumeSlider.value - step, 0f, 1f);
    }

    void OnSliderChanged(float value)
    {
        PlayerPrefs.SetFloat(VolumeKey, value);
        PlayerPrefs.Save();

        UpdateSoundIcons(value);
    }

    void MuteSound()
    {
        previousVolume = volumeSlider.value;
        PlayerPrefs.SetFloat("PreviousVolume", previousVolume); // lưu lại
        PlayerPrefs.SetFloat(VolumeKey, 0f);
        PlayerPrefs.Save();

        volumeSlider.value = 0f;
    }

    void UnmuteSound()
    {
        // Đọc lại âm lượng trước đó từ PlayerPrefs
        float restoredVolume = PlayerPrefs.GetFloat("PreviousVolume", 1f);
        volumeSlider.value = Mathf.Clamp(restoredVolume, 0.1f, 1f);
    }


    void UpdateSoundIcons(float volume)
    {
        bool isMuted = volume <= 0.01f;

        if (soundOnButton != null) soundOnButton.gameObject.SetActive(!isMuted);
        if (soundOffButton != null) soundOffButton.gameObject.SetActive(isMuted);
    }
}