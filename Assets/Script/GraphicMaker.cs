using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicMaker : MonoBehaviour
{
    const int EpaisseurTrait = 10, EpaisseurBordure = 4;
    Color CouleurBordure = Color.black;

    public int MaxX = 180;
    float MaxPoint = 0;

    [SerializeField] private Sprite CircleSprite;

    private RectTransform GraphContainer;
    private RectTransform AfficheurScoreFinal;
    public Dictionary<Color, Dictionary<float, float>> lesGraphs;

    // Start is called before the first frame update
    void Start()
    {
        GraphContainer = transform.Find("GraphContainer") as RectTransform;
        AfficheurScoreFinal = transform.Find("AfficheScoreFinal") as RectTransform;
        
        DrawGraphics(lesGraphs);
        AfficheurScoreFinal.gameObject.SetActive(false);
        /*Dictionary<float, float> valueList = new Dictionary<float, float>() { [0] = 0, [20] = 10, [30] = 20, [40] = 10, [50] = 20, [60] = 30 };
        Dictionary<float, float> valueListB = new Dictionary<float, float>() { [0] = 0, [10] = 10, [20] = 20, [35] = 30, [55] = 40, [70] = 50 };
        Dictionary<Color, Dictionary<float, float>> lesGraphs = new Dictionary<Color, Dictionary<float, float>>();
        lesGraphs.Add(Color.red, valueList);
        lesGraphs.Add(Color.blue, valueListB);
        
        //List<int> valueList = new List<int>() { 5, 98, 32, 45, 46, 84, 15, 32, 54, 26, 51, 32 };
        //ShowGraph(valueList, Color.red);
        //ShowGraph(valueListB, Color.blue);*/
    }

    private void DrawGraphics(Dictionary<Color, Dictionary<float, float>> lesGraphs)
    {
        MaxPoint = 10;
        foreach(var unGraph in lesGraphs.Values)
        {
            foreach(var unScore in unGraph.Values)
            {
                if (unScore > MaxPoint) MaxPoint = unScore;
            }
        }


        foreach(var unGraph in lesGraphs)
        {
            ShowGraph(unGraph.Value, CouleurBordure, EpaisseurTrait+EpaisseurBordure*2);
            ShowGraph(unGraph.Value, unGraph.Key, EpaisseurTrait);
        }
    }

    private GameObject CreateCircle(Vector2 position, Color couleur, float épaisseur)
    {
        GameObject go = new GameObject("circle", typeof(Image));
        go.transform.SetParent(GraphContainer, false);
        go.GetComponent<Image>().sprite = CircleSprite;
        go.GetComponent<Image>().color = couleur;
        //go.GetComponent<Image>().color = new Color(0,0,0,0);
        RectTransform rect = go.GetComponent<RectTransform>();

        rect.anchoredPosition = position;

        rect.sizeDelta = new Vector2(épaisseur, épaisseur);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        return go;
    }

    private void ShowGraph(Dictionary<float,float> valueList, Color couleur,int épaisseur)
    {
        float graphHeight = GraphContainer.sizeDelta.y;
        float graphLengt = GraphContainer.sizeDelta.x;
        float yMaximum = MaxPoint;
        float xSize = 50;
        //valueList.Add(180, valueList[valueList.Count - 1]);
        GameObject LastCircleGo = null;
        float lastValue = 0;
        foreach(var value in valueList)
        {
            lastValue = value.Value;
            float xPosition = value.Key/MaxX * graphLengt;
            if (xPosition > graphLengt) xPosition = graphLengt;
            float yPosition = value.Value / yMaximum * graphHeight;

            

            GameObject CircleGo = CreateCircle(new Vector2(xPosition, yPosition), couleur, épaisseur);
            if (LastCircleGo!=null)
            {
                CreateDotConnection(LastCircleGo.GetComponent<RectTransform>().anchoredPosition, CircleGo.GetComponent<RectTransform>().anchoredPosition, couleur, épaisseur);
            }
            LastCircleGo = CircleGo;
        }

        if(lastValue!=MaxX)
            CreateDotConnection(LastCircleGo.GetComponent<RectTransform>().anchoredPosition, new Vector2(graphLengt, LastCircleGo.GetComponent<RectTransform>().anchoredPosition.y), couleur, épaisseur);

        GameObject afficheurScore = Instantiate(AfficheurScoreFinal.gameObject,GraphContainer);
        afficheurScore.SetActive(true);
        afficheurScore.transform.Find("Image").GetComponent<Image>().color = couleur;
        afficheurScore.GetComponentInChildren<Text>().text = lastValue.ToString();
        RectTransform RectAffiche = afficheurScore.GetComponent<RectTransform>();
        RectAffiche.anchoredPosition = new Vector2(RectAffiche.anchoredPosition.x, LastCircleGo.GetComponent<RectTransform>().anchoredPosition.y);
    }

    private void CreateDotConnection(Vector2 posA, Vector2 posB, Color couleur, int épaisseur)
    {
        //couleur.a = .5f;
        Vector2 dir = (posB - posA).normalized;
        if (dir == Vector2.zero) return;
        GameObject go = new GameObject("dotConnection", typeof(Image));
        go.transform.SetParent(GraphContainer, false);
        go.GetComponent<Image>().color = couleur;
        
        float Dist = Vector2.Distance(posA, posB);
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.sizeDelta = new Vector2(Dist, épaisseur);
        rect.anchoredPosition = posA + dir * Dist/2;
        rect.localEulerAngles = Vector3.forward * GetAngleFromVectorFloat(dir);

    }

    private float GetAngleFromVectorFloat(Vector2 dir)
    {
        float difference = (dir.y / dir.x);
        float angleRot = Mathf.Atan(difference) * 180 / Mathf.PI;
        return angleRot;
    }
}
