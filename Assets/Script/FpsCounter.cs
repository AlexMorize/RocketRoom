using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("ShowCount", 0, .5f);
    }

    // Update is called once per frame
    void ShowCount()
    {
        Debug.Log("FPS: " + Mathf.Round( 1 / Time.deltaTime));
        
    }
}
