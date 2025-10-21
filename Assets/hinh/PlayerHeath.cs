using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeath : MonoBehaviour
{
    public int health = 100;

    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log("Máu còn lại: " + health);
        if (health <= 0)
        {
            Debug.Log("Người chơi đã chết!");
        }
    }
}
