using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;  // thêm n?u b?n dùng UI Slider

public class LightningEffect : MonoBehaviour
{
    [Header("Ánh sáng s?m sét")]
    public Light2D lightningLight;
    public float flashDuration = 0.1f;
    public float delayBetweenFlashes = 0.05f;

    [Header("Âm thanh s?m")]
    public AudioSource thunderSource;
    public AudioClip thunderClip;
    public float minThunderDelay = 0.3f;  // Ch?p tr??c s?m sau
    public float maxThunderDelay = 1.5f;
    [Range(0f, 1f)] public float minVolume = 0.6f;
    [Range(0f, 1f)] public float maxVolume = 1.0f;

    [Header("Âm thanh gió")]
    public AudioSource windSource;
    public AudioClip windClip;
    [Range(0f, 1f)] public float minWindVolume = 0.2f;
    [Range(0f, 1f)] public float maxWindVolume = 0.6f;
    public float windChangeSpeed = 0.5f;

    [Header("T?n su?t s?m sét (giây)")]
    public float minTime = 4f;
    public float maxTime = 10f;

    [Header("?i?u ch?nh âm l??ng chung")]
    [Range(0f, 1f)] public float masterVolume = 1f;   // volume t?ng
    public Slider volumeSlider; // kéo slider vào n?u có

    void Start()
    {
        // Gán slider n?u có
        if (volumeSlider != null)
        {
            float savedVol = PlayerPrefs.GetFloat("MasterVolume", 1f);
            masterVolume = savedVol;
            volumeSlider.value = savedVol;
            volumeSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        StartCoroutine(LightningRoutine());
        StartWind();
    }

    void SetMasterVolume(float value)
    {
        masterVolume = value;
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    void StartWind()
    {
        if (windSource != null && windClip != null)
        {
            windSource.clip = windClip;
            windSource.loop = true;
            windSource.volume = Random.Range(minWindVolume, maxWindVolume) * masterVolume;
            windSource.Play();
            StartCoroutine(ChangeWindVolume());
        }
    }

    IEnumerator ChangeWindVolume()
    {
        while (true)
        {
            float target = Random.Range(minWindVolume, maxWindVolume) * masterVolume;
            float start = windSource.volume;
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * windChangeSpeed;
                windSource.volume = Mathf.Lerp(start, target, t);
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
    }

    IEnumerator LightningRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minTime, maxTime);
            yield return new WaitForSeconds(waitTime);

            StartCoroutine(FlashLightning());

            // Ch?p xong ? tr? 0.3–1.5s r?i phát ti?ng s?m
            float thunderDelay = Random.Range(minThunderDelay, maxThunderDelay);
            yield return new WaitForSeconds(thunderDelay);

            thunderSource.pitch = Random.Range(0.9f, 1.1f);
            thunderSource.volume = Random.Range(minVolume, maxVolume) * masterVolume;
            thunderSource.PlayOneShot(thunderClip);
        }
    }

    IEnumerator FlashLightning()
    {
        int flashes = Random.Range(1, 4);
        for (int i = 0; i < flashes; i++)
        {
            lightningLight.intensity = 1.5f;
            yield return new WaitForSeconds(flashDuration);
            lightningLight.intensity = 0f;
            yield return new WaitForSeconds(delayBetweenFlashes);
        }
    }
}
