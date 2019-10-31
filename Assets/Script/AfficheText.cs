using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AfficheText : MonoBehaviour
{

    private float TempsDeVie;
    private float resteVie;

    public void AfficheAtPosition(string Text, Vector3 position, float tempsDeVie, Color color)
    {
        Text TextRend = GetComponent<Text>();
        TextRend.text = Text;
        TextRend.color = color;
        transform.position = FindObjectOfType<Camera>().WorldToScreenPoint(position);
        Destroy(gameObject, tempsDeVie);
        TempsDeVie = resteVie = tempsDeVie;
    }

    public void AfficheScreenCenter(string Text, float tempsDeVie, Color color)
    {
        Text TextRend = GetComponent<Text>();
        TextRend.text = Text;
        TextRend.color = color;
        transform.position = new Vector2(Screen.width / 2, Screen.height / 2);
        Destroy(gameObject, tempsDeVie);
        TempsDeVie = resteVie = tempsDeVie;
    }

    public void AfficheScreenCenterDecay(string Text, float tempsDeVie, Color color, int TextSize, Vector2 decay)
    {
        Text TextRend = GetComponent<Text>();
        TextRend.text = Text;
        TextRend.color = color;
        TextRend.fontSize = TextSize;
        transform.position = new Vector2(Screen.width / 2 + decay.x, Screen.height / 2 + decay.y);
        Destroy(gameObject, tempsDeVie);
        TempsDeVie = resteVie = tempsDeVie;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        resteVie -= Time.deltaTime;
        Text textRend = GetComponent<Text>();
        textRend.color = new Color(textRend.color.r, textRend.color.g, textRend.color.b, resteVie / TempsDeVie);
    }
}
