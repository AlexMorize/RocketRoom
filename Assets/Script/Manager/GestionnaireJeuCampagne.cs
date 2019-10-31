using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GestionnaireJeuCampagne : GestionnaireDeJeu
{
    public GameObject Bot;
    public CilbeEntrainement CiblePrefab;
    public bool ClearAllToFinish = true;

    [Range(0,4)]
    public int NbBot = 1;

    int ScoreBot = 0;

    public override void GivePoint(int numPlayer, int nbPoints)
    {
        if (numPlayer < 0)
        {
            ScoreBot += nbPoints;
        }
        Debug.Log("Score bot : " + ScoreBot);

        base.GivePoint(numPlayer, nbPoints);
        
    }

    protected List<Vector3> SpawnDeCible = new List<Vector3>();
   
    // Start is called before the first frame update
    protected override void Start()
    {
        UtiliséClavierSouris = Input.GetJoystickNames().Length<1;
        OnlyOnePlayer = true;
        base.Start();
        CamJeu = FindObjectOfType<Camera>();
        lastKillPosition = CamJeu.transform.position + Vector3.forward * 5;
        OriginalCamPosition = CamJeu.transform.position;

        for(int i=0;i<NbBot;i++)
            Invoke("SpawnBot", 2+i);



    }

    protected override void MoveToKiller()
    {
        if (!stopEndAnimation)
        {

            Vector3 Direction = (LastPlayer.transform.position - Vector3.forward * 3) - (OriginalCamPosition);
            CamJeu.transform.position = OriginalCamPosition + Direction * (Time.time - (endTime)) * 4;
            if ((Time.time - endTime < .25f))
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

    public override void Kill(Joueur Victime, int IdTueur)
    {
        base.Kill(Victime, IdTueur);
        if (!isEnd && Victime.NumPlayer == -1) Invoke("SpawnBot",2);
    }

    void SpawnBot()
    {
        if (FindObjectsOfType<Bot>().Length >= NbBot) return;
        if (isEnd) return;
        Instantiate(Bot, LesSpawn[Random.Range(0, LesSpawn.Count)].transform.position, Quaternion.Euler(0,90,0));
    }

    public void SpawnCible()
    {

        Invoke("RespawnCible", .5f);
    }

    void RespawnCible()
    {
        if (isEnd)
        {
            VerifForPlayer();
            return;
        }
        Vector3 positionCible;
        if(SpawnDeCible.Count<4)
        {
            positionCible = LesSpawn[Random.Range(0, LesSpawn.Count)].transform.position;
        }else
        {
            positionCible = SpawnDeCible[Random.Range(0,2)];
            SpawnDeCible.Remove(positionCible);
        }

        Joueur player = FindObjectOfType<Joueur>();
        if (player) SpawnDeCible.Add(player.transform.position);
        Instantiate(CiblePrefab, positionCible, new Quaternion());
        
    }

    protected override void VerifForPlayer()
    {
        CancelInvoke("RespawnCible");
        if (ClearAllToFinish && FindObjectsOfType<Joueur>().Length>0)
        {

            if (FindObjectsOfType<CilbeEntrainement>().Length > 0) return;
        }

        if (FindObjectsOfType<Joueur>().Length <= 1)
        {
    
            endTime = Time.time;
            LastPlayer = null;
            if (FindObjectOfType<Joueur>())
                LastPlayer = FindObjectOfType<Joueur>().gameObject;
            foreach (var player in FindObjectsOfType<Joueur>()) player.EndDestroy();
            CamJeu = FindObjectOfType<Camera>();
            OriginalCamPosition = CamJeu.transform.position;
            FindObjectOfType<Canvas>().enabled = false;

            if (LastPlayer != null)
            {
                MoveToKiller();
                Invoke("AfficheGraph", 3);
            }
            else
            {
                AfficheGraph();

            }

        }
    }

    public void GivePoint(int numPlayer, int nbPoints, Vector3 Position)
    {
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
        AfficheText instance = Instantiate(AfficheurScore.gameObject, canvas.transform).GetComponent<AfficheText>();
        instance.AfficheAtPosition("10", Position, 1, PlayColors[numPlayer-1]);

    }

}
