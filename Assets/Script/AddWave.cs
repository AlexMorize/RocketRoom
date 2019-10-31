using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddWave : MonoBehaviour
{
    public float scale = 1f;
    public float speed = 1.0f;
    public float noiseStrength = 1f;
    public float noiseWalk = 1f;
    public float frequence = 1;

    private Vector3[] baseHeight;

    void Update()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        //MeshCollider collider = GetComponent<MeshCollider>();
        if (baseHeight == null)
            baseHeight = mesh.vertices;

        Vector3[] vertices = new Vector3[baseHeight.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = baseHeight[i];
            vertex.y += Mathf.Sin(Time.time * speed + (baseHeight[i].x + baseHeight[i].y + baseHeight[i].z)*frequence) * scale;
            vertex.y += Mathf.PerlinNoise(baseHeight[i].x + noiseWalk, baseHeight[i].y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
            vertices[i] = vertex;
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        //collider.sharedMesh = null;
        //collider.sharedMesh = mesh;
    }
}
