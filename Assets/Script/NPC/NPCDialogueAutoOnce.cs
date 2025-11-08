using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogueAutoOnce : MonoBehaviour
{
    [Header("UI l?i tho?i")]
    public GameObject dialoguePanel;   // Panel hi?n th? h?i tho?i
    public Text dialogueText;          // Text hi?n th? l?i
    public GameObject conButton;       // Nút "Ti?p"

    [Header("Âm thanh (tu? ch?n)")]
    public AudioSource voiceSource;

    [TextArea(2, 5)]
    public string[] dialogues;         // Danh sách câu tho?i

    public float textSpeed = 0.05f;    // T?c ?? hi?n ch?

    private bool playerIsClose;
    private int index = 0;
    private bool hasTalked = false;    // ? NPC ch? nói 1 l?n
    private bool isTyping = false;

    void Update()
    {
        // Khi ?ang hi?n th? panel và ?ã gõ xong 1 câu ? b?t nút ti?p
        if (dialoguePanel.activeSelf && dialogueText.text == dialogues[index] && !isTyping)
        {
            conButton.SetActive(true);
        }
        else
        {
            conButton.SetActive(false);
        }
    }

    public void NextLine()
    {
        if (isTyping) return;

        conButton.SetActive(false);

        if (index < dialogues.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator Typing()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in dialogues[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTalked)
        {
            playerIsClose = true;
            dialoguePanel.SetActive(true);
            index = 0;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialogueText.text = "";
        dialoguePanel.SetActive(false);
        index = 0;
        hasTalked = true; // ? Sau khi nói xong thì không nói l?i n?a
    }
}
