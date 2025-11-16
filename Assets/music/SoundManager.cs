using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    public AudioSource audioSource;
    public AudioClip clickSound;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void PlayClick()
    {
        audioSource.PlayOneShot(clickSound);
    }
}
