using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public GameObject Explosion;

    public Rigidbody rigid { get; private set; }
    private float vitesse = 12.5f, multiplieur, OriginalTime;

    int NumPlayer;
    bool alreadyDestroy = false;




    /// <summary>
    /// Retourne vrai si il y a un point de danger pour le joueur par rapport à cette roquette
    /// La sortie PointDeDanger indique la position du point de danger
    /// </summary>
    /// <param name="positionJoueur">Position du joueur voulant connaitre le point de danger</param>
    /// <param name="PointDeDanger">Sortie retournant le point de danger si il y a un danger</param>
    /// <returns></returns>
    public Vector3 GetPointDeDanger(Vector3 positionJoueur)
    {
        Vector3 PointDeDanger;
        if (!rigid) throw new System.Exception("Aucun RigidBody n'est attaché à la roquette");
        else
        {
            if (rigid.angularVelocity.z == 0)//Si la roquette à une direction linéaire
            {
                Vector3 RelativePostionRocket = transform.position - positionJoueur;//Direction de la roquette par rapport au joueur
                Vector3 DirectionRoquette = transform.up;
                Vector3 Départ;
                Vector3 Arriver;

                GetTrajectoireLinéaire(out Départ, out Arriver);

                if (Vector3.Angle(DirectionRoquette, RelativePostionRocket) > 90)//Si la roquette va dans la direction opposé au joueur
                {

                    return PointDeDanger = Arriver;//Le point de danger est l'impact;
                }
                else
                {
                    

                    if(DirectionRoquette.x == 0) //Si on ne peut pas calculer avec une equation cartésienne (x=0)
                    {
                        return PointDeDanger = new Vector3(transform.position.x, positionJoueur.y);

                    }else
                    {
                        if(DirectionRoquette.y == 0) // Si la roquette va a gauche ou a droite (y=0)
                        {
                            return PointDeDanger = new Vector3(positionJoueur.x, transform.position.y);
                        }
                        else// Si il faut calculer le point de danger avec une equation cartésienne;
                        {
                            Vector3 Perpendiculaire = new Vector3(-DirectionRoquette.y, -DirectionRoquette.x);
                            EquationReduite EquaRoquette = new EquationReduite(transform.position, DirectionRoquette);
                            EquationReduite EquaJoueur = new EquationReduite(positionJoueur, Perpendiculaire);

                            Vector3 PointProcheDroite = EquationReduite.PointEntreDroite(EquaRoquette, EquaJoueur);

                            if(DirectionRoquette.x>0)//Si la roquette va de gauche a droite;
                            {
                                if (PointProcheDroite.x < Départ.x) return PointDeDanger = transform.position;
                                if (PointProcheDroite.x > Arriver.x) return PointDeDanger = Arriver;

                                return PointDeDanger = PointProcheDroite;
                            }
                            else//Si la roquette va de droite a gauche
                            {
                                if (PointProcheDroite.x > Départ.x) return PointDeDanger = transform.position;
                                if (PointProcheDroite.x < Arriver.x) return PointDeDanger = Arriver;

                                return PointDeDanger = PointProcheDroite;
                            }

                        }
                    }
                }
            }
            else//Si la roquette à une direction circulaire
            {
                float rayon;
                Vector3 CentreCercle;
                GetTrajectoireCercle(out rayon, out CentreCercle);
                if (Vector3.Distance(positionJoueur, CentreCercle) < rayon)
                {
                    return PointDeDanger = transform.position;
                    
                }
                else
                {

                    float DistanceAuCentre = Vector3.Distance(positionJoueur, CentreCercle);
                    float DistanceAuCercle = DistanceAuCentre - rayon;

                    Vector3 DirectionCercle = (CentreCercle - positionJoueur).normalized;

                    return PointDeDanger = positionJoueur + DirectionCercle * DistanceAuCercle;
                }
            }
        }
    }


    public void GetTrajectoireCercle(out float Rayon, out Vector3 CentreCercle)
    {
        if (!rigid)
        {
            throw new System.Exception("Aucun RigidBody n'est attaché à la roquette");
        }
        if (rigid.angularVelocity.z != 0)
        {
            // V = vitesse objet (m/s), W = vitesse angulaire (rad/sec), R= Rayon (m)
            //W  * R = V
            //R = W/V


            float W = rigid.angularVelocity.z;
            float V = rigid.velocity.magnitude;

            Rayon = V / W;

            Gizmos.color = Color.black;

            CentreCercle = transform.position + -transform.right * Rayon;
        }
        else
        {
            throw new System.Exception("Le rigidbody de la roquette n'as pas de vitesse angulaire");
        }
    }


    public void GetTrajectoireLinéaire(out Vector3 Départ, out Vector3 Arriver)
    {
        Départ = transform.position;

        RaycastHit impact;
        if (Physics.Raycast(Départ, transform.up, out impact))
        {
            Arriver = impact.point;
        }
        else
            throw new System.Exception("La roquette n'as pas de point d'impact, elle est probablement sortie du terrain");

    }

    /*void DrawTrajectoire()
    {
        if (!rigid) {
            Debug.Log("Pas de rigidbody");
            return; }
        if(rigid.angularVelocity.z != 0)
        {
            Debug.Log("Angular Velocity = " + rigid.angularVelocity.z);
            float W = rigid.angularVelocity.z;
            float V = rigid.velocity.magnitude;

            float Rayon = V / W;

            Gizmos.color = Color.black;

            Vector3 CentreCercle = transform.position + -transform.right * Rayon;
            Debug.Log(CentreCercle);

            DrawCircle(Rayon, CentreCercle);

        }else
        {
            Debug.Log("Le rigidbody n'as pas de vitesse angulaire");
        }
    }


    void DrawCircle(float radius, Vector3 CentreCercle)
    {


        for (int i = 0; i < 360; i++)
        {
            Vector3 Départ, Fin;
            Départ = new Vector3(Mathf.Cos((float)i * Mathf.Deg2Rad), Mathf.Sin((float)i * Mathf.Deg2Rad)) * (radius);
            Fin = new Vector3(Mathf.Cos((float)(i + 1) * Mathf.Deg2Rad), Mathf.Sin((float)(i + 1) * Mathf.Deg2Rad)) * (radius);
            Debug.DrawRay(Départ + CentreCercle, Fin - Départ, Color.red);

        }
    }*/

    public void SetSpeed(Vector3 Direction, float Vitesse, Joueur Player)
    {
        rigid = GetComponent<Rigidbody>();
        transform.eulerAngles = Vector3.forward * Mathf.Atan2(-Direction.x, Direction.y) * Mathf.Rad2Deg;
        rigid.velocity = Direction * Vitesse;
        vitesse = Vitesse;

        NumPlayer = Player.NumPlayer;
        multiplieur = transform.localScale.y / 10;
        GetComponentInChildren<MeshRenderer>().material.color = Player.Couleur;
    }


    void Explose()
    {
        if (alreadyDestroy) return;
        ExplosionPhysique instance = Instantiate(Explosion, transform.position, new Quaternion()).GetComponent<ExplosionPhysique>();
        instance.NumPlayer = NumPlayer;
        foreach (var SmokeEffect in GetComponentsInChildren<ParticleSystem>())
        {

            SmokeEffect.transform.parent = null;
            //SmokeEffect.transform.localScale = SmokeEffect.transform.localScale / transform.localScale.magnitude;
            Destroy(SmokeEffect.gameObject, SmokeEffect.startLifetime);
            SmokeEffect.Stop();
        }
        Destroy(gameObject);
        alreadyDestroy = true;
    }
    protected void OnCollisionStay(Collision collision)
    {

        if (collision.GetContact(0).separation < -.3f && !collision.gameObject.GetComponent<Rocket>()) Explose();
    }
    private void OnCollisionEnter(Collision collision)
    {
        Joueur player = collision.transform.GetComponent<Joueur>();
        if (player && player.NumPlayer == NumPlayer)
        {
            if (Time.time - OriginalTime < .5f)
            {
                return;
            }
        }

        if (Vector3.Angle(transform.up, collision.GetContact(0).point - transform.position) < 90)
        {
            Explose();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        Joueur player = collision.transform.GetComponentInParent<Joueur>();
        if (player && player.NumPlayer == NumPlayer)
        {
            if (Time.time - OriginalTime < .5f)
            {
                return;
            }
        }
        if (player)
            Explose();

    }


    private void OnTriggerExit(Collider other)
    {
        GetComponent<Collider>().isTrigger = false;

    }


    // Start is called before the first frame update
    void Awake()
    {
        Destroy(gameObject, 10);
        rigid = GetComponent<Rigidbody>();
        OriginalTime = Time.time;
    }

    // Update is called once per frame
    void Update()

    {
        GetComponent<Rigidbody>().velocity = transform.up * vitesse;
        RaycastHit info;
        Physics.Raycast(new Ray(transform.position, GetComponent<Rigidbody>().velocity * Time.deltaTime), out info);
        if (info.distance < vitesse * Time.deltaTime /** multiplieur/* && info.collider.gameObject != gameObject && info.collider.gameObject != player.gameObject*/)
        {
            if (info.transform)
            {
                Joueur player = info.transform.GetComponent<Joueur>();
                if (player && player.NumPlayer == NumPlayer)
                {
                    if (Time.time - OriginalTime < .5f)
                    {
                        return;
                    }
                }
                Explose();
            }
        }

    }
}

public class EquationCartesienne
{
    public float a, b, c;


    public override string ToString()
    {
        return a + "x + " + b + "y + " + c; 
    }
    /// <summary>
    /// Retourne l'equation normaliser, avec a = 1;
    /// </summary>
    public EquationCartesienne normalized
    {
        get
        {
            return new EquationCartesienne(1, b / a, c/a);
        }
    }

    public EquationCartesienne(float a, float b, float c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }

    /// <summary>
    /// Creer une equation cartésienne à partir d'un point et d'un vecteur
    /// </summary>
    /// <param name="Position">Point de la droite</param>
    /// <param name="Direction">Vecteur de direction</param>
    /// <returns></returns>
    public EquationCartesienne(Vector3 Position, Vector3 Direction)
    {
        /* Obtenir ordonné à l'origine sachant une direction (Vector(x,y))
         et une position P(x, y)
         Rappel:
         a = Vector.y
         b = -Vector.x
         c = -(a*P.x + b*P.y)
         */
        if (Direction.x == 0) throw new System.Exception("La propriété x du vecteur de direction ne peut etre égal à 0");
        a = Direction.y;
        b = -Direction.x;
        c = -(a * Position.x + b * Position.y);

    }

    /// <summary>
    /// Transforme l'equation tel que a = 1;
    /// </summary>
    public void Normalize()
    {
        b = b / a;
        c = c / a;
        a = 1;
        
    }

    public EquationCartesienne(EquationReduite Reduite)
    {
        a = -Reduite.a;
        b = 1;
        c = -Reduite.b;
    }
}

public class EquationReduite
{
    public float a, b;

    public static Vector3 PointEntreDroite(EquationReduite EquationA,EquationReduite EquationB)
    {
        float XWeight = EquationA.a + EquationB.a;
        float ValueWeighted = EquationA.b + EquationB.b;
        float XValue = ValueWeighted / XWeight;
        float yValue = EquationA.CalculY(XValue);

        return new Vector3(XValue, yValue);
    }

    public float CalculY(float X)
    {
        return a * X + b;
    }


    public EquationReduite(EquationCartesienne cartesienne)
    {
        a = -cartesienne.a / cartesienne.b;
        b = -cartesienne.c / cartesienne.b;
    }

    public EquationReduite(Vector3 Point, Vector3 Direction)
    {
        EquationCartesienne cartesienne = new EquationCartesienne(Point, Direction);
        a = -cartesienne.a / cartesienne.b;
        b = -cartesienne.c / cartesienne.b;
    }
}


