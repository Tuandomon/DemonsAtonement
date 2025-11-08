using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class NextScence : MonoBehaviour
{
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void RePlay()
    {
        string name = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Man_1");
        Time.timeScale = 1;
    }
    public void RePlay1()
    {
        string name = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Man_1");
      
    }
}
