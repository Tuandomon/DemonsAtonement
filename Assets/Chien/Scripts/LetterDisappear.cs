using UnityEngine;

public class LetterDisappear : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);   // xoá chữ
        }
    }
}
