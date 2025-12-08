using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Text winerName;
    public Button retry;

    private void Awake()
    {
        retry.onClick.AddListener(OnClick);
    }

    public void SetName(string s)
    {
        winerName.text = s;

        if (s == "x")
        {
            StartCoroutine(GoToNextScene());
        }
    }

    IEnumerator GoToNextScene()
    {
        yield return new WaitForSeconds(3f);

        // L?u l?i tr?ng thái win ?? map chính bi?t r?t item
        PlayerPrefs.SetInt("DropReward1", 1);

        SceneManager.LoadScene("Map2");
    }

    public void OnClick()
    {
        SceneManager.LoadScene("Caro");
    }
}
