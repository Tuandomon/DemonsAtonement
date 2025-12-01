using System.Collections;
using System.Collections.Generic;
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
    }

    // Update is called once per frame
    public void OnClick()
    {
        SceneManager.LoadScene("Caro");
    }

}
