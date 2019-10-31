using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfilTactile : ProfilControle
{
    Touch MoveTouche = new Touch(), FireTouch;
    Vector2 OldPosition, Move;
    public Vector2 ShotPosition { get; private set; }
    bool Fire, Jump, EndOfMove = true;

    private GestionnaireDeJeu GestionJeu;
    public static float Sensibilité = 100;
    float deathZone = 5;

    public override float GetX()
    {

        return Move.x / Sensibilité;
    }
    public override float GetY()
    {
        return Move.y;
    }
    public override bool GetShoot()
    {
        return Fire;
    }
    public override bool GetJump()
    {
        return Move.y > Sensibilité/2;
        //return MoveTouche.deltaPosition.y > Sensibilité;
    }

    // Start is called before the first frame update
    void Start()
    {
        Move = new Vector2();
        MoveTouche.phase = TouchPhase.Ended;
        MoveTouche.fingerId = -1;
        FireTouch.phase = TouchPhase.Ended;
        deathZone = Sensibilité/8;
        GestionJeu = FindObjectOfType<GestionnaireDeJeu>();
        GestionJeu.JoystickVirtuel.rectTransform.localScale = Vector2.one * Sensibilité * 2 / 100;
        GestionJeu.JoystickVirtuel.enabled = false;
        GestionJeu.JoystickBackground.rectTransform.localScale = Vector2.one * Sensibilité * 2 / 100;
        GestionJeu.JoystickBackground.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        Fire = false;
        Jump = false;
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch current = Input.GetTouch(i);

            // Changer le doigt de mouvement
            bool AssignNewMoveTouch = current.phase == TouchPhase.Moved && MoveTouche.phase == TouchPhase.Ended && current.deltaPosition.magnitude>deathZone;
            if (AssignNewMoveTouch)
            {
                Debug.Log("New Move touhch");
                MoveTouche = current;
                OldPosition = current.position;
                GestionJeu.JoystickVirtuel.enabled = true;
                GestionJeu.JoystickBackground.enabled = true;
            }

            //Appliquer le mouvement
            if (current.fingerId == MoveTouche.fingerId && current.phase == TouchPhase.Moved)
            {
                Move = (current.position - OldPosition);
                if (Move.magnitude > Sensibilité)
                {
                    OldPosition += Move.normalized * (Move.magnitude - Sensibilité);
                    Move = Move.normalized * Sensibilité;
                }
                GestionJeu.JoystickVirtuel.rectTransform.position = current.position;
                GestionJeu.JoystickBackground.rectTransform.position = OldPosition;

            }

            if (current.phase == TouchPhase.Ended)
            {
                if (current.fingerId != MoveTouche.fingerId)
                {
                    Fire = true;
                    ShotPosition = current.position;
                }
                else
                {
                    GestionJeu.JoystickVirtuel.enabled = false;
                    GestionJeu.JoystickBackground.enabled = false;
                    EndOfMove = true;
                    MoveTouche.fingerId = -1;
                    MoveTouche.phase = TouchPhase.Ended;
                    Move = Vector2.zero;
                }
                
            }

            

        }
        

    }
}
