using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollCamera : MonoBehaviour
{
    Joueur Player;

    Vector3 Pos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Player==null)
        {
            Player = FindObjectOfType<Joueur>();
            if (!Player) return;
            Pos = Player.transform.position;
            transform.position = Pos - Vector3.forward * 10 + Vector3.up * 2;
        }
        else
        {

            
            if (Vector3.Distance(Player.transform.position, Pos) > 1) Pos = Player.transform.position - (Player.transform.position - Pos).normalized;

            Vector3 PosFixed = Player.transform.position - Pos;
            
            PosFixed.y = 0;
            
            

            transform.position = Player.transform.position + PosFixed - Vector3.forward * 10 + Vector3.up * 2;
        }
    }
}
