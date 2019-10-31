using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetwrokedPlayer : NetworkBehaviour
{

    public Transform Bazooka;
    public GameObject Rocket;
    public int NumPlayer;
    [SyncVar]
    public Color Couleur;

    private bool isOnline;
    private Rigidbody rigid;

    private Vector3 DirectionTir = Vector3.up;
    private float timerTir;
    private ProfilControle MesControles;
    private Camera cam;
    float distToGround;



    bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    #region Command

    [Command]
    void Cmd_DoFire(Vector3 Dir, Vector3 Position)
    {
        transform.position = Position;
        GameObject instance = Instantiate(Rocket, Position, Quaternion.Euler(Dir));
        instance.GetComponent<NetworkedRocket>().SetSpeed(Dir, 12.5f, this);
        instance.GetComponent<MeshRenderer>().material.color = Couleur;
        NetworkServer.Spawn(instance);
    }


    #endregion

    // Start is called before the first frame update
    void Start()
    {
        isOnline = FindObjectOfType<NetworkManager>() != null;

        if (isOnline)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Couleur;
            if (!isLocalPlayer)
            {
                Debug.Log(gameObject.name + " is not local player");
                this.enabled = false;
                return;
            }
            else
                Debug.Log(gameObject.name + " is local player");

        }
        else
            Debug.Log("Game is offline");

        cam = GameObject.Find("CameraJeu").GetComponent<Camera>();
        rigid = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
        timerTir = 1;

        MesControles = ProfilControle.GetMyProfil(this);
        if (MesControles is ProfilTactile)
        {
            MesControles = gameObject.AddComponent<ProfilTactile>();
            Cursor.visible = false;
        }

        DebugAndroid.LogSpecific("Player_" + NumPlayer + " spawn", MesControles.ToString());



    }

    [Command]
    void Cmd_MajPositionVelocity(Vector3 Position, Vector3 Velocity)
    {
        transform.position = Position;
        rigid.velocity = Velocity;
    }


    // Update is called once per frame
    void Update()
    {

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
            /*if (MesControles.GetX() > 0)
            {
                if (rigid.velocity.x > 5)
                    acceleration = 0;
                else
                    acceleration = 5 - rigid.velocity.x;
            }
            else
            {
                if (rigid.velocity.x < -5)
                    acceleration = 0;
                else
                    acceleration = -5 - rigid.velocity.x;
            }*/
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
            rigid.velocity += Vector3.up * 10;

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
        Bazooka.localEulerAngles = Vector3.right * Mathf.Atan2(DirectionTir.x, DirectionTir.y) * Mathf.Rad2Deg;

        #endregion
        //////////
        #region Tir
        timerTir += Time.deltaTime;

        if (MesControles.GetShoot() && timerTir >= 1)
        {

            timerTir = 0;

            Cmd_DoFire(DirectionTir, transform.position);

        }
        #endregion
    }
}