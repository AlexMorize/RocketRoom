using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuVersus : MonoBehaviour
{
    public Button FlecheGaucheCarte, FlecheDroiteCarte, btn_StartGame, btn_QuitGame, btn_ConfigManette, btn_TempsPlus, btn_TempsMoins;
    public Text TextCarte, TextTemps;
    public Toggle UseClavier, UseOnlyManette;
    public Image AffichageJoueurPrésent, SampleAffichageJoueur;
    public Texture2D StyleCurseur;
    public ConfigManette MenuConfigManette;
    public Camera cam;
    public Transform Texte3D;
    public GestionnaireDeJeu GameController;
    public GameObject Map;
    public GameObject AllMenu;
    public List<GameObject> ListeCarte;
    public GameObject LaCarteActuelle;

    float BeginTurnTime;

    bool MenuIsReady = false;

    int temps = 3;
    private int NbJoueurValide, NbControllerValide, indexCarte = 1;
    private Component selectedMenu;
    private List<GameObject> LesAffichageJoueurs = new List<GameObject>();

    public Component SelectedMenu {
        get => selectedMenu;
        set
        {
            BeginTurnTime = Time.time;
            selectedMenu = value;
        }
    }

    void FlecheDroite_Clic()
    {
        indexCarte++;
        if (indexCarte >= ListeCarte.Count) indexCarte = 0;

        Map = ListeCarte[indexCarte];
        TextCarte.text = Map.name;

    }

    void FlecheGauche_Clic()
    {
        indexCarte--;
        if (indexCarte < 0) indexCarte = ListeCarte.Count-1;

        Map = ListeCarte[indexCarte];
        TextCarte.text = Map.name;

    }

    void TempsPlus()
    {
        temps++;
        if (temps > 10) temps = 10;
        TextTemps.text = temps.ToString();
        GameController.TempsDeJeu = 60 * temps;
    }

    void TempsMoins()
    {
        temps--;
        if (temps < 1) temps = 1;
        TextTemps.text = temps.ToString();
        GameController.TempsDeJeu = 60 * temps;
    }

    public void SelectMenu()
    {
        Vector3 direction = SelectedMenu.transform.position - cam.transform.position;
        Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, toRotation, (Time.time - BeginTurnTime)*.1f);
        cam.transform.eulerAngles -= Vector3.forward * cam.transform.eulerAngles.z;
    }

    void StartMenu()
    {
        selectedMenu = this;
        BeginTurnTime = Time.time;

        MenuIsReady = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //MenuConfigManette.gameObject.SetActive(false);
        Time.timeScale = 1;
        SelectedMenu = Texte3D;
        Invoke("StartMenu", 2);
        Cursor.visible = true;
        Cursor.SetCursor(StyleCurseur, new Vector2(StyleCurseur.width / 2, StyleCurseur.height / 2), CursorMode.ForceSoftware);
        UseClavier.onValueChanged.AddListener(ChangeUseClavier);
        UseClavier.isOn = GestionnaireDeJeu.UtiliséClavierSouris;
        UseOnlyManette.isOn = !GestionnaireDeJeu.UtiliséClavierSouris;
        btn_StartGame.onClick.AddListener(StartGame);
        btn_QuitGame.onClick.AddListener(QuitGame);
        btn_ConfigManette.onClick.AddListener(() => { SelectedMenu = MenuConfigManette; });
        btn_TempsMoins.onClick.AddListener(TempsMoins);
        btn_TempsPlus.onClick.AddListener(TempsPlus);
        FlecheDroiteCarte.onClick.AddListener(FlecheDroite_Clic);
        FlecheGaucheCarte.onClick.AddListener(FlecheGauche_Clic);
        FlecheDroite_Clic();
        InvokeRepeating("MajAffichage", 0, 1);
        MenuConfigManette.Btn_Save.onClick.AddListener(() => { SelectedMenu = this; });


#if UNITY_ANDROID
        UseClavier.GetComponentInChildren<Text>().text = "Utiliser le tactile";
#endif


        if (Input.GetJoystickNames().Length>1)
        {
            UseClavier.isOn = false;
            UseOnlyManette.isOn = true;
        }
    }




    private void Update()
    {
        SelectMenu();
    }

    void ChangeUseClavier(bool etat)
    {
        GestionnaireDeJeu.UtiliséClavierSouris = etat;
        MajAffichage();
    }

    void StartGame()
    {
        //SceneManager.LoadScene("SceneAlexis1");
        Map.SetActive(true);
        GameController.gameObject.SetActive(true);
        AllMenu.SetActive(false);
    }

    void QuitGame()
    {
        Application.Quit();
    }

    void MajAffichage()
    {
        if (!MenuIsReady) return;
        bool NombreMaxManetteAtteint = false;
        int nbPlayer = 0;
        NbJoueurValide = 0;
        NbControllerValide = 0;
        foreach (var unAffichage in LesAffichageJoueurs)
        {
            Destroy(unAffichage);
        }
        LesAffichageJoueurs.Clear();
        SampleAffichageJoueur.gameObject.SetActive(true);
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (i == 10)
            {
                NombreMaxManetteAtteint = true;
                break;
            }
            GameObject instance = Instantiate(SampleAffichageJoueur.gameObject, AffichageJoueurPrésent.transform);
            if (Input.GetJoystickNames()[i] == "")
            {
                instance.GetComponentInChildren<Text>().text = "Joueur " + (i + 1) + " - Disconnected";
                instance.GetComponentInChildren<Text>().color = Color.red;
            }
            else
            {
                NbJoueurValide ++;
                NbControllerValide ++;
                if (!ProfilControle.ProfilIsConfigured(Input.GetJoystickNames()[i]))
                {
                    instance.GetComponentInChildren<Text>().text = "Joueur " + (i + 1) + " - La Manette n'est pas configurer";
                    instance.GetComponentInChildren<Text>().color = Color.red;
                    SelectedMenu = MenuConfigManette;
                    MenuConfigManette.SetCurrentController(i);
                    MenuConfigManette.MessageUtilisateur.text = "Merci de completer la configuration de votre manette pour l'utiliser";

                }
                else
                {
                    instance.GetComponentInChildren<Text>().text = "Joueur " + (i + 1) + " - " + Input.GetJoystickNames()[i];
                }
            }
            instance.transform.position -= Vector3.up * 60 * i;
            LesAffichageJoueurs.Add(instance);
            nbPlayer++;
        }
        if (GestionnaireDeJeu.UtiliséClavierSouris)
        {
            GameObject instance = Instantiate(SampleAffichageJoueur.gameObject, AffichageJoueurPrésent.transform);
            instance.GetComponentInChildren<Text>().text = "Joueur " + (Input.GetJoystickNames().Length + 1) + " - Clavier-Souris";
            instance.transform.position -= Vector3.up * 60 * Input.GetJoystickNames().Length;
            LesAffichageJoueurs.Add(instance);
            nbPlayer++;
            NbJoueurValide++;
        }
        if (NombreMaxManetteAtteint)
        {
            GameObject instance = Instantiate(SampleAffichageJoueur.gameObject, AffichageJoueurPrésent.transform);
            instance.GetComponentInChildren<Text>().text = "Impossible de connecter plus de 6 manettes";
            instance.GetComponentInChildren<Text>().color = Color.red;
            instance.transform.position -= Vector3.up * 60 * nbPlayer;
            LesAffichageJoueurs.Add(instance);
        }

        btn_ConfigManette.interactable = NbControllerValide > 0;
        btn_StartGame.interactable = NbJoueurValide > 0;

        if (btn_StartGame.interactable)
            btn_StartGame.GetComponentInChildren<Text>().color = Color.green;
        else
            btn_StartGame.GetComponentInChildren<Text>().color = Color.gray;

        if (btn_ConfigManette.interactable)
            btn_ConfigManette.GetComponentInChildren<Text>().color = Color.green;
        else
            btn_ConfigManette.GetComponentInChildren<Text>().color = Color.gray;

        SampleAffichageJoueur.gameObject.SetActive(false);
    }

}
