using UnityEngine;

public class LetterDisappear : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);   // xoá chữ
        }
    }
}
