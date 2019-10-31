using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : ProfilControle
{
    //Constante
    float vitesseRoquette = 15;

    //Parametre réglable
    [Range(1.5f, 3)]
    public float distanceDeSecurite = 2.5f;
    [Range(1.5f, 10)]
    public float distanceSecuriteJoueur = 6;
    [Range(0, 10)]
    public float distanceMaxSpot = 2;
    [Range(0, 1)]
    public float Anticipation = .75f;
    [Range(0, 1)]
    public float TempsDeReaction = 0f;
    [Range(0, 5)]
    public float précision = 5f;


    public bool AllAgainstPlayer = true;
    

    Vector3 spotPosition = new Vector3(0, 0, 0);

    //Parametre calculer
    float directionChoisi;
    Vector3 DirectionTir;
    bool DoJump, DoShot;

    //Variable Interne
    bool warningRocket;
    bool warningIsPlayer;
    float timerReaction = 0;


    //Sortie de controlle
    public override float GetX()
    {
        return directionChoisi;
    }
    public override float GetX2()
    {
        return DirectionTir.x;
    }
    public override float GetY2()
    {
        return DirectionTir.y;
    }
    public override bool GetJump()
    {
        return DoJump;
    }
    public override bool GetShoot()
    {
        return DoShot;
    }


    void CalculDanger()
    {
        Vector3 SommePointDeDanger = new Vector3();
        int nbDanger = 0;
        Rocket[] lesRockets = FindObjectsOfType<Rocket>();

        foreach (Rocket rocket in lesRockets)
        {
            try
            {
                Vector3 PointDeDanger = rocket.GetPointDeDanger(transform.position);

                Vector3 DirectionDange = PointDeDanger - transform.position;

                if (DirectionDange.magnitude < distanceDeSecurite) // Si le danger est suffisament proche
                {
                    SommePointDeDanger += PointDeDanger;
                    nbDanger++;
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        foreach(var platform in FindObjectsOfType<MovingPlateform>())
        {
            Vector3 volume = platform.GetComponent<Collider>().bounds.extents * 2;//Taille de l'objet
            Vector3 MoveDirection = platform.GetComponent<MovingObject>().Move;//Déplacement de l'objet
            Vector3 DirectionPlateform = platform.transform.position - transform.position;//Position relative de l'objet par rapport au joueur

            RaycastHit hit;
            if (Vector3.Angle(MoveDirection, DirectionPlateform) > 90)
            {
                if (Physics.Raycast(transform.position, DirectionPlateform, out hit) && hit.distance < distanceDeSecurite)//Si l'objet peu ecraser le joueur
                {
                    if (Physics.Raycast(platform.transform.position, MoveDirection, out hit))
                    {
                        SommePointDeDanger += hit.transform.position;//On le considère comme point de danger
                        nbDanger++;
                    }
                }
            }
        }

        if (nbDanger > 0)
        {
            Vector3 DirectionDanger = SommePointDeDanger / nbDanger - transform.position;

            if (DirectionDanger.x < 0)// si danger a gauche
                directionChoisi = 1;// on va a droite
            else directionChoisi = -1;//sinon on va a gauche;

            DoJump = (DirectionDanger.y < 0);
            warningRocket = true;
        }
    }

    void CalculTir()
    {
        timerReaction += Time.deltaTime;

        Joueur Cible = null;
        float distanceCible = Mathf.Infinity;
        Joueur[] lesJoueurs = FindObjectsOfType<Joueur>();
        Vector3 PositionCible = new Vector3();
        Vector3 VitesseCible = new Vector3();
        foreach (Joueur player in lesJoueurs)
        {
            bool playerIsCible;
           
            if(AllAgainstPlayer)
            {
                playerIsCible = player.NumPlayer > 0;
            }else
            {
                playerIsCible = player.transform != this.transform;
            }

            if (playerIsCible)
            {

                Vector3 DirectionPlayer = player.transform.position - transform.position;
                float CurrentDistance = DirectionPlayer.magnitude;

                RaycastHit info;

                Physics.Raycast(transform.position, DirectionPlayer, out info, CurrentDistance);

                if (CurrentDistance < distanceCible && CurrentDistance > distanceDeSecurite && info.transform == player.transform && !player.Invincible)
                {
                    distanceCible = CurrentDistance;
                    Cible = player;
                    if(timerReaction>TempsDeReaction)
                        DoShot = true;
                    PositionCible = player.transform.position;
                    VitesseCible = player.rigid.velocity;
                }

                Vector3 DirectionDanger = DirectionPlayer;

                if (DirectionDanger.magnitude < distanceSecuriteJoueur && !warningRocket) // Si le danger est suffisament proche
                {//On entamne une procédure d'esquive
                    if (DirectionDanger.x < 0)// si danger a gauche
                        directionChoisi = 1;// on va a droite
                    else directionChoisi = -1;//sinon on va a gauche;
                    warningIsPlayer = true;
                }
            }

            if (Cible != null)
            {
                float randomity = 5 - précision;

                Vector3 NextPosition = PositionCible + (VitesseCible * distanceCible / 15) * Anticipation + Vector3.up * Random.Range(-randomity,randomity) + Vector3.right * Random.Range(-randomity, randomity);
                DirectionTir = NextPosition - transform.position;
            }else if(lesJoueurs.Length>0 && !warningRocket)
            {
                DoJump = true;
                timerReaction = 0;
            }

        }
    }



    // Update is called once per frame
    void Update()
    {
        DoShot = DoJump = false;
        warningIsPlayer = false;
        warningRocket = false;

        CalculDanger();

        CalculTir();







        if (!warningRocket && !warningIsPlayer)
        {
            Vector3 DirectionSpot = spotPosition - transform.position;

            directionChoisi = 0;
            if (DirectionSpot.x > .5f) directionChoisi = 1;
            if (DirectionSpot.x < -.5f) directionChoisi = -1;
        }

        if (!DoJump && directionChoisi != 0)
        {
            LayerMask filtre = new LayerMask();
            filtre = ~(1 << LayerMask.NameToLayer("Player"));

            DoJump = Physics.Raycast(transform.position, Vector3.right * directionChoisi, 1, filtre);

        }

    }
}
