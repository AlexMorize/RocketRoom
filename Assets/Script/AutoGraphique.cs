using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoGraphique : MonoBehaviour
{
    public GameObject MenuEndOfTest;

    int index = 0;
    int nbFrame = 0;

    bool countFrame = false;
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.SetQualityLevel(0);
        Invoke("SetGraphique",1.5f);
        Invoke("StartCount", 1);
        
        
        
    }

    void StartCount()
    {
        countFrame = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(countFrame)
            nbFrame++;
    }

    void SetGraphique()
    {
        if(index>4)
        {
            Destroy(gameObject);
            return;
        }

        if (nbFrame > 15)
        {
            Debug.Log("Quality_" + index + " : " + (nbFrame * 2) + " Fps");
            QualitySettings.SetQualityLevel(index);
            Invoke("SetGraphique", 0.5f);
            index++;
            GetComponentInChildren<Text>().text = "Configuration de vos paramètres graphique en cours..." + '\n' + "Quality_" + index + " : " + (nbFrame * 2) + " Fps";
            nbFrame = 0;
        }
        else
        {
            MenuEndOfTest.SetActive(true);
            Destroy(gameObject);
        }
        

        
        
    }
}
