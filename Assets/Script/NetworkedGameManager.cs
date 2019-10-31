using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedGameManager : NetworkManager
{
    private LanMenu Menu;
    private List<short> PlayerIdList = new List<short>();
    private List<NetworkConnection> PlayerConnList = new List<NetworkConnection>();
    //private List<Text> LesAfficheursScore = new List<Text>();
    private List<Spawner> LesSpawn = new List<Spawner>();
    private List<int> NumToSpawn = new List<int>();
    int[] score = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    private static Color[] PlayColors = { Color.blue, Color.red, Color.green, Color.yellow, Color.cyan, Color.magenta, Color.white, Color.black };

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        MajPlayerList();
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        PlayerIdList.Add(playerControllerId);
        PlayerConnList.Add(conn);
        Invoke("MajPlayerList", .1f);

        /* GameObject player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
         //player.GetComponent<Player>().color = Color.red;
         NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);*/
    }


    void SpawnAllPlayers()
    {
        LesSpawn.Clear();
        LesSpawn.AddRange(FindObjectsOfType<Spawner>());
        for (int i = 0; i < PlayerConnList.Count; i++)
        {
            //if (i >= Input.GetJoystickNames().Length || Input.GetJoystickNames()[i] != "")
            //{
                NumToSpawn.Add(i);

                SpawnPlayer();
            //}
        }
    }

    void SpawnPlayer()
    {
        foreach(var player in FindObjectsOfType<NetwrokedPlayer>())
        {
            if (player.NumPlayer == NumToSpawn[0] + 1)
            {
                Debug.Log("Bug spawn esquive");
                return;

            }
        }

        GameObject Instance = Instantiate(playerPrefab, LesSpawn[UnityEngine.Random.Range(0, LesSpawn.Count)].transform.position, Quaternion.Euler(0, 90, 0));

        NetwrokedPlayer ScriptInstance = Instance.GetComponent<NetwrokedPlayer>();
        ScriptInstance.NumPlayer = NumToSpawn[0] + 1;
        ScriptInstance.Couleur = PlayColors[NumToSpawn[0]];
        Instance.GetComponentInChildren<MeshRenderer>().material.color = ScriptInstance.Couleur;

        
        NetworkServer.AddPlayerForConnection(PlayerConnList[NumToSpawn[0]], Instance, PlayerIdList[NumToSpawn[0]]);

        NumToSpawn.RemoveAt(0);

    }


    public void Kill(int NumVictime, int Tueur)
    {
        if (NumVictime == Tueur)
        {
            //GivePoint(Victime.NumPlayer, -10);
            //AfficheText instance = Instantiate(AfficheurScore.gameObject, canvas.transform).GetComponent<AfficheText>();
            //instance.AfficheAtPosition("-10", Victime.transform.position, 1, PlayColors[Tueur - 1]);
        }
        else
        {
            //GivePoint(Tueur, 10);
            //AfficheText instance = Instantiate(AfficheurScore.gameObject, canvas.transform).GetComponent<AfficheText>();
            //instance.AfficheAtPosition("10", Victime.transform.position, 1, PlayColors[Tueur - 1]);
        }
        if(!NumToSpawn.Contains(NumVictime))
            AddSpawnPlayer(NumVictime);
        foreach (var player in FindObjectsOfType<NetwrokedPlayer>())
        {
            if (player.NumPlayer == NumVictime) Destroy(player.gameObject);
        }
    }

    public void AddSpawnPlayer(int NumPlayer)
    {
        if (!NumToSpawn.Contains(NumPlayer))
        {
            Invoke("SpawnPlayer", 2 + NumToSpawn.Count * 0.3f);
            NumToSpawn.Add(NumPlayer-1);

        }

    }


    void MajPlayerList()
    {
        Menu.MajPlayerList(PlayerConnList.ToArray(), PlayerIdList.ToArray());

        string AffText = "";
        for (int i = 0; i < PlayerConnList.Count; i++)
        {
            AffText += "Joueur " + (i + 1) + " - " + "ID: " + PlayerIdList[i] + " - " + PlayerConnList[i].ToString() + '\n' + '\n';
            /*if (PlayerConn.Value.isReady) AffText += "Prêt" + '\n';
            else AffText += "En attente..." + '\n';*/
        }
        Menu.AffPlayerList.text = AffText;
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (Menu == null) Menu = FindObjectOfType<LanMenu>();
        Menu.Btn_Jouer.onClick.AddListener(() => {
            Menu.Rpc_StartGame();
            Invoke("SpawnAllPlayers", .1f);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}