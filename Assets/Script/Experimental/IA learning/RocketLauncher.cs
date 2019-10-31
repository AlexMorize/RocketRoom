using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : MonoBehaviour
{

    public GameObject Rocket, Cible;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Launch", 3, 3);
    }


    void Launch()
    {
        
        if (FindObjectOfType<Joueur>() == null) return;
        Cible = FindObjectOfType<Joueur>().gameObject;
        Vector3 RandomPosition = new Vector3(Random.Range(-7.5f, 7.5f), Random.Range(5f, 6f),0);
        Vector3 Direction = Cible.transform.position - (RandomPosition+transform.position);
        
        Instantiate(Rocket, RandomPosition + transform.position, Quaternion.Euler(Vector3.forward * Mathf.Atan2(-Direction.x, Direction.y) * Mathf.Rad2Deg));
    }
}
