using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bosstalk : MonoBehaviour
{
    [Header("UI lời thoại")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public GameObject conButton;

    [Header("Âm thanh khi panel bật")]
    public AudioSource voiceSource;   // Phát 1 lần khi panel mở

    [TextArea(2, 5)]
    public string[] dialogues;

    public float textSpeed = 0.05f;

    private bool playerIsClose;
    private bool hasTalked = false;
    private int index;

    void Update()
    {
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

            // 🔥 Phát âm thanh 1 lần khi mở panel
            if (voiceSource != null)
            {
                voiceSource.loop = false;
                voiceSource.Play();   // Chỉ play 1 lần
            }

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

        // 🔥 Nếu âm thanh đang chạy mà thoát panel → dừng
        if (voiceSource != null)
        {
            voiceSource.Stop();
        }
    }
}
