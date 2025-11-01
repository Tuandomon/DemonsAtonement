using UnityEngine;

public class ShowButtonWhenNear : MonoBehaviour
{
    public GameObject buttonUI;
    public Transform player;
    private Camera mainCam;
    private bool isPlayerNear = false;

    void Start()
    {
        mainCam = Camera.main;
        buttonUI.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear)
        {
            Vector3 screenPos = mainCam.WorldToScreenPoint(transform.position + new Vector3(0, 1.5f, 0));
            buttonUI.transform.position = screenPos;
        }
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
