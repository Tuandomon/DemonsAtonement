using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fall : MonoBehaviour
{
    float wait = 0.1f;
    public GameObject fallingObject;
    void Start()
    {
        InvokeRepeating("FallObject", wait, wait);
    }

    void FallObject()
    {
        Instantiate(fallingObject, new Vector3(Random.Range(-10, 10), 10, 0), Quaternion.identity);
    }
}
