using UnityEngine;

public class ShowButtonWhenNear : MonoBehaviour
{
    public GameObject buttonUI;
    public Transform player;
    private bool isPlayerNear = false;

    void Start()
    {
        buttonUI.SetActive(false);
    }

    void Update()
    {
        // Không cập nhật vị trí nữa, giữ nguyên chỗ đặt sẵn trong Canvas
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
            buttonUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            buttonUI.SetActive(false);
        }
    }
}
