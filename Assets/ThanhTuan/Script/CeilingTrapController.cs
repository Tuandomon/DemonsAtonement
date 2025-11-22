using UnityEngine;
using System.Collections;

public class CeilingTrapController : MonoBehaviour
{
    [Header("Components")]
    public SpriteRenderer staticTrapSprite;
    public Collider2D trapTriggerCollider;

    [Header("Animated Trap Settings")]
    public GameObject animatedTrapObject;
    public float animationDuration = 1.0f;

    [Header("Damage Settings")]
    public int damageAmount = 1;

    [Header("Audio Settings")]
    public AudioSource trapAudioSource;
    public AudioClip activationSound;

    private bool isActivated = false;

    void Start()
    {
        if (staticTrapSprite == null) staticTrapSprite = GetComponent<SpriteRenderer>();
        if (trapTriggerCollider == null) trapTriggerCollider = GetComponent<Collider2D>();

        if (trapAudioSource == null)
        {
            trapAudioSource = GetComponent<AudioSource>();
        }

        if (animatedTrapObject != null)
        {
            animatedTrapObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            ActivateTrap(other.gameObject);
        }
    }

    void ActivateTrap(GameObject player)
    {
        isActivated = true;

        if (trapAudioSource != null && activationSound != null)
        {
            trapAudioSource.PlayOneShot(activationSound);
        }

        staticTrapSprite.enabled = false;
        trapTriggerCollider.enabled = false;

        animatedTrapObject.SetActive(true);
        Animator anim = animatedTrapObject.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Activate");
        }

        CauseDamage(player);

        StartCoroutine(DeactivateAndResetTrap(animationDuration));
    }

    void CauseDamage(GameObject player)
    {
        var healthScript = player.GetComponent<PlayerHealth>();

        if (healthScript != null)
        {
            healthScript.TakeDamage(damageAmount);
        }
    }

    IEnumerator DeactivateAndResetTrap(float delay)
    {
        yield return new WaitForSeconds(delay);

        animatedTrapObject.SetActive(false);

        staticTrapSprite.enabled = true;
        trapTriggerCollider.enabled = true;

        isActivated = false;
    }
}