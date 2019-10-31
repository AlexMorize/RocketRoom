using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeControle
{
    ClavierSouris,
    XboxManette,
    ManetteChelou
}

public class Joueur : MonoBehaviour
{

    public Transform Bazooka, Spine1, Spine2, body;
    public GameObject Rocket;
    public int NumPlayer;
    public Color Couleur;
    public Animator anim;
    public SkinnedMeshRenderer Model3D;
    public RuntimeAnimatorController Dance, Death;
    public int nbTué = 0;


    public bool isDestroy { get; protected set; } = false;
    protected GestionnaireDeJeu Gestion;
    public bool Invincible { get; protected set; }
    public Rigidbody rigid { get; protected set; }
    protected Vector3 DirectionTir = Vector3.left;
    protected float timerTir;
    protected ProfilControle MesControles;
    protected Camera cam;
    protected LineRenderer Line;
    protected float OriginalScale;
    protected float distToGround;
    protected bool IsVisible;
    protected bool canJump = true;
    protected bool SyncMove = false;

    public float GetSurvivalTime()
    {
        return GetComponent<IAController>().GetSurvivalTime();
    }

    public ReseauNeural GetBrain()
    {
        return GetComponent<IAController>().GetBrain();
    }

    public bool isGrounded()
    {
        


        LayerMask filtre = new LayerMask();
        filtre = ~((1 << 9) | (1 << 11));


        RaycastHit hit;
        bool SurLeSol = Physics.Raycast(transform.position - Vector3.up * .1f, -Vector3.up, out hit, distToGround, filtre);

        if (SurLeSol && SyncMove)
        {
            SyncMove = false;
            MovingObject Move = hit.transform.gameObject.GetComponent<MovingObject>();

            if (Move)
            {
                transform.position += Move.Move;
            }
        }
        //transform.SetParent(hit.transform);
        /*if(transform.lossyScale.magnitude != OriginalScale)
        {
            if (transform.parent != null)
            {
                Vector3 parent = transform.parent.localScale;
                transform.localScale = new Vector3(1.5f/parent.x,1.5f/parent.y,1.5f/parent.z);
            }else
            {
                transform.localScale = Vector3.one * 1.5f;
            }
        }*/

            return SurLeSol;

    }

    void ResetCanJump()
    {
        canJump = true;
    }

    protected void OnCollisionStay(Collision collision)
    {

        if (collision.GetContact(0).separation < -.3f && !collision.gameObject.GetComponent<Rocket>()) Gestion.Kill(this, NumPlayer);
    }

    public void EndDestroy()
    {
        foreach (Transform tr in GetComponentsInChildren<Transform>()) tr.gameObject.layer = 0;
        GetComponent<Collider>().material = null;
        anim.SetBool("Stop", true);
        anim.speed = 1;
        //anim.SetBool("Dance", true);
        anim.runtimeAnimatorController = Dance;
        body.localEulerAngles = new Vector3(0, 90, 0);
        Bazooka.gameObject.SetActive(false);
        gameObject.AddComponent<SphereCollider>().radius = .5f;
        Destroy(Line.gameObject);
        Destroy(Line.gameObject);
        Destroy(this);


    }

    protected void RemoveInvincibilite()
    {
        Invincible = false;
        foreach (Transform tr in GetComponentsInChildren<Transform>()) tr.gameObject.layer = 9;
        Model3D.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        Model3D.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        Model3D.material.SetInt("_ZWrite", 1);
        Model3D.material.DisableKeyword("_ALPHATEST_ON");
        Model3D.material.DisableKeyword("_ALPHABLEND_ON");
        Model3D.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        Model3D.material.color = Couleur;
        CancelInvoke("ClignoteInvincible");
    }

    protected void ClignoteInvincible()
    {
        IsVisible = !IsVisible;

        Color transparent = Couleur;
        if (IsVisible)
            transparent.a = .5f;
        else
            transparent.a = .2f;
        Model3D.material.color = transparent;
    }

    public virtual void Kill()
    {
        if (isDestroy) return;
        isDestroy = true;
        foreach (var col in GetComponentsInChildren<Collider>()) Destroy(col);
        anim.runtimeAnimatorController = Death;
        Destroy(this);
        Destroy(gameObject, 2);

        Bazooka.GetComponentInChildren<MeshRenderer>().gameObject.AddComponent<MeshCollider>().convex = true;
        Bazooka.transform.parent = null;
        Rigidbody RigidBazooka = Bazooka.gameObject.AddComponent<Rigidbody>();
        //RigidBazooka.constraints = RigidbodyConstraints.FreezePositionZ;
        Bazooka.gameObject.layer = 11;//Ignore rocket
        //rigid.constraints = RigidbodyConstraints.FreezePositionZ|RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezeRotationY;
        //Model3D.gameObject.AddComponent<MeshCollider>().convex = true ;
        Model3D.gameObject.AddComponent<UpdateColliderSkin>();
        Destroy(Bazooka.gameObject, 2);
        gameObject.layer = 11;//Ignore rocket
        
        if(Line) Destroy(Line.gameObject);
    }



    // Start is called before the first frame update
    void Start()
    {

        Gestion = FindObjectOfType<GestionnaireDeJeu>();
        OriginalScale = transform.lossyScale.magnitude;
        cam = GameObject.Find("CameraJeu").GetComponent<Camera>();
        rigid = GetComponent<Rigidbody>();
        distToGround = GetComponent<Collider>().bounds.extents.y + (.25f * transform.localScale.y);
        timerTir = 1;

        MesControles = ProfilControle.GetMyProfil(this);
        if (MesControles is ProfilTactile)
        {
            MesControles = gameObject.AddComponent<ProfilTactile>();
            Cursor.visible = false;
        }

        DebugAndroid.LogSpecific("Player_" + NumPlayer + " spawn", MesControles.ToString());

        Line = GetComponentInChildren<LineRenderer>();
        Invincible = true;
        Invoke("RemoveInvincibilite", 1);
        InvokeRepeating("ClignoteInvincible", 0,.1f);

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



    // Update is called once per frame
    void Update()
    {
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

        if (MesControles.GetJump() && isGrounded() && canJump)
        {
            rigid.velocity += Vector3.up * 7.5f;
            canJump = false;
            Invoke("ResetCanJump", .2f);
        }

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
        //Bazooka.localEulerAngles = Vector3.right * Mathf.Atan2(DirectionTir.x, DirectionTir.y) * Mathf.Rad2Deg;

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
            timerTir = 0;
            GameObject instance = Instantiate(Rocket, new Vector3(Bazooka.position.x, Bazooka.position.y, transform.position.z), Quaternion.Euler(DirectionTir));
            instance.GetComponent<Rocket>().SetSpeed(DirectionTir, 15f, this);
        }
        #endregion
    }
}

public class UpdateColliderSkin : MonoBehaviour
{
    SkinnedMeshRenderer meshRenderer;
    MeshCollider collider;

    void Start()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        GameObject go = new GameObject("Collider");

        go.transform.parent = transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one * .01f;
        collider = go.AddComponent<MeshCollider>();
        go.layer = 11;
        collider.convex = true;
    }

    void Update()
    {
        Mesh colliderMesh = new Mesh();
        meshRenderer.BakeMesh(colliderMesh);
        collider.sharedMesh = null;
        collider.sharedMesh = colliderMesh;
        collider.transform.localEulerAngles = Vector3.zero;
    }

}
