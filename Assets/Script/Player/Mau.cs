using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mau : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Slider healthBar; // G?n n?u có thanh máu

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.maxValue = maxHealth;
    }

    public void AddHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        if (healthBar != null)
            healthBar.value = currentHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player ch?t r?i!");
        }

        if (healthBar != null)
            healthBar.value = currentHealth;
    }
}
