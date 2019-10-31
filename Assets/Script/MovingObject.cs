using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public Vector3 Translation;
    public float tempsAllerSimple, tempsDePause;

    Vector3 LastPosition;

    public Vector3 Move { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        LastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Move = transform.position - LastPosition;
        LastPosition = transform.position;
    }
}