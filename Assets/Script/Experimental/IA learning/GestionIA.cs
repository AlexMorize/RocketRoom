using Assets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GestionIA : GestionnaireDeJeu
{
    public List<ReseauNeural> CerveauxSauvegarde = new List<ReseauNeural>();
    public ArrayList LesCerveauxTester = new ArrayList();
    public Text AfficheInfo, Classement;
    public bool ShowBest;
    [Range(0,5)]
    public float TimeSpeed = 3;

    public float Moyenne = 1, last = 1;

    const int NbRoquetteMax = 4, NbEnemyMax = 3, NbIAPerGeneration = 200;
    int nbToSpawn = 0;
    public static int NbEntree { get; private set; }
    int Generation = 0;
    int IndexTestIA = 0;
    float BestScore = 0, BestScoreGeneration = 0;

    public void SaveCerveaux()
    {

        string result = "";
        for (int i = 0; i < CerveauxSauvegarde.Count; i++)
        {
            if (i > NbIAPerGeneration) break;
            if (i != 0) result += '[';
            result += (CerveauxSauvegarde[i] as ReseauNeural).SaveToString();
        }
        System.IO.File.WriteAllText(@"C:\Users\alexa\OneDrive\Bureau\RocketRoom\SaveIA\save3.txt", result);
    }

    public void GetNextGeneration(float CoefficientRandomization)
    {
        /*foreach(var player in FindObjectsOfType<Joueur>())
        {
            Kill
        }*/
        Time.timeScale = 0;
        System.IO.File.WriteAllText(@"C:\Users\alexa\OneDrive\Bureau\RocketRoom\SaveIA\Classement.txt", ClassementToString(NbIAPerGeneration));

        string Text = "";
        if (System.IO.File.Exists(@"C:\Users\alexa\OneDrive\Bureau\RocketRoom\SaveIA\Stats.txt"))
        {
            Text += System.IO.File.ReadAllText(@"C:\Users\alexa\OneDrive\Bureau\RocketRoom\SaveIA\Stats.txt");
            Text += System.Environment.NewLine;
        }
        Text += System.DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss") + " - Moyenne: " + Moyenne;
        System.IO.File.WriteAllText(@"C:\Users\alexa\OneDrive\Bureau\RocketRoom\SaveIA\Stats.txt", Text);


        LesCerveauxTester.RemoveRange(NbIAPerGeneration / 2, LesCerveauxTester.Count - NbIAPerGeneration / 2);// On supprime les cerveaux tester les plus faible
        CerveauxSauvegarde.Clear();// On supprime l'ancienne sauvegarde de cerveau
        foreach (SauvegardeIA aSave in LesCerveauxTester)
        {
            CerveauxSauvegarde.Add(aSave.Cerveau);// On ajoute les meilleurs cerveau tester au cerveau à spawn
            CerveauxSauvegarde.Add(aSave.Cerveau);// et leurs clones
        }

        for (int i = NbIAPerGeneration / 2; i < NbIAPerGeneration; i++)//Pour la dernière moité des cerveau à spawn;
        {
            (CerveauxSauvegarde[i]).Randomize(CoefficientRandomization);//On modifie légerement leur comportement
        }
        LesCerveauxTester.Clear();
        BestScoreGeneration = 0;
        IndexTestIA = 0;//On réinitialise le test;
        Generation++;
        SaveCerveaux();

        Time.timeScale = TimeSpeed;
    }

    public void LoadCerveau()
    {
        CerveauxSauvegarde.Clear();
        if (File.Exists(@"C:\Users\alexa\OneDrive\Bureau\RocketRoom\SaveIA\save3.txt"))
        {
            string[] Cerv = File.ReadAllText(@"C:\Users\alexa\OneDrive\Bureau\RocketRoom\SaveIA\save3.txt").Split('[');
            for (int i = 0; i < Cerv.Length; i++)
            {
                if (i >= NbIAPerGeneration) break;
                ReseauNeural UneSave = ReseauNeural.CreateRandom(NbEntree, 5, 5, 0);
                UneSave.LoadByString(Cerv[i]);
                CerveauxSauvegarde.Add(UneSave);
            }
        }
    }

    public void AddCerveau(int _score, ReseauNeural _cerveau)
    {

        int PositionCerveau = 0;
        foreach (SauvegardeIA save in LesCerveauxTester)
        {
            if (save.Score < _score) break;
            else
                PositionCerveau++;
        }

        LesCerveauxTester.Insert(PositionCerveau, new SauvegardeIA(_score, _cerveau));

        // SaveCerveaux();




        /*if (lesCerveaux.Count > 50)
        {
            lesCerveaux.RemoveAt(50);
            lesCerveaux.RemoveRange(50, lesCerveaux.Count - 50);
        }*/
        //AfficheTemps.text = "Meilleur: " + (lesCerveaux[0] as SauvegardeIA).Score + System.Environment.NewLine + "Pire: " + (lesCerveaux[lesCerveaux.Count - 1] as SauvegardeIA).Score + System.Environment.NewLine + "Last: "+last;
        //Moyenne = (lesCerveaux[0] as SauvegardeIA).Score + (lesCerveaux[lesCerveaux.Count - 1] as SauvegardeIA).Score / 2;

    }

    string ClassementToString(int MaxResult)
    {

        Moyenne = 0;
        string result = "";
        for (int i = 0; i < LesCerveauxTester.Count; i++)
        {
            if (i > MaxResult) break;
            float score = (LesCerveauxTester[i] as SauvegardeIA).Score;
            result += "[" + (i + 1) + "] " + score + System.Environment.NewLine;
            Moyenne += score;
        }
        Moyenne = Moyenne / LesCerveauxTester.Count;

        return result;
    }

    /*void Spawn()
    {
        if (FindObjectOfType<IALearning>()) return;
        IALearning Instance = Instantiate(ModelJoueur, FindObjectOfType<Spawner>().transform.position, new Quaternion()).GetComponent<IALearning>();
       // Debug.Log("CountCerveau " + lesCerveaux.Count);
        if (lesCerveaux.Count < 50)
        {
            Instance.SetCerveau( ReseauNeural.CreateRandom(NbEntree, 5, 5, 10));
        }
        else
        {
            if (ShowBest) Instance.SetCerveau((lesCerveaux[0] as SauvegardeIA).Cerveau);
            else
            {
                Instance.SetCerveau((lesCerveaux[Random.Range(0, 50)] as SauvegardeIA).Cerveau);
                Instance.monCerveau.Cerveau.Randomize(.1f / Moyenne);
            }
        }

    }*/

    void Spawn()
    {
        nbToSpawn--;
        //if (FindObjectsOfType<IALearning>().Length>=4) return;
        IALearning Instance = Instantiate(ModelJoueur, LesSpawn[Random.Range(0,LesSpawn.Count)].transform.position, Quaternion.Euler(0,90,0)).GetComponent<IALearning>();

        Instance.SetCerveau(CerveauxSauvegarde[Random.Range(0,CerveauxSauvegarde.Count)]);
        CerveauxSauvegarde.Remove(Instance.monCerveau.Cerveau);
        Instance.NumPlayer = IndexTestIA + 1;
        Instance.Couleur = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        AfficheInfo.text = "Generation: " + Generation + System.Environment.NewLine + "Index: " + IndexTestIA + System.Environment.NewLine + "BestScore: " + Mathf.Round(BestScore) + System.Environment.NewLine + "BestScoreGen: " + Mathf.Round(BestScoreGeneration);
        Classement.text = ClassementToString(NbIAPerGeneration);

        IndexTestIA++;
        if (IndexTestIA >= NbIAPerGeneration) GetNextGeneration(0.1f);


    }

    public override void Kill(Joueur Victime, int IdTueur)
    {
        if (Victime.isDestroy) return;
        if (Victime.NumPlayer != IdTueur)
        {
            foreach (var player in FindObjectsOfType<IALearning>())
            {
                if (player.NumPlayer == IdTueur)
                {
                    player.nbTué += 1;
                }
            }
        }
        else
        {
            Victime.nbTué -= 1;
        }

        int Note = Mathf.RoundToInt((Victime.nbTué +1.2f) * Victime.GetSurvivalTime());
        if (Note > BestScore) BestScore = Note;
        if (Note > BestScoreGeneration) BestScoreGeneration = Note;

        AddCerveau(Note, Victime.GetBrain());
        //LesCerveauxTester.Add(new SauvegardeIA(Note,Victime.GetBrain()));
        Invoke("Spawn", 2 + nbToSpawn);
        nbToSpawn++;

        Victime.Kill();



    }

    private void Start()
    {
        LesSpawn.AddRange(FindObjectsOfType<Spawner>());
        nbToSpawn = 0;
        LesCerveauxTester.Clear();
        CerveauxSauvegarde.Clear();
        NbEntree = 5 + 8 * NbRoquetteMax + 5 * NbEnemyMax;
        LoadCerveau();
        if (CerveauxSauvegarde.Count < NbIAPerGeneration)
        {
            for (int i = CerveauxSauvegarde.Count; i < NbIAPerGeneration; i++)
            {
                CerveauxSauvegarde.Add(ReseauNeural.CreateRandom(NbEntree, 5, 5, 10));
            }
        }
        Time.timeScale = TimeSpeed;

        Spawn();
        Spawn();
        Spawn();
        Spawn();

    }

    private void Update()
    {
        Time.timeScale = TimeSpeed;
    }
}

public class SauvegardeIA
{


    public int Score;
    public ReseauNeural Cerveau;

    public SauvegardeIA()
    {

    }

    public string SaveToString()
    {
        string result = "";
        result += Score.ToString() + " ";
        result += Cerveau.SaveToString();
        return result;
    }

    public void LoadByString(string SaveString)
    {
        string[] Inputs = SaveString.Split(' ');
        Score = System.Convert.ToInt32(Inputs[0]);
        Cerveau = new ReseauNeural(GestionIA.NbEntree, 5, 5);
        Cerveau.LoadByString(Inputs[1]);
    }

    public SauvegardeIA(int score, ReseauNeural cerveau)
    {


        Score = score;
        Cerveau = cerveau;
    }

}
