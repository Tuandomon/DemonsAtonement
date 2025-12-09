using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuuViTri : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.HasKey("ReturnPointName"))
        {
            string pointName = PlayerPrefs.GetString("ReturnPointName");

            Transform returnPoint = GameObject.Find(pointName)?.transform;

            if (returnPoint != null)
            {
                transform.position = returnPoint.position;
            }
        }
    }
}
