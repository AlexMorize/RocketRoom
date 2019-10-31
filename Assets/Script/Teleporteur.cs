using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporteur : MonoBehaviour
{
    public Teleporteur Sortie;
    public AudioClip Son;

    AudioSource Audio = null;
    private MeshRenderer renderer;
    private float lightIntensity;

    List<Transform> LesObjets = new List<Transform>();


    public void TeleportHere(Transform Object, float velocity)
    {
        LesObjets.Add(Object);
        Invoke("RemoveObjectFromList", .2f);
        Object.transform.position = transform.position + transform.up * 1.2f;
        Rigidbody rigid = Object.GetComponent<Rigidbody>();
        if (velocity < 2f)
            velocity = 2f;
        rigid.velocity = transform.up * velocity;
        Audio.pitch = Random.Range(0.8f, 1.2f);
        Audio.Play();
        LightOn(5);

    }

    void RemoveObjectFromList()
    {
        LesObjets.Remove(LesObjets[0]);
    }

    void LightOn(float intensity)
    {
        SetEmission(new Color(0, 0, 1), 2);
        CancelInvoke("ResetLight");
        Invoke("ResetLight", 0.01f);
        lightIntensity = intensity;
    }

    void SetEmission(Color color, float Intensity)
    {
        Material[] materials = renderer.materials;
        materials[1].SetColor("_EmissionColor", color * Intensity);

        renderer.materials = materials;
    }


    private void OnCollisionEnter(Collision collision)
    {
        Joueur TpPlayer = collision.transform.GetComponent<Joueur>();
        if (collision.transform.GetComponent<Joueur>() || collision.gameObject.layer == 12)
        {
            if (LesObjets.Contains(collision.transform)) return;
            LightOn(5);
            Sortie.TeleportHere(collision.transform,collision.relativeVelocity.magnitude);
        }
    }


    void Start()
    {
        renderer = GetComponentInChildren<MeshRenderer>();
        if(Son!=null)
        {
            Audio = gameObject.AddComponent<AudioSource>();
            Audio.clip = Son;

        }
    }
    void ResetLight()
    {
        lightIntensity -= Time.deltaTime * 2;
        if (lightIntensity <= .5f)
        {
            SetEmission(new Color(0, 0, 1), .5f);
            return;
        }
        else
        {
            SetEmission(new Color(0, 0, 1), lightIntensity);
            Invoke("ResetLight", 0.01f);
        }
    }

}
