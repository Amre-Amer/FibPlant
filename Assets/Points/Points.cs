using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Points : MonoBehaviour {

	Mesh mesh;
    int numPoints = 60000;
	GameObject goMesh;

    // Use this for initialization
    void Start()
    {
		goMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
		MeshFilter meshFilter = goMesh.GetComponent<MeshFilter>();
		//MeshRenderer meshRenderer = goMesh.GetComponent<MeshRenderer>();
		//meshRenderer.sharedMaterial = new Material(Shader.Find("Custom/Particle"));
		mesh = new Mesh();
		meshFilter.mesh = mesh;
        CreateMesh();
    }

    void CreateMesh()
    {
        Vector3[] points = new Vector3[numPoints];
        int[] indecies = new int[numPoints];
//        Color[] colors = new Color[numPoints];
        for (int i = 0; i < points.Length; ++i)
        {
            points[i] = new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10));
            indecies[i] = i;
//            colors[i] = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1.0f);
        }

        mesh.vertices = points;
//        mesh.colors = colors;
        mesh.SetIndices(indecies, MeshTopology.Points, 0);

    }
}
