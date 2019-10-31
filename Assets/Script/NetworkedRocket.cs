using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedRocket : NetworkBehaviour
{
    public GameObject Explosion;

    [SyncVar]
    private Vector3 Direction;
    [SyncVar]
    private Color Couleur;
    [SyncVar]
    private float vitesse;
    [SyncVar]
    private float multiplieur;
    private bool isOnline;
    [SyncVar]
    int NumPlayer;



    [Command]
    void Cmd_Explose(Vector3 position)
    {
        transform.position = position;
        Rpc_DoExplosion(position);
    }


    [ClientRpc]
    void Rpc_DoExplosion(Vector3 position)
    {
        NetworkedExplosion instance = Instantiate(Explosion, position, new Quaternion()).GetComponent<NetworkedExplosion>();
        instance.NumPlayer = NumPlayer;
        NetworkServer.Spawn(instance.gameObject);
        foreach (var SmokeEffect in GetComponentsInChildren<ParticleSystem>())
        {
            SmokeEffect.transform.parent = null;
            //SmokeEffect.transform.localScale = SmokeEffect.transform.localScale / transform.localScale.magnitude;
            Destroy(SmokeEffect.gameObject, SmokeEffect.startLifetime);
            SmokeEffect.Stop();
        }
        Destroy(gameObject);
    }

    void Explose(Vector3 position)
    {
        ExplosionPhysique instance = Instantiate(Explosion, position, new Quaternion()).GetComponent<ExplosionPhysique>();
        instance.NumPlayer = NumPlayer;
        foreach (var SmokeEffect in GetComponentsInChildren<ParticleSystem>())
        {
            SmokeEffect.transform.parent = null;
            //SmokeEffect.transform.localScale = SmokeEffect.transform.localScale / transform.localScale.magnitude;
            Destroy(SmokeEffect.gameObject, SmokeEffect.startLifetime);
            SmokeEffect.Stop();
        }
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {

        /*Debug.DrawRay(collision.GetContact(0).point, Vector3.up, Color.red, 10);
        Debug.DrawRay(collision.GetContact(0).point, Vector3.right, Color.red, 10);
        Debug.DrawRay(transform.position, Vector3.up, Color.green, 10);
        Debug.DrawRay(transform.position, Vector3.right, Color.green, 10);
        Time.timeScale = 0;*/

        if (isOnline)
            Cmd_Explose(transform.position);
        else
            Explose(transform.position);


    }

    private void OnTriggerExit(Collider other)
    {
        GetComponent<Collider>().isTrigger = false;

    }

    public void SetSpeed(Vector3 _Direction, float _Vitesse,NetwrokedPlayer _Player)
    {
        Direction = _Direction;
        vitesse = _Vitesse;
        Couleur = _Player.Couleur;
        NumPlayer = _Player.NumPlayer;

        Rigidbody rigid = GetComponent<Rigidbody>();
        transform.eulerAngles = Vector3.forward * Mathf.Atan2(-Direction.x, Direction.y) * Mathf.Rad2Deg;
        rigid.velocity = Direction * _Vitesse;
        vitesse = _Vitesse;

        NumPlayer = _Player.NumPlayer;
        multiplieur = transform.localScale.y / 10;
        GetComponentInChildren<MeshRenderer>().material.color = _Player.Couleur;
    }

    // Start is called before the first frame update
    void Awake()
    {
        isOnline = FindObjectOfType<NetworkManager>() != null;
        Debug.Log("is online = " + isOnline);

        Rigidbody rigid = GetComponent<Rigidbody>();
        transform.rotation = Quaternion.Euler(Direction);
        rigid.velocity = Direction * vitesse;
        multiplieur = transform.localScale.y / 10;
        GetComponentInChildren<MeshRenderer>().material.color = Couleur;
    }

    // Update is called once per frame
    void Update()

    {
        GetComponent<Rigidbody>().velocity = transform.up * vitesse;
        RaycastHit info;
        Physics.Raycast(new Ray(transform.position, GetComponent<Rigidbody>().velocity * Time.deltaTime), out info);
        if (info.distance < vitesse * Time.deltaTime * multiplieur/* && info.collider.gameObject != gameObject && info.collider.gameObject != player.gameObject*/)
        {
            //DebugAndroid.Log(info.collider.gameObject.name + " VS " + gameObject.name + " & " + player.gameObject.name);
            if (isOnline)
                Cmd_Explose(info.point);
            else
                Explose(info.point);
        }
    }
}


