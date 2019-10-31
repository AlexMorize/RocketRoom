using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ConfigManette : MonoBehaviour
{
    public Button Btn_FlecheGauche, Btn_FlecheDroite, Btn_Save;
    public Text NomManette;
    public Text MessageUtilisateur;

    public Button Btn_X, Btn_Y, Btn_X2, Btn_Y2;
    public Toggle Invert_X, Invert_Y, Invert_X2, Invert_Y2;
    public Button Btn_Jump, Btn_Shoot;
    public RectTransform SpritesManettes;

    private Dictionary<string, GameObject> LesSpritesManettes = new Dictionary<string, GameObject>();
    private List<string> LesAxes;
    public int CurrentController;
    private bool GetAxe, GetBtn;
    private Button ButtonModif;
    private string CurrentModif;

    public void SetCurrentController(int NumController)
    {
        CurrentController = NumController;
        NomManette.text = Input.GetJoystickNames()[NumController];
    }

    void ClicFlecheGauche()
    {
        SaveConfig();
        CurrentController--;
        if (CurrentController < 0) CurrentController = Input.GetJoystickNames().Length-1;
        if(Input.GetJoystickNames()[CurrentController]=="")
        {
            ClicFlecheGauche();
            return;
        }
        else
        {
            NomManette.text = Input.GetJoystickNames()[CurrentController];
            LoadConfig();
        }
    }
    void ClicFlecheDroite()
    {
        SaveConfig();
        CurrentController++;
        if (CurrentController > Input.GetJoystickNames().Length - 1) CurrentController = 0;
        if (Input.GetJoystickNames()[CurrentController] == "")
        {
            ClicFlecheDroite();
            return;
        }
        else
        {
            NomManette.text = Input.GetJoystickNames()[CurrentController];
            LoadConfig();
        }
    }

    static List<string> GetAxes()
    {
        List<string> result = new List<string>();
        /*var inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];

        SerializedObject obj = new SerializedObject(inputManager);

        SerializedProperty axisArray = obj.FindProperty("m_Axes");

        if (axisArray.arraySize == 0)
            DebugAndroid.Log("No Axes");

        string AllAxis = "";

        for (int i = 0; i < axisArray.arraySize; ++i)
        {
            var axis = axisArray.GetArrayElementAtIndex(i);
            result.Add(axis.FindPropertyRelative("m_Name").stringValue);
            AllAxis += axis.FindPropertyRelative("m_Name").stringValue + ";";
        }
        DebugAndroid.Log(AllAxis);*/
        string AllAxes = "joystick 1 analog 0;joystick 1 analog 1;joystick 1 analog 2;joystick 1 analog 3;joystick 1 analog 4;joystick 1 analog 5;joystick 1 analog 6;joystick 1 analog 7;joystick 1 analog 8;joystick 1 analog 9;joystick 1 analog 10;joystick 1 analog 11;joystick 1 analog 12;joystick 1 analog 13;joystick 1 analog 14;joystick 1 analog 15;joystick 1 analog 16;joystick 1 analog 17;joystick 1 analog 18;joystick 1 analog 19;joystick 2 analog 0;joystick 2 analog 1;joystick 2 analog 2;joystick 2 analog 3;joystick 2 analog 4;joystick 2 analog 5;joystick 2 analog 6;joystick 2 analog 7;joystick 2 analog 8;joystick 2 analog 9;joystick 2 analog 10;joystick 2 analog 11;joystick 2 analog 12;joystick 2 analog 13;joystick 2 analog 14;joystick 2 analog 15;joystick 2 analog 16;joystick 2 analog 17;joystick 2 analog 18;joystick 2 analog 19;joystick 3 analog 0;joystick 3 analog 1;joystick 3 analog 2;joystick 3 analog 3;joystick 3 analog 4;joystick 3 analog 5;joystick 3 analog 6;joystick 3 analog 7;joystick 3 analog 8;joystick 3 analog 9;joystick 3 analog 10;joystick 3 analog 11;joystick 3 analog 12;joystick 3 analog 13;joystick 3 analog 14;joystick 3 analog 15;joystick 3 analog 16;joystick 3 analog 17;joystick 3 analog 18;joystick 3 analog 19;joystick 4 analog 0;joystick 4 analog 1;joystick 4 analog 2;joystick 4 analog 3;joystick 4 analog 4;joystick 4 analog 5;joystick 4 analog 6;joystick 4 analog 7;joystick 4 analog 8;joystick 4 analog 9;joystick 4 analog 10;joystick 4 analog 11;joystick 4 analog 12;joystick 4 analog 13;joystick 4 analog 14;joystick 4 analog 15;joystick 4 analog 16;joystick 4 analog 17;joystick 4 analog 18;joystick 4 analog 19;joystick 5 analog 0;joystick 5 analog 1;joystick 5 analog 2;joystick 5 analog 3;joystick 5 analog 4;joystick 5 analog 5;joystick 5 analog 6;joystick 5 analog 7;joystick 5 analog 8;joystick 5 analog 9;joystick 5 analog 10;joystick 5 analog 11;joystick 5 analog 12;joystick 5 analog 13;joystick 5 analog 14;joystick 5 analog 15;joystick 5 analog 16;joystick 5 analog 17;joystick 5 analog 18;joystick 5 analog 19;joystick 6 analog 0;joystick 6 analog 1;joystick 6 analog 2;joystick 6 analog 3;joystick 6 analog 4;joystick 6 analog 5;joystick 6 analog 6;joystick 6 analog 7;joystick 6 analog 8;joystick 6 analog 9;joystick 6 analog 10;joystick 6 analog 11;joystick 6 analog 12;joystick 6 analog 13;joystick 6 analog 14;joystick 6 analog 15;joystick 6 analog 16;joystick 6 analog 17;joystick 6 analog 18;joystick 6 analog 19;joystick 7 analog 0;joystick 7 analog 1;joystick 7 analog 2;joystick 7 analog 3;joystick 7 analog 4;joystick 7 analog 5;joystick 7 analog 6;joystick 7 analog 7;joystick 7 analog 8;joystick 7 analog 9;joystick 7 analog 10;joystick 7 analog 11;joystick 7 analog 12;joystick 7 analog 13;joystick 7 analog 14;joystick 7 analog 15;joystick 7 analog 16;joystick 7 analog 17;joystick 7 analog 18;joystick 7 analog 19;joystick 8 analog 0;joystick 8 analog 1;joystick 8 analog 2;joystick 8 analog 3;joystick 8 analog 4;joystick 8 analog 5;joystick 8 analog 6;joystick 8 analog 7;joystick 8 analog 8;joystick 8 analog 9;joystick 8 analog 10;joystick 8 analog 11;joystick 8 analog 12;joystick 8 analog 13;joystick 8 analog 14;joystick 8 analog 15;joystick 8 analog 16;joystick 8 analog 17;joystick 8 analog 18;joystick 8 analog 19;joystick 9 analog 0;joystick 9 analog 1;joystick 9 analog 2;joystick 9 analog 3;joystick 9 analog 4;joystick 9 analog 5;joystick 9 analog 6;joystick 9 analog 7;joystick 9 analog 8;joystick 9 analog 9;joystick 9 analog 10;joystick 9 analog 11;joystick 9 analog 12;joystick 9 analog 13;joystick 9 analog 14;joystick 9 analog 15;joystick 9 analog 16;joystick 9 analog 17;joystick 9 analog 18;joystick 9 analog 19;joystick 10 analog 0;joystick 10 analog 1;joystick 10 analog 2;joystick 10 analog 3;joystick 10 analog 4;joystick 10 analog 5;joystick 10 analog 6;joystick 10 analog 7;joystick 10 analog 8;joystick 10 analog 9;joystick 10 analog 10;joystick 10 analog 11;joystick 10 analog 12;joystick 10 analog 13;joystick 10 analog 14;joystick 10 analog 15;joystick 10 analog 16;joystick 10 analog 17;joystick 10 analog 18;joystick 10 analog 19;mouse x;mouse y;mouse z;Horizontal;Vertical;Fire1;Fire2;Fire3;Jump;Mouse X;Mouse Y;Mouse ScrollWheel;Horizontal;Vertical;Fire1;Fire2;Fire3;Jump;Submit;Submit;Cancel";

        result.AddRange(AllAxes.Split(';'));

        return result;
    }

    void SaveConfig()
    {
        new ProfilControle(Input.GetJoystickNames()[CurrentController], "Joystick",
            Btn_X.GetComponentInChildren<Text>().text, Invert_X.isOn,
            Btn_Y.GetComponentInChildren<Text>().text, Invert_Y.isOn,
            Btn_X2.GetComponentInChildren<Text>().text, Invert_X2.isOn,
            Btn_Y2.GetComponentInChildren<Text>().text, Invert_Y2.isOn,
            Btn_Jump.GetComponentInChildren<Text>().text,
            Btn_Shoot.GetComponentInChildren<Text>().text
            ).SaveProfil();
    }

    void LoadConfig()
    {
        if (PlayerPrefs.GetString(Input.GetJoystickNames()[CurrentController])=="")
        {
            Reinit();
            return;
        }

        ProfilControle SavedProfil = ProfilControle.LoadProfil(Input.GetJoystickNames()[CurrentController]);
        Btn_X.GetComponentInChildren<Text>().text = SavedProfil.Axe_X;
        Invert_X.isOn = SavedProfil.InvertX;
        Btn_Y.GetComponentInChildren<Text>().text = SavedProfil.Axe_Y;
        Invert_Y.isOn = SavedProfil.InvertY;
        Btn_X2.GetComponentInChildren<Text>().text = SavedProfil.Axe_X2;
        Invert_X2.isOn = SavedProfil.InvertX2;
        Btn_Y2.GetComponentInChildren<Text>().text = SavedProfil.Axe_Y2;
        Invert_Y2.isOn = SavedProfil.InvertY2;
        Btn_Jump.GetComponentInChildren<Text>().text = SavedProfil.Key_Jump;
        Btn_Shoot.GetComponentInChildren<Text>().text = SavedProfil.Key_Shoot;
    }

    void Reinit()
    {
        Btn_X.GetComponentInChildren<Text>().text = "";
        Invert_X.isOn = false;
        Btn_Y.GetComponentInChildren<Text>().text = "";
        Invert_Y.isOn = true;
        Btn_X2.GetComponentInChildren<Text>().text = "";
        Invert_X2.isOn = false;
        Btn_Y2.GetComponentInChildren<Text>().text = "";
        Invert_Y2.isOn = true;
        Btn_Jump.GetComponentInChildren<Text>().text = "";
        Btn_Shoot.GetComponentInChildren<Text>().text = "";
        MessageUtilisateur.text = "";
    }

    void ModifierUnAxes(Button btn)
    {
        HideAllSpritesManette();
        if (ButtonModif != null)//Pour annuler si un bouton à été selectionner sans modification
        {
            ButtonModif.colors = ColorBlock.defaultColorBlock;
            GetBtn = false;
            
        }

        GetAxe = true;
        ButtonModif = btn;
        ColorBlock NewColors = new ColorBlock();
        NewColors.normalColor = Color.gray;
        NewColors.pressedColor = Color.gray;
        NewColors.highlightedColor = Color.gray;
        NewColors.disabledColor = Color.gray;
        NewColors.fadeDuration = 1;
        NewColors.colorMultiplier = 1;
        btn.colors = NewColors;
        ShowSpriteManette(btn.gameObject.name.Replace("Btn_Config", ""));
    }

    void ModifierUnBouton(Button btn)
    {
        HideAllSpritesManette();
        if (ButtonModif != null)//Pour annuler si un bouton à été selectionner sans modification
        {
            ButtonModif.colors = ColorBlock.defaultColorBlock;
            GetAxe = false;
        }

        GetBtn = true;
        ButtonModif = btn;
        ColorBlock NewColors = new ColorBlock();
        NewColors.normalColor = Color.gray;
        NewColors.pressedColor = Color.gray;
        NewColors.highlightedColor = Color.gray;
        NewColors.disabledColor = Color.gray;
        NewColors.fadeDuration = 1;
        NewColors.colorMultiplier = 1;
        btn.colors = NewColors;
        ShowSpriteManette(btn.gameObject.name.Replace("Btn_Config", ""));
    }

    void ShowSpriteManette(string SpriteName)
    {
        LesSpritesManettes[SpriteName].SetActive(true);
    }

    void HideAllSpritesManette()
    {
        foreach (GameObject current in LesSpritesManettes.Values)
            current.SetActive(false);
    }

    private void Awake()
    {
        foreach (Image unSprite in SpritesManettes.gameObject.GetComponentsInChildren<Image>())
        {
            LesSpritesManettes.Add(unSprite.gameObject.name, unSprite.gameObject);
            unSprite.gameObject.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LesAxes = GetAxes();
        Btn_FlecheDroite.onClick.AddListener(ClicFlecheDroite);
        Btn_FlecheGauche.onClick.AddListener(ClicFlecheGauche);
        
        Btn_X.onClick.AddListener(()=> ModifierUnAxes(Btn_X));
        Btn_Y.onClick.AddListener(() => ModifierUnAxes(Btn_Y));
        Btn_X2.onClick.AddListener(() => ModifierUnAxes(Btn_X2));
        Btn_Y2.onClick.AddListener(() => ModifierUnAxes(Btn_Y2));
        Btn_Jump.onClick.AddListener(() => ModifierUnBouton(Btn_Jump));
        Btn_Shoot.onClick.AddListener(() => ModifierUnBouton(Btn_Shoot));
        Btn_Save.onClick.AddListener(SaveConfig);

        

        NomManette.text = Input.GetJoystickNames()[CurrentController];
        LoadConfig();
        

    }

    // Update is called once per frame
    void Update()
    {
        if (GetAxe)
        {
            foreach (var axe in LesAxes)
            {
                if (axe.Contains("joystick " + (CurrentController+1)))
                {
                    float AxisValue = Input.GetAxis(axe);
                    if (AxisValue == 1 || AxisValue == -1)
                    {
                        string ShortName = axe.Replace("joystick " + (CurrentController+1), "");
                        ButtonModif.GetComponentInChildren<Text>().text = CurrentModif = ShortName;
                        ButtonModif.colors = ColorBlock.defaultColorBlock;
                        GetAxe = false;
                        ButtonModif = null;
                        HideAllSpritesManette();
                        break;
                    }
                }
            }
        }
        if(GetBtn)
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if(kcode.ToString().Contains("Joystick"+ (CurrentController + 1)))
                if (Input.GetKeyDown(kcode))
                {
                        string ShortName = kcode.ToString().Replace("Joystick" + (CurrentController + 1), "");
                        ShortName = ShortName.Replace("Button", "button ");
                        ButtonModif.GetComponentInChildren<Text>().text = CurrentModif = ShortName;
                        ButtonModif.colors = ColorBlock.defaultColorBlock;
                        GetBtn = false;
                        ButtonModif = null;
                        HideAllSpritesManette();
                        break;
                    }
            }
        }
    }
}
