using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets;

public class IAController : ProfilControle
{
    public ReseauNeural Cerveau;
    Joueur player;

    const int NbRoquetteMax = 4, NbEnemyMax = 3;

    private float OriginalTime;
    private bool Tirer, Sauter;
    private Vector2 Direction, Tir;

    public override bool GetJump()
    {
        return Sauter;
    }
    public override bool GetShoot()
    {
        return Tirer;
    }
    public override float GetX()
    {
        return Direction.x;
    }
    public override float GetX2()
    {
        return Tir.x;
    }
    public override float GetY2()
    {
        return Tir.y;
    }

    private void OnDestroy()
    {
        /*GestionIA Gestion = FindObjectOfType<GestionIA>();
        Gestion.AddCerveau(Time.time - OriginalTime, Cerveau);*/
    }

    public float GetSurvivalTime()
    {
        if (OriginalTime == 0) return 0;

        return Time.time - OriginalTime;
    }
    public ReseauNeural GetBrain()
    {
        return Cerveau;
    }

    public void SetReseau(ReseauNeural cerveau)
    {
        //Debug.Log(cerveau);
        Cerveau = cerveau;
    }

    // Start is called before the first frame update
    void Start()
    {
        OriginalTime = Time.time;
        player = GetComponent<Joueur>();
        //Cerveau = ReseauNeural.CreateRandom(13, 5, 3, 10);
    }

    // Update is called once per frame
    void Update()
    {
        /*
         * Les Entrees = 13
         * 
            isGrounded  0
            MyPosition    1-2
            MyDirection (include speed) 3-4
            RocketPosition 5-6
            RocketDirection 7-8
            isRocket 9
            isExplosion 10
            ExplosionPosition 11-12

        LesEntrees[] = 

        v2

NbEntree = 5 + 8 *  NbMaxRoquette + 5 * NbMaxPlayer  = 

isGroundedv  1
MyPosition    2					5
MyDirection (include speed) 2

isRocket 1 * NbMaxRoquette
RocketPosition 2 * NbMaxRoquette     		5 *  NbMaxRoquette
RocketDirection 2 * NbMaxRoquette

isExplosion 1 * NbMaxRoquette
ExplosionPosition 2*  * NbMaxRoquette		3   * NbMaxRoquette     

isPlayer 1 * NbMaxPlayer
PlayerPosition 2 * NbMaxPlayer			5 * NbMaxPlayer
PlayerDirection 2 * NbMaxPlayer
         */

        int NbEntree = 5 + 8 * NbRoquetteMax + 5 * NbEnemyMax;

        float[] LesEntrees = new float[NbEntree];

        if (player.isGrounded()) LesEntrees[0] = 1;
        else LesEntrees[0] = 0;

        LesEntrees[1] = player.transform.position.x;
        LesEntrees[2] = player.transform.position.y;
        LesEntrees[3] = player.rigid.velocity.x;
        LesEntrees[4] = player.rigid.velocity.y;


        Rocket[] rockets = FindObjectsOfType<Rocket>();

        for (int i = 0; i < rockets.Length; i++)
        {
            if (i >= NbRoquetteMax) break;
            int RelativePosition = 4 + i * 5;
            Rocket rocket = rockets[i];
            if (rocket)
            {
                
                LesEntrees[RelativePosition +1] = 1;
                LesEntrees[RelativePosition +2] = rocket.transform.position.x;
                LesEntrees[RelativePosition +3] = rocket.transform.position.y;
                LesEntrees[RelativePosition +4] = rocket.rigid.velocity.x;
                LesEntrees[RelativePosition +5] = rocket.rigid.velocity.y;
            }
            else
            {
                LesEntrees[RelativePosition + 1] = 0;
            }
        }

        ExplosionPhysique[] explosionPhysiques = FindObjectsOfType<ExplosionPhysique>();

        for (int i = 0; i < explosionPhysiques.Length; i++)
        {
            if (i >= NbRoquetteMax) break;
            int RelativePosition = 4 + 5 * NbRoquetteMax + i * 3;
            ExplosionPhysique Explosion = explosionPhysiques[i];
            if (Explosion)
            {
                LesEntrees[RelativePosition + 1] = 1;
                LesEntrees[RelativePosition + 2] = Explosion.transform.position.x;
                LesEntrees[RelativePosition + 3] = Explosion.transform.position.y;
            }
            else
            {
                LesEntrees[RelativePosition + 1] = 0;
            }
        }

        Joueur[] lesJoueurs = FindObjectsOfType<Joueur>();

        int Decay = 0;
        for(int i=0;i<lesJoueurs.Length;i++)
        {
            if (i + Decay >= NbEnemyMax) break;
            int RelativePosition = 4 + 8 * NbRoquetteMax + (i+Decay) * 3;
            Joueur unJoueur = lesJoueurs[i];

            if(unJoueur==this)
            {
                Decay = -1;
            }else
            {
                if(unJoueur && unJoueur.rigid)
                {
                    LesEntrees[RelativePosition + 1] = 1;
                    LesEntrees[RelativePosition + 2] = unJoueur.transform.position.x;
                    LesEntrees[RelativePosition + 3] = unJoueur.transform.position.y;
                    
                    LesEntrees[RelativePosition + 4] = unJoueur.rigid.velocity.x;
                    LesEntrees[RelativePosition + 4] = unJoueur.rigid.velocity.y;
                }
            }
        }
        /*
        Sortie = 5

        Jump → 0
        Direction → 1
        Direction tir  2-3
        Tir 4

         */

        float[] Sorties = Cerveau.Calcul(LesEntrees);

        Sauter = Sorties[0] > .5f;
        Direction = new Vector2(Sorties[1], 0).normalized;
        Tir = new Vector2(Sorties[2], Sorties[3]).normalized;
        Tirer = Sorties[4] > .5f;



    }
}
