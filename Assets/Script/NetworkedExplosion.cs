using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkedExplosion : NetworkBehaviour
{

    public float radius;
    public float puissance;
    public bool showRadiusEdit = true;
    public int NumPlayer { get; set; }

    private Light Lumiere;
    private int PlayerToSpawn;

    void DrawCircle()
    {


        for (int i = 0; i < 360; i++)
        {
            Vector3 Départ, Fin;
            Départ = new Vector3(Mathf.Cos((float)i * Mathf.Deg2Rad), Mathf.Sin((float)i * Mathf.Deg2Rad)) * (radius / 2);
            Fin = new Vector3(Mathf.Cos((float)(i + 1) * Mathf.Deg2Rad), Mathf.Sin((float)(i + 1) * Mathf.Deg2Rad)) * (radius / 2);
            Debug.DrawRay(Départ + transform.position, Fin - Départ, Color.red);

        }
    }

    [Client]
    [Command]
    void Cmd_SendKillOnline(int _Player, int _Tueur)
    {
        FindObjectOfType<NetworkedGameManager>().Kill(_Player, _Tueur);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col == null) Debug.Log("No Collider");
        else Debug.Log("Ok for Collider");

        float proximité;
        Vector3 diff = col.transform.position - transform.position;
        NetwrokedPlayer Player = col.GetComponent<NetwrokedPlayer>();
        if (Player && diff.magnitude < radius / 2)
        {
            NetworkedGameManager OnlineManager = FindObjectOfType<NetworkedGameManager>();
            Cmd_SendKillOnline(Player.NumPlayer, NumPlayer);
            //Destroy(col.gameObject);

            //return;
        }
        if (diff.magnitude < radius)
        {
            proximité = radius - diff.magnitude;
            col.attachedRigidbody.velocity += diff.normalized * proximité / radius * puissance;
        }
    }

    void ResetLight()
    {
        if (Lumiere.intensity <= 0)
        {
            Lumiere.intensity = 0;
            Destroy(Lumiere);
            return;
        }
        else
        {
            Lumiere.intensity -= Time.deltaTime * 10;
            Invoke("ResetLight", 0.01f);
        }
    }

    void Start()
    {
        Lumiere = GetComponentInChildren<Light>();
        Invoke("ResetLight", 0.01f);
        Destroy(this, .3f);
        Destroy(gameObject, 5);
        Destroy(GetComponent<Collider>(), .3f);
        if (!GetComponent<SphereCollider>())
        {
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.radius = radius;
            col.isTrigger = true;
        }
    }


#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        if (showRadiusEdit)
            DrawCircle();
        GetComponent<SphereCollider>().radius = radius;
    }
#endif
}
