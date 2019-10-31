using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HouseBuilder : MonoBehaviour
{
    const int EspacementTorche = 3;


    [Range(4, 7)]
    public int HauteurEtage = 4;

    [Range(1, 5)]
    public int NbEtage = 2;
    [Range(5,20)]
    public int LargeurMaison;

    [SerializeField]
    private WallBuilder FloorPrefab, WallPrefab;
    [SerializeField]
    private GameObject Coin, Torche;
    [SerializeField]
    private Jumper Jump;


    private Transform Construction;

    public void Build()
    {
        //Destruction de l'ancienne construction
        Construction = transform.Find("Construction");
        if (Construction != null) DestroyImmediate(Construction.gameObject);
        Construction = new GameObject("Construction").transform;
        Construction.parent = transform;
        Construction.position = transform.position;

        for(int CurrentEtage=0;CurrentEtage<NbEtage;CurrentEtage++)
        {

            Vector3 RefEtage = transform.position + Vector3.up * CurrentEtage * HauteurEtage;
            GameObject Instance;
            WallBuilder InstanceScript;
            //Sol
            if (CurrentEtage > 0)
            {
                //Sol Droit
                Instance = Instantiate(FloorPrefab.gameObject, RefEtage + Vector3.up * .5f - transform.forward * .5f + transform.right * (((LargeurMaison / 2 - 2) / 2f) + 1.5f), new Quaternion());
                Instance.transform.parent = Construction;
                InstanceScript = Instance.GetComponent<WallBuilder>();
                InstanceScript.BuildAutomatic = false;
                InstanceScript.Size = new Vector3((LargeurMaison - 5)/2, 1, 2);
                InstanceScript.Build();

                //Sol Gauche
                Instance = Instantiate(FloorPrefab.gameObject, RefEtage + Vector3.up * .5f - transform.forward * .5f - transform.right * (((LargeurMaison / 2 - 2) / 2f)+1.5f), new Quaternion());
                Instance.transform.parent = Construction;
                InstanceScript = Instance.GetComponent<WallBuilder>();
                InstanceScript.BuildAutomatic = false;
                InstanceScript.Size = new Vector3((LargeurMaison - 5) / 2, 1, 2);
                InstanceScript.Build();

                if(CurrentEtage<NbEtage-1 && LargeurMaison>7)
                {
                    if(CurrentEtage%2==0)
                        Instance = Instantiate(Jump.gameObject, RefEtage + Vector3.up * 1.05f - Vector3.right * (1 + 0.5f * ((LargeurMaison + 1) % 2)), new Quaternion());
                    else
                        Instance = Instantiate(Jump.gameObject, RefEtage + Vector3.up * 1.05f + Vector3.right * (1 + 0.5f * ((LargeurMaison + 1) % 2)), new Quaternion());
                    Jumper jumpScript = Instance.GetComponent<Jumper>();
                    jumpScript.HauteurSaut = HauteurEtage + 1;
                    Instance.transform.parent = Construction;

                    if (CurrentEtage % 2 == 0)
                        Instance = Instantiate(FloorPrefab.gameObject, RefEtage + Vector3.up * .5f - transform.forward * .5f - Vector3.right * (1 + 0.5f * ((LargeurMaison + 1) % 2)), new Quaternion());
                    else
                        Instance = Instantiate(FloorPrefab.gameObject, RefEtage + Vector3.up * .5f - transform.forward * .5f + Vector3.right * (1 + 0.5f * ((LargeurMaison + 1) % 2)), new Quaternion());
                    Instance.transform.parent = Construction;
                    InstanceScript = Instance.GetComponent<WallBuilder>();
                    InstanceScript.BuildAutomatic = false;
                    InstanceScript.Size = new Vector3(1, 1, 2);
                    InstanceScript.Build();
                }
            }
            else
            {
                
                Instance = Instantiate(FloorPrefab.gameObject, RefEtage + Vector3.up * .5f - transform.forward * .5f, new Quaternion());
                Instance.transform.parent = Construction;
                InstanceScript = Instance.GetComponent<WallBuilder>();
                InstanceScript.BuildAutomatic = false;
                InstanceScript.Size = new Vector3(LargeurMaison - 2, 1, 2);
                InstanceScript.Build();

                Instance = Instantiate(Jump.gameObject, RefEtage + Vector3.up * 1.05f, new Quaternion());
                Jumper jumpScript = Instance.GetComponent<Jumper>();
                Jump.HauteurSaut = HauteurEtage + 1;
                Instance.transform.parent = Construction;
            }

            //Mur Fond
            Instance = Instantiate(WallPrefab.gameObject, RefEtage + Vector3.up * (float)HauteurEtage/2f + transform.forward, new Quaternion());
            Instance.transform.parent = Construction;
            InstanceScript = Instance.GetComponent<WallBuilder>();
            InstanceScript.BuildAutomatic = false;
            InstanceScript.Size = new Vector3(LargeurMaison, HauteurEtage, 1);
            InstanceScript.Build();

            //Mur Gauche
            Instance = Instantiate(WallPrefab.gameObject, RefEtage + Vector3.up * (float)HauteurEtage / 2f - transform.forward + Vector3.right * (LargeurMaison/2f -.5f), new Quaternion());
            Instance.transform.parent = Construction;
            InstanceScript = Instance.GetComponent<WallBuilder>();
            InstanceScript.BuildAutomatic = false;
            InstanceScript.Size = new Vector3(1, HauteurEtage, 1);
            InstanceScript.Build();

            //Mur Droite
            Instance = Instantiate(WallPrefab.gameObject, RefEtage + Vector3.up * (float)HauteurEtage / 2f - transform.forward - Vector3.right * (LargeurMaison / 2f -.5f), new Quaternion());
            Instance.transform.parent = Construction;
            InstanceScript = Instance.GetComponent<WallBuilder>();
            InstanceScript.BuildAutomatic = false;
            InstanceScript.Size = new Vector3(1, HauteurEtage, 1);
            InstanceScript.Build();

            //Specialité D'étage
            if(CurrentEtage==0)//RDC
            {
                //Mur gauche
                Instance = Instantiate(WallPrefab.gameObject, RefEtage + Vector3.up * ((float)HauteurEtage / 2f +1.5f)  - Vector3.right * (LargeurMaison / 2f - .5f), new Quaternion());
                Instance.transform.parent = Construction;
                InstanceScript = Instance.GetComponent<WallBuilder>();
                InstanceScript.BuildAutomatic = false;
                InstanceScript.Size = new Vector3(1, HauteurEtage-3, 1);
                InstanceScript.Build();

                //Mur Droit
                Instance = Instantiate(WallPrefab.gameObject, RefEtage + Vector3.up * ((float)HauteurEtage / 2f + 1.5f) + Vector3.right * (LargeurMaison / 2f - .5f), new Quaternion());
                Instance.transform.parent = Construction;
                InstanceScript = Instance.GetComponent<WallBuilder>();
                InstanceScript.BuildAutomatic = false;
                InstanceScript.Size = new Vector3(1, HauteurEtage - 3, 1);
                InstanceScript.Build();

                //Prolongation Sol
                Instance = Instantiate(FloorPrefab.gameObject, RefEtage + Vector3.up * .5f - Vector3.right * (LargeurMaison / 2f - .5f), new Quaternion());
                Instance.transform.parent = Construction;
                InstanceScript = Instance.GetComponent<WallBuilder>();
                InstanceScript.BuildAutomatic = false;
                InstanceScript.Size = new Vector3(1, 1, 1);
                InstanceScript.Build();

                //Prolongation Sol
                Instance = Instantiate(FloorPrefab.gameObject, RefEtage + Vector3.up * .5f + Vector3.right * (LargeurMaison / 2f - .5f), new Quaternion());
                Instance.transform.parent = Construction;
                InstanceScript = Instance.GetComponent<WallBuilder>();
                InstanceScript.BuildAutomatic = false;
                InstanceScript.Size = new Vector3(1, 1, 1);
                InstanceScript.Build();
            }
            else
            {
                //Mur gauche haut
                Instance = Instantiate(WallPrefab.gameObject, RefEtage + Vector3.up * ((float)HauteurEtage / 2f + 2f) - Vector3.right * (LargeurMaison / 2f - .5f), new Quaternion());
                Instance.transform.parent = Construction;
                InstanceScript = Instance.GetComponent<WallBuilder>();
                InstanceScript.BuildAutomatic = false;
                InstanceScript.Size = new Vector3(1, HauteurEtage - 4, 1);
                InstanceScript.Build();

                //Mur Droit Haut
                Instance = Instantiate(WallPrefab.gameObject, RefEtage + Vector3.up * ((float)HauteurEtage / 2f + 2f) + Vector3.right * (LargeurMaison / 2f - .5f), new Quaternion());
                Instance.transform.parent = Construction;
                InstanceScript = Instance.GetComponent<WallBuilder>();
                InstanceScript.BuildAutomatic = false;
                InstanceScript.Size = new Vector3(1, HauteurEtage - 4, 1);
                InstanceScript.Build();

                //Mur gauche bas
                Instance = Instantiate(WallPrefab.gameObject, RefEtage + Vector3.up - Vector3.right * (LargeurMaison / 2f - .5f), new Quaternion());
                Instance.transform.parent = Construction;
                InstanceScript = Instance.GetComponent<WallBuilder>();
                InstanceScript.BuildAutomatic = false;
                InstanceScript.Size = new Vector3(1, 2, 1);
                InstanceScript.Build();

                //Mur Droit bas
                Instance = Instantiate(WallPrefab.gameObject, RefEtage + Vector3.up + Vector3.right * (LargeurMaison / 2f - .5f), new Quaternion());
                Instance.transform.parent = Construction;
                InstanceScript = Instance.GetComponent<WallBuilder>();
                InstanceScript.BuildAutomatic = false;
                InstanceScript.Size = new Vector3(1, 2, 1);
                InstanceScript.Build();
            }
            //SpecialitéEnFonctionDeLaLargeur;

            if(LargeurMaison>=7 && HauteurEtage > 4)
            {
                Instance = Instantiate(Coin, RefEtage + Vector3.up * (HauteurEtage - .5f) - Vector3.right * (LargeurMaison / 2f - 1.5f), new Quaternion());
                Instance.transform.eulerAngles = new Vector3(0, 0, -90);
                Instance.transform.parent = Construction;
                Instance = Instantiate(Coin, RefEtage + Vector3.up * (HauteurEtage - .5f) + Vector3.right * (LargeurMaison / 2f - 1.5f), new Quaternion());
                Instance.transform.eulerAngles = new Vector3(0, 0, 180);
                Instance.transform.parent = Construction;
            }

            /*Instance = Instantiate(Torche, RefEtage + Vector3.up * 2 + Vector3.forward * .5f, new Quaternion());
            Instance.transform.eulerAngles = new Vector3(-20, 0, 0);
            Instance.transform.parent = Construction;*/

            int nbTorche = (LargeurMaison - 2) / (EspacementTorche) +1;
            for (int i = 1;i<nbTorche;i++)
            {
                Instance = Instantiate(Torche, RefEtage - Vector3.right * (nbTorche * EspacementTorche)/2f + Vector3.right * EspacementTorche * i + Vector3.up * HauteurEtage/2f + Vector3.forward * .5f, new Quaternion());
                Instance.transform.eulerAngles = new Vector3(-20, 0, 0);
                Instance.transform.parent = Construction;


            }
        }
        Vector3 RefToit = transform.position + Vector3.up * NbEtage * HauteurEtage;

        GameObject Instanc = Instantiate(FloorPrefab.gameObject, RefToit + Vector3.up * .5f, new Quaternion());
        Instanc.transform.parent = Construction;
        WallBuilder InstanceScrip = Instanc.GetComponent<WallBuilder>();
        InstanceScrip.BuildAutomatic = false;
        InstanceScrip.Size = new Vector3(LargeurMaison, 1, 3);
        InstanceScrip.Build();

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
/*[CustomEditor(typeof(HouseBuilder))]
public class HouseBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        HouseBuilder myScript = (HouseBuilder)target;
        if (GUILayout.Button("Build"))
        {
            myScript.Build();
        }
    }
}*/
