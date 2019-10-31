using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : Joueur

{
    // Start is called before the first frame update
    void Start()
    {

        Gestion = FindObjectOfType<GestionnaireDeJeu>();
        OriginalScale = transform.lossyScale.magnitude;
        cam = GameObject.Find("CameraJeu").GetComponent<Camera>();
        rigid = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y + (.25f * transform.localScale.y);
        timerTir = 1;

        /*MesControles = ProfilControle.GetMyProfil(this);
        if (MesControles is ProfilTactile)
        {
            MesControles = gameObject.AddComponent<ProfilTactile>();
            Cursor.visible = false;
        }*/

        MesControles = gameObject.AddComponent<BotController>();

        DebugAndroid.LogSpecific("Player_" + NumPlayer + " spawn", MesControles.ToString());

        Line = GetComponentInChildren<LineRenderer>();
        Invincible = true;
        Invoke("RemoveInvincibilite", 1);
        InvokeRepeating("ClignoteInvincible", 0, .1f);

        Model3D.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        Model3D.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        Model3D.material.SetInt("_ZWrite", 0);
        Model3D.material.DisableKeyword("_ALPHATEST_ON");
        Model3D.material.DisableKeyword("_ALPHABLEND_ON");
        Model3D.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        Model3D.material.renderQueue = 3000;

        foreach (Transform tr in GetComponentsInChildren<Transform>()) tr.gameObject.layer = 11;

        Color transparent = Couleur;
        transparent.a = .5f;

        Model3D.material.color = transparent;

        ParticleSystem SpawnParticle = FindObjectOfType<ParticleSystem>();
        SpawnParticle.startColor = Couleur;
        SpawnParticle.Play();
        /* Gradient grad = new Gradient();
         grad.colorKeys = new GradientColorKey[2] { new GradientColorKey(Color.red, 0), new GradientColorKey(Color.red, 1) };
         grad.alphaKeys = new GradientAlphaKey[3] { new GradientAlphaKey(.75f,0),new GradientAlphaKey(.75f,.5f), new GradientAlphaKey(0, 1) }; ;
         Line.colorGradient = grad;*/


    }
}
