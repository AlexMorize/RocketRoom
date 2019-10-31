using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GestionnaireDeJeu : MonoBehaviour
{
    public int TempsDeJeu;

    public Texture2D StyleCurseur;
    public Joueur ModelJoueur;
    public Canvas canvas;
    public AfficheText AfficheurScore;
    public Image UiScore;
    public Text Horloge;
    public RawImage JoystickBackground, JoystickVirtuel;
    public GameObject GraphMaker;
    public AudioClip Bip1, Bip2;
    public Button btn_rejouer;
    public string SceneDeRetour;

    protected int NombreJoueur;
    protected float originalTime;
    protected int TotalTempsJeu;

    protected List<Text> LesAfficheursScore = new List<Text>();
    protected List<Spawner> LesSpawn = new List<Spawner>();
    protected List<int> NumToSpawn = new List<int>();
    protected Dictionary<Color, Dictionary<float, float>> lesGraphs = new Dictionary<Color, Dictionary<float, float>>();
    protected Camera CamJeu;
    protected AudioSource Bip;
    protected int[] score = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    public static bool UtiliséClavierSouris;
    protected static Color[] PlayColors = { Color.blue, Color.red, Color.green, Color.yellow, Color.cyan, Color.magenta, Color.white, Color.black };

    protected bool isEnd = false, stopEndAnimation = false;
    protected Vector3 lastKillPosition;
    protected float endTime;
    protected Vector3 OriginalCamPosition;
    protected GameObject LastPlayer;
    protected bool OnlyOnePlayer = false;
    protected int TimeBeforeStart = 3;

    void DecompteDebut()
    {
        AfficheText instance = Instantiate(AfficheurScore.gameObject, canvas.transform).GetComponent<AfficheText>();
        instance.AfficheScreenCenterDecay(TimeBeforeStart.ToString(), .5f, Color.white, 300, Vector2.zero);
        TimeBeforeStart--;
        if (TimeBeforeStart > 0) Invoke("DecompteDebut", .5f);

    }

    public virtual void GivePoint(int numPlayer, int nbPoints)
    {
        if (numPlayer < 1) return;
        else Debug.Log("NumPlayer: " + numPlayer);
        if (isEnd) nbPoints += 10;
        score[numPlayer - 1] += nbPoints;
        if (score[numPlayer - 1] < 0) score[numPlayer - 1] = 0;
        LesAfficheursScore[numPlayer - 1].text = score[numPlayer - 1].ToString();
        float CurrentTime;
        if (isEnd)
        {
            CurrentTime = TotalTempsJeu;
        }
        else
        {
            CurrentTime = Time.time - originalTime;
        }

        Color maCouleur = PlayColors[numPlayer - 1];
        if (lesGraphs.ContainsKey(maCouleur))
        {
            if (lesGraphs[maCouleur].ContainsKey(CurrentTime))
                lesGraphs[maCouleur][CurrentTime] += nbPoints;
            else
                lesGraphs[maCouleur].Add(CurrentTime, score[numPlayer - 1]);
        }
        else
        {
            lesGraphs.Add(maCouleur, new Dictionary<float, float>() { [0] = 0 });
            lesGraphs[maCouleur].Add(CurrentTime, score[numPlayer - 1]);
        }

    }

    public void AddSpawnPlayer(int NumPlayer)
    {
        if (NumPlayer < 0) return;
        if (isEnd) return;
        if (!NumToSpawn.Contains(NumPlayer))
        {
            Invoke("SpawnPlayer", 2 + NumToSpawn.Count * 0.3f);
            NumToSpawn.Add(NumPlayer);

        }

    }

    public virtual void Kill(Joueur Victime, int IdTueur)
    {
        if (isEnd)
            Invoke("VerifForPlayer", 0.01f);

        lastKillPosition = Victime.transform.position;
        if (true/*IdTueur > 0*/)
        {
            if (Victime.NumPlayer == IdTueur)
            {
                GivePoint(Victime.NumPlayer, -10);
                
                AfficheText instance = Instantiate(AfficheurScore.gameObject, canvas.transform).GetComponent<AfficheText>();
                if(IdTueur>0)
                    instance.AfficheAtPosition("-10", Victime.transform.position, 1, PlayColors[IdTueur - 1]);
                else
                    instance.AfficheAtPosition("-10", Victime.transform.position, 1, Color.black);
            }
            else
            {
                GivePoint(IdTueur, 10);
                AfficheText instance = Instantiate(AfficheurScore.gameObject, canvas.transform).GetComponent<AfficheText>();
                if (IdTueur > 0)
                    instance.AfficheAtPosition("10", Victime.transform.position, 1, PlayColors[IdTueur - 1]);
                else
                    instance.AfficheAtPosition("10", Victime.transform.position, 1, Color.black);
            }
        }
        if (!isEnd)
            AddSpawnPlayer(Victime.NumPlayer);

        Victime.Kill();
    }

    protected virtual void VerifForPlayer()
    {
        if (FindObjectsOfType<Joueur>().Length <= 1)
        {
            Time.timeScale = .25f;
            endTime = Time.time;
            LastPlayer = null;
            if (FindObjectOfType<Joueur>())
                LastPlayer = FindObjectOfType<Joueur>().gameObject;
            foreach (var player in FindObjectsOfType<Joueur>()) player.EndDestroy();
            CamJeu = FindObjectOfType<Camera>();
            OriginalCamPosition = CamJeu.transform.position;
            FindObjectOfType<Canvas>().enabled = false;

            MoveCamera();
            if (LastPlayer != null)
            {
                Invoke("MoveToKiller", .5f);
                Invoke("AfficheGraph", 3.5f);
            }
            else
            {
                Invoke("AfficheGraph", .5f);
            }

        }
    }


    protected virtual void MoveToKiller()
    {

        if (!stopEndAnimation)
        {

            Vector3 Direction = (LastPlayer.transform.position - Vector3.forward * 3) - (lastKillPosition - Vector3.forward * 5);
            CamJeu.transform.position = lastKillPosition - Vector3.forward * 5 + Direction * (Time.time - (endTime + .5f)) * 4;
            if ((Time.time - (endTime + .5f) < .25f))
            {
                Invoke("MoveToKiller", .001f);
                Time.timeScale = .3f;
            }
            else
            {
                Time.timeScale = 1;
                CamJeu.transform.position = LastPlayer.transform.position - Vector3.forward * 3;
                Invoke("MoveToKiller", .001f);
            }
        }

    }

    protected void AfficheGraph()
    {
        Cursor.visible = true;
        //CamJeu.transform.position = OriginalCamPosition;
        stopEndAnimation = true;
        GameObject gMaker = Instantiate(GraphMaker);
        GraphicMaker MyGraphic = gMaker.GetComponentInChildren<GraphicMaker>();
        MyGraphic.lesGraphs = lesGraphs;
        MyGraphic.MaxX = TotalTempsJeu;
        Time.timeScale = 1;
        btn_rejouer.gameObject.SetActive(true);
        btn_rejouer.transform.SetParent(MyGraphic.transform);
    }

    protected void MoveCamera()
    {
        if (!stopEndAnimation)
        {
            Vector3 Direction = (lastKillPosition - Vector3.forward * 5) - OriginalCamPosition;
            CamJeu.transform.position = OriginalCamPosition + Direction * (Time.time - endTime) * 2;
            if ((Time.time - endTime) < .5f)
                Invoke("MoveCamera", .001f);
        }

    }

    void Décompte()
    {
        Horloge.text = string.Format("{0}:{1:00}", TempsDeJeu / 60, TempsDeJeu % 60);
        if (TempsDeJeu == 0)
        {
            //Time.timeScale = 0;
            /*AfficheText instance = Instantiate(AfficheurScore.gameObject, canvas.transform).GetComponent<AfficheText>();
            instance.AfficheScreenCenter("Temps écoulé", 1, Color.white);
            instance = Instantiate(AfficheurScore.gameObject, canvas.transform).GetComponent<AfficheText>();
            instance.AfficheScreenCenterDecay("Press enter to restart", 1, Color.red, 30, new Vector2(0, -50));*/
            isEnd = true;
            CancelInvoke("SpawnPlayer");
            VerifForPlayer();
            AfficheText instance = Instantiate(AfficheurScore.gameObject, canvas.transform).GetComponent<AfficheText>();
            instance.AfficheScreenCenterDecay(TempsDeJeu.ToString(), 1, new Color(1, .5f, .5f), 300, Vector2.zero);
            Bip.clip = Bip2;
            Bip.Play();

        }
        else
        {
            if (TempsDeJeu <= 5)
            {
                Bip.Play();
                AfficheText instance = Instantiate(AfficheurScore.gameObject, canvas.transform).GetComponent<AfficheText>();
                instance.AfficheScreenCenterDecay(TempsDeJeu.ToString(), 1, Color.white, 300, Vector2.zero);
            }

            TempsDeJeu--;
            Invoke("Décompte", 1);
        }

    }

    void SpawnPlayer()
    {
        GameObject Instance = Instantiate(ModelJoueur.gameObject, LesSpawn[UnityEngine.Random.Range(0, LesSpawn.Count)].transform.position, Quaternion.Euler(0, 90, 0));
        Joueur ScriptInstance = Instance.GetComponent<Joueur>();
        ScriptInstance.NumPlayer = NumToSpawn[0];
        ScriptInstance.Couleur = PlayColors[NumToSpawn[0] - 1];
        ScriptInstance.Model3D.material.color = ScriptInstance.Couleur;
        Instance.GetComponentInChildren<MeshRenderer>().material.color = ScriptInstance.Couleur;
        NumToSpawn.RemoveAt(0);
    }

    void ReloadScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    protected void SpawnAllPlayers()
    {
        if (OnlyOnePlayer)
        {
            NumToSpawn.Add(1);
            lesGraphs.Add(PlayColors[0], new Dictionary<float, float>() { [0] = 0 });
            SpawnPlayer();
            return;
        }
        for (int i = 0; i < NombreJoueur; i++)
        {
            if (i >= Input.GetJoystickNames().Length || Input.GetJoystickNames()[i] != "")
            {
                NumToSpawn.Add(i + 1);
                lesGraphs.Add(PlayColors[i], new Dictionary<float, float>() { [0] = 0 });
                SpawnPlayer();
            }
        }
    }

    protected void Update()
    {
        if (TempsDeJeu == 0 && (Input.GetKey(KeyCode.Return) || Input.touchCount != 0)) ReloadScene();
        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene("MenuVersus");
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {

        Horloge.text = string.Format("{0}:{1:00}", TempsDeJeu / 60, TempsDeJeu % 60);
        DecompteDebut();
        if (SceneDeRetour == "")
            btn_rejouer.onClick.AddListener(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); });
        else
            btn_rejouer.onClick.AddListener(() => { SceneManager.LoadScene(SceneDeRetour); });

        btn_rejouer.gameObject.SetActive(false);
        Bip = gameObject.AddComponent<AudioSource>();
        Bip.clip = Bip1;
        TotalTempsJeu = TempsDeJeu;
        originalTime = Time.time +3;
        PlayerPrefs.SetString("Clavier/Souris", ";Horizontal;False;Vertical;False;Mouse X;False;Mouse Y;False;space;mouse 0;");
        DebugAndroid.Log(GetControllersResume());



        if (UtiliséClavierSouris)
        {
            NombreJoueur = 1 + Input.GetJoystickNames().Length;
            Cursor.SetCursor(StyleCurseur, new Vector2(StyleCurseur.width / 2, StyleCurseur.height / 2), CursorMode.ForceSoftware);
            if (NombreJoueur > 7) NombreJoueur = 7;
        }
        else
        {
            Cursor.visible = false;
            NombreJoueur = Input.GetJoystickNames().Length;
            if (NombreJoueur > 6) NombreJoueur = 6;
        }

        LesSpawn.AddRange(FindObjectsOfType<Spawner>());
        Invoke("SpawnAllPlayers", 1.5f);
        Invoke("Décompte", 1.5f);


        //SpawnAllPlayers();
        int décallage = 0;
        for (int i = 0; i < NombreJoueur; i++)
        {
            if (i >= Input.GetJoystickNames().Length || Input.GetJoystickNames()[i] != "")
            {
                Text instance = Instantiate(UiScore.gameObject, canvas.transform).GetComponentInChildren<Text>();
                instance.gameObject.transform.parent.position += Vector3.right * 110 * (i - décallage);
                instance.color = PlayColors[i];
                LesAfficheursScore.Add(instance);
            }
            else
            {
                décallage++;
                LesAfficheursScore.Add(null);
            }
        }
        UiScore.gameObject.SetActive(false);
    }


    string GetControllersResume()
    {
        string[] LesJoysticks = Input.GetJoystickNames();
        string result = LesJoysticks.Length + " controller(s) connected:" + System.Environment.NewLine;

        for (int i = 0; i < LesJoysticks.Length; i++)
        {
            result += "Joy_" + i + " : " + LesJoysticks[i] + System.Environment.NewLine;
        }
        return result;
    }
}
