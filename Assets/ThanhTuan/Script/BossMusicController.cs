using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class BossMusicController : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Âm thanh Boss")]
    public AudioClip bossBGM;

    [Header("Âm lượng tối đa (0.0 đến 1.0)")]
    [Range(0f, 1f)]
    public float masterVolume = 0.8f;

    [Header("Khoảng cách nghe tối đa")]
    [Tooltip("Nhạc sẽ nhỏ dần về 0 khi Player ở khoảng cách này.")]
    public float maxListenDistance = 30f;

    [Header("Thời gian nhạc nhỏ dần khi Boss Die")]
    public float fadeOutTime = 2f;

    private float startVolume;
    private bool isFadingOut = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("BossMusicController yêu cầu AudioSource component!");
            return;
        }

        audioSource.clip = bossBGM;
        audioSource.loop = true;
        audioSource.playOnAwake = false;

        audioSource.spatialBlend = 1f;

        audioSource.maxDistance = maxListenDistance;
        audioSource.minDistance = 5f;

        audioSource.volume = masterVolume;

        startVolume = masterVolume;
    }

    void Start()
    {
        audioSource.Play();
    }

    public void OnBossDefeated()
    {
        if (audioSource.isPlaying && !isFadingOut)
        {
            isFadingOut = true;
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;
        float currentVolume = audioSource.volume;

        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(currentVolume, 0f, timer / fadeOutTime);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
        isFadingOut = false;
    }
}