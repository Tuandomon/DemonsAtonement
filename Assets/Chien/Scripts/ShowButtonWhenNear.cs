using UnityEngine;

public class ShowButtonWhenNear : MonoBehaviour
{
    public GameObject buttonUI; // gán Button_cua vào đây trong Inspector

    private void Start()
    {
        if (buttonUI != null)
            buttonUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // player phải có tag Player
        {
            if (buttonUI != null)
                buttonUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (buttonUI != null)
                buttonUI.SetActive(false);
        }
    }
}
