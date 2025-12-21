using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LuuViTri1 : MonoBehaviour
{
    public Transform checkpoint; // v? trí mu?n quay v?

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // L?u scene hi?n t?i
            PlayerPrefs.SetString("Diem", SceneManager.GetActiveScene().name);

            // L?u v? trí checkpoint
            PlayerPrefs.SetFloat("CheckpointX", checkpoint.position.x);
            PlayerPrefs.SetFloat("CheckpointY", checkpoint.position.y);

            PlayerPrefs.Save();

        }
    }
}
