using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Jumper : MonoBehaviour
{

    public float HauteurSaut;
    public AudioClip Son;

    private MeshRenderer renderer;
    private Light Lumiere;
    private AudioSource SourceAudio;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 9 && collision.gameObject.layer != 12 && !collision.gameObject.GetComponent<Joueur>()) return; // Layer 9 = Player, Layer 12 = objet qui saute sur les jumpers
        collision.rigidbody.velocity -= Vector3.Scale(transform.up, collision.rigidbody.velocity);
        collision.rigidbody.velocity += transform.up * Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * HauteurSaut);
        Lumiere.intensity = 5;
        SetEmission(Color.white,5);
        CancelInvoke("ResetLight");
        Invoke("ResetLight", 0.01f);
        SourceAudio.pitch = Random.Range(0.95f, 1.05f);
        SourceAudio.Play();
    }

    void SetEmission(Color color, float Intensity)
    {
        Material[] materials = renderer.materials;
        materials[1].SetColor("_EmissionColor", color * Intensity);

        renderer.materials = materials;
    }

    private void Start()
    {
        renderer = GetComponentInChildren<MeshRenderer>();
        Lumiere = GetComponentInChildren<Light>();
        SourceAudio = GetComponent<AudioSource>();
        if(SourceAudio == null)
            SourceAudio = gameObject.AddComponent<AudioSource>();
        SourceAudio.loop = false;
        SourceAudio.playOnAwake = false;
        SourceAudio.clip = Son;
    }

    void ResetLight()
    {
        if(Lumiere.intensity<=1)
        {
            Lumiere.intensity = 1;
            SetEmission(Color.white, 1);
            return;
        }
        else
        {
            Lumiere.intensity -= Time.deltaTime * 4;
            SetEmission(Color.white, Lumiere.intensity);
            Invoke("ResetLight", 0.01f);
        }
    }

    // Update is called once per frame
#if UNITY_EDITOR
    void Update()
    {


    }
#endif
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1,1,1,.5f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.up * (HauteurSaut / 2/ transform.lossyScale.y), new Vector3(1, HauteurSaut/transform.lossyScale.y, 1));
        /*//Gizmos.DrawWireCube(transform.position + transform.up * HauteurSaut/2, new Vector3(transform.lossyScale.x, HauteurSaut, transform.lossyScale.z));

        Gizmos.DrawLine(transform.position, transform.position + transform.up * HauteurSaut);
        //Gizmos.DrawLine(transform.position + transform.up * HauteurSaut, (-transform.up + transform.right) / 2);
        //Gizmos.DrawLine(transform.position + transform.up * HauteurSaut, (-transform.up - transform.right) / 2);*/
    }


}
