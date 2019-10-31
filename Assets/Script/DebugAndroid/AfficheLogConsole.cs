using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AfficheLogConsole : MonoBehaviour
{
    public Text TitreLog, LogText;


    public void SetLog(string Text)
    {

        TitreLog.text = "Debug log : " + System.DateTime.Now.ToString("HH:mm:ss.ffff");
        LogText.text = Text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(LogText.rectTransform);
        GetComponent<RectTransform>().sizeDelta = LogText.rectTransform.sizeDelta + Vector2.up * 16;
        LogText.rectTransform.anchoredPosition = new Vector2(0, LogText.rectTransform.sizeDelta.y / 2);
        CancelInvoke("ReinitColor");
        Invoke("ReinitColor", 1);
        CancelInvoke("SetColorYellow");
        Invoke("SetColorYellow",.1f);

    }

    void SetColorYellow()
    {
        TitreLog.color = Color.yellow;
    }

    void ReinitColor()
    {
        if (TitreLog.color == Color.red) return;
            Color currentColor = TitreLog.color;

        TitreLog.color = new Color(currentColor.r+.01f, currentColor.g-.01f, currentColor.b - .01f);

        if (TitreLog.color != Color.red) Invoke("ReinitColor", .02f);
    }
}
