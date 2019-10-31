using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IALearning : Joueur
{

    public IAController monCerveau;

    private TextMesh AfficheScore;

    bool aTirer = false;

    public void SetCerveau(ReseauNeural cerveau)
    {
        
        MesControles = monCerveau = gameObject.AddComponent<IAController>();
        monCerveau.SetReseau(cerveau);
    }

    public override void Kill()
    {
        GetComponent<IAController>().enabled = false;
        base.Kill();
    }



    void Start()
    {
        AfficheScore = GetComponentInChildren<TextMesh>();
        AfficheScore.color = Couleur;
        Gestion = FindObjectOfType<GestionnaireDeJeu>();
        OriginalScale = transform.lossyScale.magnitude;
        cam = GameObject.Find("CameraJeu").GetComponent<Camera>();
        rigid = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y + (.25f * transform.localScale.y);
        timerTir = 1;


        
        if (MesControles is ProfilTactile)
        {
            MesControles = gameObject.AddComponent<ProfilTactile>();
            Cursor.visible = false;
        }

        DebugAndroid.LogSpecific("Player_" + NumPlayer + " spawn", MesControles.ToString());

        Line = GetComponentInChildren<LineRenderer>();
        Invincible = true;
        Invoke("RemoveInvincibilite", 2);
        Invoke("ClignoteInvincible", .05f);
        Model3D.material.color = Couleur;
        Invoke("KillInactif", 5);


    }


    #region OldUpdate
    // Update is called once per frame
    void KillInactif()
    {
        if(!aTirer)
        {
            Gestion.Kill(this,NumPlayer);
        }
        else
        {
            Invoke("KillInactif", 5);
            aTirer = false;
        }
    }

    void Update()
    {
        AfficheScore.text = Mathf.RoundToInt(monCerveau.GetSurvivalTime()) +"s - "+ nbTué;
        if (transform.position.y < -5) FindObjectOfType<GestionIA>().Kill(this, NumPlayer);
        SyncMove = true;
        #region Déplacement

        if (MesControles.GetX() != 0)
        {
            DebugAndroid.LogSpecific("Player_" + NumPlayer + " move", "is moving, speed : " + MesControles.GetX());
            float acceleration = 5;
            if (MesControles.GetX() > 0 == rigid.velocity.x > 0)
            {
                acceleration = (Mathf.Abs(MesControles.GetX() * 5) - Mathf.Abs(rigid.velocity.x));
                if (acceleration < 0) acceleration = 0;
            }

            acceleration = MesControles.GetX() * acceleration;

            if (isGrounded())
                rigid.velocity += Vector3.right * acceleration * Time.deltaTime * 5;
            else
                rigid.velocity += Vector3.right * acceleration * Time.deltaTime * 3;
        }
        else
        {

            DebugAndroid.LogSpecific("Player_" + NumPlayer + " move", "is stopped");
            if (isGrounded())
            {
                rigid.velocity -= Vector3.right * rigid.velocity.x * 10 * Time.deltaTime;
            }

        }


        #endregion
        //////////
        #region Saut

        if (MesControles.GetJump() && isGrounded())
            rigid.velocity += Vector3.up * 7.5f;

        #endregion
        //////////
        #region Orientation Bazooka

        if (MesControles.NomProfil == "Clavier/Souris")
        {
            Vector3 point = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -cam.transform.localPosition.z));
            DirectionTir = point - transform.position;

        }
        else if (MesControles is ProfilTactile)
        {
            ProfilTactile MyCont = (ProfilTactile)MesControles;
            Vector2 position = MyCont.ShotPosition;
            Vector3 point = cam.ScreenToWorldPoint(new Vector3(position.x, position.y, -cam.transform.localPosition.z));
            DirectionTir = point - transform.position;
        }
        else
        {
            DirectionTir += (MesControles.GetX2() * Vector3.right + MesControles.GetY2() * Vector3.up) * 2000 * Time.deltaTime;
        }
        DirectionTir.Normalize();


        //Animation
        if (isGrounded())
        {
            float speed = DirectionTir.x * rigid.velocity.x;
            anim.SetBool("Flying", false);
            if (Mathf.Abs(speed) < .1f)
            {
                anim.SetBool("Stop", true);
                anim.speed = 1;
            }
            else
            {

                anim.SetBool("Stop", false);
                anim.SetBool("MarcheDroite", speed > 0);
                anim.speed = Mathf.Abs(rigid.velocity.x);
            }

        }
        else
        {
            anim.SetBool("Flying", true);
            anim.speed = 1;
        }



        Line.SetPosition(1, new Vector3(0, DirectionTir.y, DirectionTir.x));
        Line.SetPosition(2, new Vector3(0, DirectionTir.y * 1.15f, DirectionTir.x * 1.15f));
        body.localEulerAngles = new Vector3(0, -90 * DirectionTir.x + 90, 0);

        Spine1.localEulerAngles = Spine2.localEulerAngles = new Vector3(-25 * DirectionTir.y + 15, 0, 0);

        #endregion
        //////////
        #region Tir
        timerTir += Time.deltaTime;

        if (MesControles.GetShoot() && timerTir >= 1)
        {
            aTirer = true;
            timerTir = 0;
            GameObject instance = Instantiate(Rocket, new Vector3(Bazooka.position.x, Bazooka.position.y, transform.position.z), Quaternion.Euler(DirectionTir));
            instance.GetComponent<Rocket>().SetSpeed(DirectionTir, 15f, this);
        }
        #endregion
    }
    #endregion
}
