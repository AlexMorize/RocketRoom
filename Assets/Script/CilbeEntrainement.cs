using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CilbeEntrainement : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ExplosionPhysique Explosion = other.GetComponent<ExplosionPhysique>();
        if (Explosion)
        {
            Vector3 diff = other.transform.position - transform.position;
            if (diff.magnitude < 1.5f)
            {
                GestionnaireJeuCampagne GestionJeu = FindObjectOfType<GestionnaireJeuCampagne>();
                GestionJeu.GivePoint(Explosion.NumPlayer, 10, transform.position);
                GestionJeu.SpawnCible();
                Destroy(gameObject);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
