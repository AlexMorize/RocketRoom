using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugAndroid : MonoBehaviour
{
    public RectTransform ContentViewport;
    public AfficheLogConsole LogSample;
    public ScrollRect ViewPort;
    public Button btn_affiche;

    Dictionary<string, AfficheLogConsole> specificLogc = new Dictionary<string, AfficheLogConsole>();
    
    private static DebugAndroid InstanceThis;
    private bool AfficherConsole;
    private float lastPosition = 0;

    public static void Log(string Text)
    {
        Debug.Log(Text);
        if (InstanceThis!=null)
        InstanceThis.AddLog(Text);
        
    }

    public static void LogSpecific(string Key, string TextValue)
    {
        //Debug.Log(Key + Environment.NewLine + TextValue);
        if (InstanceThis != null)
            InstanceThis.SetLogSpecific(Key, TextValue);
    }

    void SetLogSpecific(string Key, string TextValue)
    {
        if(specificLogc.ContainsKey(Key))
        {
            specificLogc[Key].SetLog(TextValue);
            specificLogc[Key].TitreLog.text += " Specific log \"" + Key + "\"";
            specificLogc[Key].TitreLog.color = Color.green;
        }else
        {
            AfficheLogConsole instance = Instantiate(LogSample.gameObject, ContentViewport).GetComponent<AfficheLogConsole>();
            instance.gameObject.SetActive(true);
            instance.SetLog(TextValue);

            instance.TitreLog.text += " Specific log \"" + Key + "\"";
            instance.TitreLog.color = Color.green;
            RectTransform InstanceTransform = instance.GetComponent<RectTransform>();
            InstanceTransform.anchoredPosition = new Vector2(10, -InstanceTransform.sizeDelta.y / 2 + lastPosition);
            ContentViewport.sizeDelta = new Vector2(ContentViewport.sizeDelta.x, -lastPosition + InstanceTransform.sizeDelta.y);
            lastPosition -= InstanceTransform.sizeDelta.y;
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            specificLogc.Add(Key, instance);
            
        }
    }

    void AddLog(string text)
    {
        AfficheLogConsole instance = Instantiate(LogSample.gameObject, ContentViewport).GetComponent<AfficheLogConsole>();
        instance.gameObject.SetActive(true);
        instance.SetLog(text);
        RectTransform InstanceTransform = instance.GetComponent<RectTransform>();
        InstanceTransform.anchoredPosition = new Vector2(10,-InstanceTransform.sizeDelta.y/2+lastPosition);
        ContentViewport.sizeDelta = new Vector2(ContentViewport.sizeDelta.x, -lastPosition + InstanceTransform.sizeDelta.y);
        lastPosition -= InstanceTransform.sizeDelta.y;
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());



    }

    void ToggleAfficheConsole()
    {
        AfficherConsole = !AfficherConsole;
        if (AfficherConsole)
        {
            ViewPort.verticalScrollbar.value = 0;
            btn_affiche.GetComponentInChildren<Text>().text = "Masquer la console";
            ViewPort.GetComponent<CanvasGroup>().alpha = 1;
            ViewPort.GetComponent<CanvasGroup>().interactable = true;
            ViewPort.GetComponent<CanvasGroup>().blocksRaycasts = true;

        }
        else
        {
            btn_affiche.GetComponentInChildren<Text>().text = "Afficher la console";
            ViewPort.GetComponent<CanvasGroup>().alpha = 0;
            ViewPort.GetComponent<CanvasGroup>().interactable = false;
            ViewPort.GetComponent<CanvasGroup>().blocksRaycasts = false;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        LogSample.gameObject.SetActive(false);
        AfficherConsole = true;
        ToggleAfficheConsole();
        btn_affiche.onClick.AddListener(ToggleAfficheConsole);
        InstanceThis = this;


    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
