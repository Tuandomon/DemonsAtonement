using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public Text winer;

    public void SetName(string s)
    {
        winer.text = s;
    }

}
