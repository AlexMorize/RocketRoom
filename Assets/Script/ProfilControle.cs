using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfilControle : MonoBehaviour
{
    public string NomProfil, PreWord = "Joystick", Axe_X;
    public bool InvertX;
    public string Axe_Y;
    public bool InvertY;
    public string Axe_X2;
    public bool InvertX2;
    public string Axe_Y2;
    public bool InvertY2;

    public string Key_Jump, Key_Shoot;



    #region Constructeur
    public ProfilControle(string nomProfil, string preWord, string axe_X, bool invertX, string axe_Y, bool invertY, string axe_X2, bool invertX2, string axe_Y2, bool invertY2, string key_Jump, string key_Shoot)
    {
        NomProfil = nomProfil;
        PreWord = preWord;
        Axe_X = axe_X;
        InvertX = invertX;
        Axe_Y = axe_Y;
        InvertY = invertY;
        Axe_X2 = axe_X2;
        InvertX2 = invertX2;
        Axe_Y2 = axe_Y2;
        InvertY2 = invertY2;
        Key_Jump = key_Jump;
        Key_Shoot = key_Shoot;
    }

    public ProfilControle()
    {

    }
    #endregion

    public static ProfilControle GetMyProfil(Joueur player)
    {
        if (player.NumPlayer - 1 < Input.GetJoystickNames().Length)
            return getTypeControl(Input.GetJoystickNames()[player.NumPlayer - 1]).CreateProfileForPlayer(player.NumPlayer);
        else
        {
#if UNITY_ANDROID
            return new ProfilTactile();

#endif
            return getTypeControl("Clavier/Souris").CreateProfileForPlayer(player.NumPlayer);
        }

    }

    public static ProfilControle GetMyProfil(NetwrokedPlayer player)
    {
        if (Input.GetJoystickNames().Length>0)
            return getTypeControl(Input.GetJoystickNames()[0]).CreateProfileForPlayer(1);
        else
        {
#if UNITY_ANDROID
            return new ProfilTactile();

#endif
            return getTypeControl("Clavier/Souris").CreateProfileForPlayer(player.NumPlayer);
        }

    }

    /*public static bool CheckJoytsickCompatibility(string nomControlleur)
    {
        DebugAndroid.Log("Check manette : " + getTypeControl(nomControlleur).NomProfil);
        return getTypeControl(nomControlleur).NomProfil != "Clavier/Souris";
        
    }*/



    static ProfilControle getTypeControl(string nomControlleur)
    {
        return LoadProfil(nomControlleur);

        /*switch(nomControlleur)
        {
            case "2In1 USB Joystick":
                return new ProfilControle(nomControlleur, "Joystick", "analog 0", false, "analog 1", true, "analog 2", false, "analog 3", true, "button 2", "button 5");

            case "PLAYSTATION(R)3 Controller":
            case "PS3 GamePad":
                return new ProfilControle(nomControlleur, "Joystick", "Axis 0", false, "Axis 1", true, "Axis 2", false, "Axis 3", true, "button 14", "button 11");

            case "Xbox Bluetooth Gamepad" :
            case "Controller (Xbox One For Windows)":
                return new ProfilControle(nomControlleur, "Joystick", "analog 0", false, "analog 1", true, "analog 3", false, "analog 4", true, "button 0", "button 5");

            case "Wireless Controller":
                return new ProfilControle(nomControlleur, "Joystick", "Axis 0", false, "Axis 1", true, "Axis 2", false, "Axis 5", true, "button 1", "button 5");

            default:
                return new ProfilControle("Clavier/Souris", "", "Horizontal", false, "Vertical", false, "Mouse X", false, "Mouse Y", false, "space", "mouse 0");
        }*/
    }

    public static bool ProfilIsConfigured(string _nomProfil)
    {
        if (PlayerPrefs.GetString(_nomProfil) == "") return false;

        var TestProfil = LoadProfil(_nomProfil);
        return TestProfil.Axe_X != "" &&
            TestProfil.Axe_Y != "" &&
            TestProfil.Axe_X2 != "" &&
            TestProfil.Axe_Y2 != "" &&
            TestProfil.Key_Jump != "" &&
            TestProfil.Key_Shoot != "";
    }


    ProfilControle CreateProfileForPlayer(int NumPlayer)
    {
        if (PreWord == "")
        {
            return this;
        }
        else
        {
            return new ProfilControle(this.NomProfil, "",
                PreWord.ToLower() + " " + NumPlayer + "" + Axe_X, this.InvertX,
                PreWord.ToLower() + " " + NumPlayer + "" + Axe_Y, this.InvertY,
                PreWord.ToLower() + " " + NumPlayer + "" + Axe_X2, InvertX2,
                PreWord.ToLower() + " " + NumPlayer + "" + Axe_Y2, InvertY2,
                PreWord.ToLower() + " " + NumPlayer + " " + Key_Jump,
                PreWord.ToLower() + " " + NumPlayer + " " + Key_Shoot);
        }
    }

    public override string ToString()
    {
        string SaveString = NomProfil + ";" +
            PreWord + ";" +
            Axe_X + ";" + InvertX.ToString() + ";" +
            Axe_Y + ";" + InvertY.ToString() + ";" +
            Axe_X2 + ";" + InvertX2.ToString() + ";" +
            Axe_Y2 + ";" + InvertY2.ToString() + ";" +
            Key_Jump + ";" +
            Key_Shoot;
        return SaveString;
    }

    public void SaveProfil()
    {
        string SaveString =
            PreWord + ";" +
            Axe_X + ";" + InvertX.ToString() + ";" +
            Axe_Y + ";" + InvertY.ToString() + ";" +
            Axe_X2 + ";" + InvertX2.ToString() + ";" +
            Axe_Y2 + ";" + InvertY2.ToString() + ";" +
            Key_Jump + ";" +
            Key_Shoot;

        PlayerPrefs.SetString(NomProfil, SaveString);
        DebugAndroid.Log(NomProfil + '\n' + SaveString);
    }

    public static ProfilControle LoadProfil(string NomProfil)
    {
        string Save = PlayerPrefs.GetString(NomProfil);
        if (Save == "") return null;

        string[] SavedValues = Save.Split(';');
        return new ProfilControle(NomProfil,
            SavedValues[0],
            SavedValues[1], SavedValues[2] == "True",
            SavedValues[3], SavedValues[4] == "True",
            SavedValues[5], SavedValues[6] == "True",
            SavedValues[7], SavedValues[8] == "True",
            SavedValues[9],
            SavedValues[10]);
    }

    public virtual float GetX()
    {
        if (NomProfil == "Clavier/Souris")
        {
            if (Mathf.Abs(Input.GetAxisRaw(Axe_X)) < .8f) return 0;
            if (InvertX) return -Input.GetAxisRaw(Axe_X);
            else return Input.GetAxisRaw(Axe_X);
        }
        else
        {
            if (Mathf.Abs(Input.GetAxis(Axe_X)) < .8f) return 0;
            if (InvertX) return -Input.GetAxis(Axe_X);
            else return Input.GetAxis(Axe_X);
        }
    }
    public virtual float GetX2()
    {
        if (new Vector2(Input.GetAxis(Axe_X2), Input.GetAxis(Axe_Y2)).magnitude < .7f) return 0;
        if (InvertX2) return -Input.GetAxis(Axe_X2);
        else return Input.GetAxis(Axe_X2);
    }
    public virtual float GetY()
    {

        if (InvertY) return -Input.GetAxis(Axe_Y);
        else return Input.GetAxis(Axe_Y);
    }
    public virtual float GetY2()
    {
        if (new Vector2(Input.GetAxis(Axe_X2), Input.GetAxis(Axe_Y2)).magnitude < .7f) return 0;
        if (InvertY2) return -Input.GetAxis(Axe_Y2);
        else return Input.GetAxis(Axe_Y2);
    }
    public virtual bool GetShoot()
    {
        return (Input.GetKey(Key_Shoot));
    }
    public virtual bool GetJump()
    {
        return (Input.GetKeyDown(Key_Jump));
    }

}
