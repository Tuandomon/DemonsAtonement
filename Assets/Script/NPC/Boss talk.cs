using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bosstalk : MonoBehaviour
{
    [Header("UI lời thoại")]
    public GameObject dialoguePanel;   // Panel hiển thị hội thoại
    public Text dialogueText;          // Text hiển thị lời thoại
    public GameObject conButton;       // Nút "Tiếp"

    [Header("Âm thanh (tuỳ chọn)")]
    public AudioSource voiceSource;

    [TextArea(2, 5)]
    public string[] dialogues;         // Danh sách câu thoại

    public float textSpeed = 0.05f;

    private bool playerIsClose;        // Người chơi có đang ở gần không
    private bool hasTalked = false;    // Đã nói xong chưa
    private int index;                 // Câu thoại hiện tại

    void Update()
    {
        // Hiện nút "Tiếp" khi gõ xong 1 câu
        if (dialoguePanel.activeInHierarchy && dialogueText.text == dialogues[index])
        {
            conButton.SetActive(true);
        }
    }

    public void NextLine()
    {
        conButton.SetActive(false);

        if (index < dialogues.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            // Hết thoại
            hasTalked = true;
            zeroText();
        }
    }

    IEnumerator Typing()
    {
        foreach (char letter in dialogues[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTalked)
        {
            playerIsClose = true;
            dialoguePanel.SetActive(true);
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
            zeroText();
        }
    }

    private void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
        conButton.SetActive(false);
    }
}
