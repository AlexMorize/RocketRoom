using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlateform : MonoBehaviour
{
    public Vector3 Translation;
    public float tempsAllerSimple, tempsDePause, tempsDeDecalage;

    Vector3 sizeOfObject;
    Vector3 OriginalPosition;


    

    private void OnDrawGizmosSelected()
    {
        sizeOfObject = GetComponent<Collider>().bounds.extents *2;
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + Translation);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + Translation, sizeOfObject);


    }

    // Start is called before the first frame update
    void Start()
    {
        OriginalPosition = transform.position;
        gameObject.AddComponent<MovingObject>();
    }

    // Update is called once per frame
    void Update()
    {
        float relativeTime = (Time.time+tempsDeDecalage) % (tempsAllerSimple*2+tempsDePause*2);
        if (relativeTime < tempsAllerSimple + tempsDePause)
        {
            if (relativeTime < tempsAllerSimple)
                transform.position = OriginalPosition + Translation * (relativeTime / tempsAllerSimple);
            else
                transform.position = OriginalPosition + Translation;
        }
        else
        {
            if (relativeTime < tempsAllerSimple * 2 + tempsDePause)
                transform.position = OriginalPosition + Translation * (1 - (relativeTime-tempsDePause-tempsAllerSimple)/tempsAllerSimple);
            else
                transform.position = OriginalPosition;
        }

    }
}
