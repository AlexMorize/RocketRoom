using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LigthManager : MonoBehaviour
{
    public ParticleSystem Nuages, Pluie;
    [Range(10, 70)]
    public float angle;
    [Range(0, 1)]
    public float Météo;
    public bool randomize = true;
    public GameObject Projecteurs;
    public Light LumiereNuit;

    // Start is called before the first frame update
    void Awake()
    {
        SetMeteo();
    }

    public void SetMeteo()
    {
        Light Lumiere = GetComponentInChildren<Light>();
        Lumiere.intensity = Météo / 2 + .5f;
        Lumiere.transform.eulerAngles = new Vector3(angle, 150);
        RenderSettings.skybox.SetFloat("_Exposure", angle / 70 * (Météo / 2 + .5f));
        float GrayLevel = (angle / 70 * .7f + .3f) * (Météo / 2 + .5f);
        Projecteurs.SetActive(GrayLevel < .3f);
        if (false)
        {
            LumiereNuit.shadows = LightShadows.Hard;
        }
        else
            LumiereNuit.shadows = LightShadows.None;

        RenderSettings.fogColor = new Color(GrayLevel, GrayLevel, GrayLevel);
        Nuages.startColor = new Color(GrayLevel, GrayLevel, GrayLevel, .7f);
        Nuages.emissionRate = (2 - Météo);
        Nuages.Play();
        if (Météo < .5f)
        {
            Pluie.emissionRate = (1 - Météo) * 10 * (int)QualitySettings.currentLevel + 10;
            Pluie.Play();
        }
    }

    public void Randomize()
    {
        angle = Random.Range(10, 70);
        Météo = Random.Range(0f, 1f);
        SetMeteo();
    }

    // Update is called once per frame
    /* void Update()
     {
         transform.eulerAngles += Vector3.right * Time.deltaTime;
     }*/
}

/*[CustomEditor(typeof(LigthManager))]
public class LightManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        LigthManager myScript = (LigthManager)target;
        if(GUILayout.Button("Update weather"))
        {
            myScript.SetMeteo();
        }
    }
}*/
