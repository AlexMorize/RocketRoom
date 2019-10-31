using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LanMenu : NetworkBehaviour
{
    
    public Text AffPlayerList;
    public Button Btn_Jouer;
    public GameObject Map;
    public Camera CamMenu;

    [SyncVar]
    string TextAAfficher;

    [ClientRpc]
    public void Rpc_StartGame()
    {
        Map.SetActive(true);
        GetComponent<Canvas>().enabled = false;
        CamMenu.gameObject.SetActive(false);

    }

    [Command]
    void Cmd_StartGame()
    {
        Rpc_StartGame();
    }

    private void Start()
    {
        Btn_Jouer.onClick.AddListener(Cmd_StartGame);
    }

    public void MajPlayerList(NetworkConnection[] PlayerConnList,   short[] PlayerIdList)
    {

        string AffText = "";
        for (int i = 0; i < PlayerConnList.Length; i++)
        {
            AffText += "Joueur " + (i + 1) + " - " + "ID: " + PlayerIdList[i] + " - " + PlayerConnList[i].ToString() + '\n' + '\n';
            /*if (PlayerConn.Value.isReady) AffText += "Prêt" + '\n';
            else AffText += "En attente..." + '\n';*/
        }
        AffPlayerList.text = TextAAfficher = AffText;
        Rpc_MajPlayerList();
    }

    [ClientRpc]
    void Rpc_MajPlayerList()
    {
        AffPlayerList.text = TextAAfficher;
    }
}
