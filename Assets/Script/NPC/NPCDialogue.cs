using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPCDialogue : MonoBehaviour
{
    [Header("UI lời thoại")]
    public GameObject dialoguePanel;   // Panel hiển thị hội thoại
    public Text dialogueText;          // Text hiển thị lời thoại
    public GameObject conButton;       // Nút "Tiếp"

    [Header("Âm thanh (tùy chọn)")]
    public AudioSource voiceSource;    // Nguồn phát âm thanh
    public AudioClip talkSound;        // Âm thanh mỗi khi xuất hiện 1 ký tự
    [Range(0f, 1f)] public float voiceVolume = 0.5f;

    [TextArea(2, 5)]
    public string[] dialogues;         // Danh sách câu thoại

    public float textSpeed = 0.05f;    // Tốc độ hiện chữ

    private bool hasTalked = false;    // Chỉ nói 1 lần
    private int index;                 // Câu hiện tại
    private Coroutine typingCoroutine; // Để dừng typing khi cần

    void Update()
    {
        // Hiện nút "Tiếp" khi câu hiện tại đã gõ xong
        if (dialoguePanel.activeInHierarchy && dialogues.Length > 0 && index < dialogues.Length)
        {
            if (dialogueText.text == dialogues[index])
            {
                conButton.SetActive(true);
            }
        }
    }

    public void NextLine()
    {
        conButton.SetActive(false);

        if (index < dialogues.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartTyping();
        }
        else
        {
            // Hết thoại → tắt panel & đánh dấu đã nói
            hasTalked = true;
            dialoguePanel.SetActive(false);
            dialogueText.text = "";
            index = 0;
        }
    }

    IEnumerator Typing()
    {
        // Gõ từng ký tự một
        foreach (char letter in dialogues[index].ToCharArray())
        {
            dialogueText.text += letter;

            // Phát âm thanh mỗi chữ
            if (talkSound != null && voiceSource != null)
            {
                voiceSource.pitch = Random.Range(0.95f, 1.05f); // cho tự nhiên
                voiceSource.PlayOneShot(talkSound, voiceVolume);
            }

            yield return new WaitForSeconds(textSpeed);
        }
    }

    private void StartTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(Typing());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Khi player chạm và NPC chưa nói lần nào
        if (collision.CompareTag("Player") && !hasTalked)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = "";
            index = 0;
            StartTyping();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasTalked)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            dialoguePanel.SetActive(false);
            dialogueText.text = "";
            index = 0;
        }
    }
}
