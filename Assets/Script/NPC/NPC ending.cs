using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCending : MonoBehaviour
{
    [Header("UI lời thoại")]
    public GameObject dialoguePanel;
    public Text dialogueText;
    public GameObject conButtom;
    [Header("Âm thanh (tuỳ chọn)")]
    public AudioSource voiceSource;

    [TextArea(2, 5)]
    public string[] dialogues;

    public float textSpeed = 0.05f;

    private bool playerIsClose;
    private int index;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && playerIsClose)
        {
            if (dialoguePanel.activeInHierarchy)
            {
                zeroText();
            }
            else
            {
                dialoguePanel.SetActive(true);
                StartCoroutine(Typing());
            }
        }
        if(dialogueText.text == dialogues[index])
        {
            conButtom.SetActive(true);
        }
    }

    public void NextLine()
    {
        conButtom.SetActive(false);
        if (index < dialogues.Length - 1)
        {
            index++;
            dialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            zeroText();
        }
    }

    IEnumerator Typing()
    {
        foreach(char letter in dialogues[index].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(textSpeed);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = true;
            Debug.Log("Player đã chạm NPC!"); // kiểm tra trong Console
            dialoguePanel.SetActive(true);
            StartCoroutine(Typing());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
            Debug.Log("Player rời khỏi NPC!");
            zeroText();
        }
    }
    private void zeroText()
    {
        dialogueText.text = "";
        index = 0;
        dialoguePanel.SetActive(false);
    }

}
