using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WallBuilder : MonoBehaviour
{
    public Vector3 Size = new Vector3(1,1,1);
    public bool BuildAutomatic = true;
    public List<GameObject> Faces;

    private Transform Construction = null;

    public void Build()
    {
        RoundValue();
        Construction = transform.Find("Construction");
        if (Construction!=null) DestroyImmediate(Construction.gameObject);
        Construction = new GameObject("Construction").transform;
        Construction.parent = transform;

        //FaceAvant
        GameObject Avant = new GameObject("FaceAvant");
        Avant.transform.parent = Construction.transform;
        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                GameObject Instance = Instantiate(Faces[Random.Range(0,Faces.Count)]);
                Instance.transform.localScale = Vector3.one * 100;
                Instance.transform.eulerAngles = new Vector3(-90, 0, 0);
                
                Vector3 FacePosition = transform.position - Vector3.forward * Size.z/2;
                FacePosition += Vector3.right * (x - Size.x / 2 + .5f);
                FacePosition += Vector3.up * (y - Size.y / 2 + .5f);
                Instance.transform.position = FacePosition;
                Instance.transform.parent = Avant.transform;
            }
        }
        //FaceArriere
        GameObject Arriere = new GameObject("FaceArriere");
        Arriere.transform.parent = Construction.transform;
        for (int y = 0; y < Size.y; y++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                GameObject Instance = Instantiate(Faces[Random.Range(0, Faces.Count)]);
                Instance.transform.localScale = Vector3.one * 100;
                Instance.transform.eulerAngles = new Vector3(90, 0, 0);

                Vector3 FacePosition = transform.position + Vector3.forward * Size.z / 2;
                FacePosition += Vector3.right * (x - Size.x / 2 + .5f);
                FacePosition += Vector3.up * (y - Size.y / 2 + .5f);
                Instance.transform.position = FacePosition;
                Instance.transform.parent = Arriere.transform;
            }
        }
        //Dessus
        GameObject Dessus = new GameObject("FaceDessus");
        Dessus.transform.parent = Construction.transform;
        for (int z = 0; z < Size.z; z++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                GameObject Instance = Instantiate(Faces[Random.Range(0, Faces.Count)]);
                Instance.transform.localScale = Vector3.one * 100;
                Instance.transform.eulerAngles = new Vector3(0, 0, 0);

                Vector3 FacePosition = transform.position + Vector3.up * Size.y / 2;
                FacePosition += Vector3.right * (x - Size.x / 2 + .5f);
                FacePosition += Vector3.forward * (z - Size.z / 2 + .5f);
                Instance.transform.position = FacePosition;
                Instance.transform.parent = Dessus.transform;
            }
        }
        //Dessous
        GameObject Dessous = new GameObject("FaceDessous");
        Dessous.transform.parent = Construction.transform;
        for (int z = 0; z < Size.z; z++)
        {
            for (int x = 0; x < Size.x; x++)
            {
                GameObject Instance = Instantiate(Faces[Random.Range(0, Faces.Count)]);
                Instance.transform.localScale = Vector3.one * 100;
                Instance.transform.eulerAngles = new Vector3(180, 0, 0);

                Vector3 FacePosition = transform.position - Vector3.up * Size.y / 2;
                FacePosition += Vector3.right * (x - Size.x / 2 + .5f);
                FacePosition += Vector3.forward * (z - Size.z / 2 + .5f);
                Instance.transform.position = FacePosition;
                Instance.transform.parent = Dessous.transform;
            }
        }
        //Gauche
        GameObject Gauche = new GameObject("FaceGauche");
        Gauche.transform.parent = Construction.transform;
        for (int z = 0; z < Size.z; z++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                GameObject Instance = Instantiate(Faces[Random.Range(0, Faces.Count)]);
                Instance.transform.localScale = Vector3.one * 100;
                Instance.transform.eulerAngles = new Vector3(-90, 0, 90);

                Vector3 FacePosition = transform.position - Vector3.right * Size.x / 2;
                FacePosition += Vector3.up * (y - Size.y / 2 + .5f);
                FacePosition += Vector3.forward * (z - Size.z / 2 + .5f);
                Instance.transform.position = FacePosition;
                Instance.transform.parent = Gauche.transform;
            }
        }
        //Droite
        GameObject Droite = new GameObject("FaceDroite");
        Droite.transform.parent = Construction.transform;
        for (int z = 0; z < Size.z; z++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                GameObject Instance = Instantiate(Faces[Random.Range(0, Faces.Count)]);
                Instance.transform.localScale = Vector3.one * 100;
                Instance.transform.eulerAngles = new Vector3(-90, 0, -90);

                Vector3 FacePosition = transform.position + Vector3.right * Size.x / 2;
                FacePosition += Vector3.up * (y - Size.y / 2 + .5f);
                FacePosition += Vector3.forward * (z - Size.z / 2 + .5f);
                Instance.transform.position = FacePosition;
                Instance.transform.parent = Droite.transform;
            }
        }
        BoxCollider Col = GetComponent<BoxCollider>();
        if (Col == null) Col = gameObject.AddComponent<BoxCollider>();
        Col.size = Size;

    }

    private void OnDrawGizmosSelected()
    {
        RoundValue();
        if (BuildAutomatic) Build();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, Size);


    }

    void RoundValue()
    {
        Size = new Vector3(Mathf.Round(Size.x), Mathf.Round(Size.y), Mathf.Round(Size.z));
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

/*[CustomEditor(typeof(WallBuilder))]
public class WallBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        WallBuilder myScript = (WallBuilder)target;
        if (GUILayout.Button("Build"))
        {
            myScript.Build();
        }
    }
}*/
