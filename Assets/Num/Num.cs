﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Num : MonoBehaviour {
	int sizeMesh = 128; //128;
	int numMeshesX = 2;  // * 2 million nodes calculation at 7fps mesh hidden  (7 million calculations per second)
    int numMeshesY = 2;  // * 4 250K nodes with mesh at 7fps
	int numNodesX;
	int numNodesY;
	float[,] nodes;
	float[,] nodesLast;
	GameObject parent;
	int epoch = 0;
	public bool ynStep = false;
	float startTime;
	float delay = 1;
	int fps;
	int cntFps;
	int cntFrames;
	float valueAdd = 10;
	public bool ynGos = false;
	public bool ynMesh = true;
	bool ynGosLast;
	//MeshManager meshManager;
	Mesh[,] meshes;
	GameObject[,] meshGos;
	MeshFilter[,] meshFilters;
	Material materialMesh;
	bool ynMeshLast;
	int cntVertices;
	GameObject meshesGo;
	// Use this for initialization
	void Start () {
		numNodesX = sizeMesh * numMeshesX;
		numNodesY = sizeMesh * numMeshesY;
		valueAdd = numNodesX / 4;
        //
		nodes = new float[numNodesX, numNodesY];
		nodesLast = new float[numNodesX, numNodesY];
		for (int nx = 0; nx < numNodesX; nx++){
			for (int ny = 0; ny < numNodesY; ny++)
            {
                nodes[nx, ny] = 0;
            }
		}
		InvokeRepeating("ShowFps", 1, 1);
	}

	void UPdateMesh()
	{
		if (meshes == null) {
			//meshManager = new MeshManager();
			meshesGo = new GameObject();
			meshes = new Mesh[numMeshesX, numMeshesY];
			meshGos = new GameObject[numMeshesX, numMeshesY];
			meshFilters = new MeshFilter[numMeshesX, numMeshesY];
			Shader shader = Shader.Find("Custom/DoubleSided");
            materialMesh = new Material(shader);
            //          material.mainTexture = CreateGridTexture();
            materialMesh.mainTexture = Resources.Load<Texture2D>("grid");
    	}
		cntVertices = 0;
		for (int nx = 0; nx < numMeshesX; nx++)
		{
			for (int ny = 0; ny < numMeshesY; ny++)
			{
				meshes[nx, ny] = CreateMeshFromNodes(nx, ny);
				if (meshGos[nx, ny] == null)
				{
					meshGos[nx, ny] = new GameObject();
					meshGos[nx, ny].transform.parent = meshesGo.transform;
					MeshRenderer meshRenderer = meshGos[nx, ny].AddComponent<MeshRenderer>();
					meshRenderer.sharedMaterial = materialMesh;
					meshFilters[nx, ny] = meshGos[nx, ny].AddComponent<MeshFilter>();
				}
				meshFilters[nx, ny].sharedMesh = meshes[nx, ny];
				int cntVertices0 = meshes[nx, ny].vertices.Length;
				meshGos[nx, ny].name = "MeshGo (vertices:" + cntVertices0 + ")";
				cntVertices += cntVertices0;
			}
			meshesGo.name = "Meshes (vertices:" + cntVertices + ")";
		}
	}

	Mesh CreateMeshFromNodes(int nx0, int ny0) {
		int numX = numNodesX / numMeshesX;
        int numY = numNodesY / numMeshesY;

		int startX = nx0 * numX;
        int startY = ny0 * numY;

		int endX = startX + numX;
        int endY = startY + numY;

        Vector3[] points = new Vector3[numX * numY];
        for (int nx = startX; nx < endX; nx++)
        {
            for (int ny = startY; ny < endY; ny++)
            {
                int nNodes = nx * numNodesY + ny;
                float dist = nodes[nx, ny];
                int nPoints = (nx - startX) * numY + (ny - startY);
                points[nPoints] = new Vector3(nx, dist, ny);
            }
        }
		Mesh mesh = CreateMeshFromPoints(points, numX - 1, numY - 1);
		return mesh;
	}

	public Mesh CreateMeshFromPoints(Vector3[] points, int numX, int numY)
    {
        int numFacesPerQuad = 2;
        int numVerticesPerQuad = 4;
        int numQuads = numX * numY;
        int numTrianglesPerQuad = numFacesPerQuad * 3;
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[numVerticesPerQuad * numQuads];
        Vector2[] uvs = new Vector2[numVerticesPerQuad * numQuads];
        int[] triangles = new int[numTrianglesPerQuad * numQuads];
        int q = 0;
        for (int nx = 0; nx < numX; nx++)
        {
            for (int ny = 0; ny < numY; ny++)
            {
				int n0 = getDR2N(nx, ny, numY);
				int n1 = getDR2N(nx, ny + 1, numY);
				int n2 = getDR2N(nx + 1, ny + 1, numY);
				int n3 = getDR2N(nx + 1, ny, numY);
                Vector3 pnt0 = points[n0];
                Vector3 pnt1 = points[n1];
                Vector3 pnt2 = points[n2];
                Vector3 pnt3 = points[n3];
                //
                vertices[q * numVerticesPerQuad + 0] = pnt0;
                vertices[q * numVerticesPerQuad + 1] = pnt1;
                vertices[q * numVerticesPerQuad + 2] = pnt2;
                vertices[q * numVerticesPerQuad + 3] = pnt3;
				//
				float s = .05f;
                uvs[q * numVerticesPerQuad + 0] = new Vector2(0, 0);
                uvs[q * numVerticesPerQuad + 1] = new Vector2(0, s);
                uvs[q * numVerticesPerQuad + 2] = new Vector2(s, s);
                uvs[q * numVerticesPerQuad + 3] = new Vector2(s, 0);
                //
                triangles[q * numTrianglesPerQuad + 0] = q * numVerticesPerQuad + 0;
                triangles[q * numTrianglesPerQuad + 1] = q * numVerticesPerQuad + 1;
                triangles[q * numTrianglesPerQuad + 2] = q * numVerticesPerQuad + 2;
                triangles[q * numTrianglesPerQuad + 3] = q * numVerticesPerQuad + 0;
                triangles[q * numTrianglesPerQuad + 4] = q * numVerticesPerQuad + 2;
                triangles[q * numTrianglesPerQuad + 5] = q * numVerticesPerQuad + 3;
                q++;
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
    int getDR2N(int dx, int dy, int numY)
    {
		return dx * (numY + 1) + dy;
    }
	Vector3[] CreatePointsFromNodes(int numX, int numY) {
		Vector3[] points = new Vector3[numX * numY];
        for (int nx = 0; nx < numX; nx++)
        {
            for (int ny = 0; ny < numY; ny++)
            {
                int n = nx * numY + ny;
				float dist = nodes[nx, ny];
                points[n] = new Vector3(nx, dist, ny);
            }
        }
        return points;
	}

	Vector3[] CreateDemoMeshPoints(int numX, int numY) {
        Vector3[] points = new Vector3[numX * numY];
        for (int nx = 0; nx < numX; nx++)
        {
            for (int ny = 0; ny < numY; ny++)
            {
                int n = nx * numY + ny;
				float dist = 1 * Mathf.Sin(cntFrames * .25f + 3 * n * Mathf.Deg2Rad);
                points[n] = new Vector3(nx, dist, ny);
            }
        }
		return points;
	}

	Texture2D CreateGridTexture() {
		int numX = 10;
        int numY = 10;
		Texture2D texture = new Texture2D(numX, numY);
		for (int nx = 0; nx < numX; nx++) {
			for (int ny = 0; ny < numY; ny++)
            {
				if (nx % 4 <= 2 || ny % 4 <= 2) {
					texture.SetPixel(nx, ny, Color.black);
				} else {
					texture.SetPixel(nx, ny, Color.white);
				}
            }
		}
		texture.Apply();
		return texture;
	}

	void ShowFps() {
		fps = cntFps;
		name = "Num (nodes:" + (numNodesX * numNodesY) + " fps:" + fps + ")";
		cntFps = 0;
	}
    
	void SineCurve() {
		int numT = 5;
		for (int t = 0; t < numT; t++)
		{
			float f = (t + 1) / (numT + 1f);
			int nx = (int)(numNodesX * f);
			int ny = (int)(numNodesY * f);
			nx = Random.Range(0, numNodesX);
			ny = Random.Range(0, numNodesY);
			float value = valueAdd * Mathf.Cos((cntFrames * 5 + (t * 90)) * Mathf.Deg2Rad);
			nodes[nx, ny] = value;
		}
	}

	// Update is called once per frame
	void Update () {
		//Debug.Log("............................\n");
		if (ynStep == false || Time.realtimeSinceStartup - startTime > delay)
		{
			startTime = Time.realtimeSinceStartup;
			SineCurve();
			Compute();
			ShowGos();
			if (ynMesh == true)
            {
                UPdateMesh();
            }
			epoch++;
		}
		cntFps++;
		cntFrames++;
		if (ynGosLast == true && ynGos == false) {
			DestroyImmediate(parent);
		}
		ynGosLast = ynGos;
		if (ynMeshLast == true && ynMesh == false) {
			for (int nx = 0; nx < numMeshesX; nx++)
			{
				for (int ny = 0; ny < numMeshesY; ny++)
				{
					DestroyImmediate(meshGos[nx, ny]);
				}
			}
		}
		ynMeshLast = ynMesh;
	}
	void Compute() {
		nodesLast = nodes;
		nodes = new float[numNodesX, numNodesY];
		for (int nx = 0; nx < numNodesX; nx++)
        {
			for (int ny = 0; ny < numNodesY; ny++)
            {
                Activate(nx, ny);
            }
        }
	}
	void Activate(int nx, int ny) {
		float value = nodesLast[nx, ny];
		int nLeft = WrapNX(nx - 1);
		int nRight = WrapNX(nx + 1);
		int nDown = WrapNY(ny - 1);
        int nUp = WrapNY(ny + 1);
		float share = .5f;
		float valueQuarter = share * value / 4;
		value = (1f - share) * value;
		nodes[nLeft, ny] += valueQuarter;
		nodes[nRight, ny] += valueQuarter;
		nodes[nx, nUp] += valueQuarter;
		nodes[nx, nDown] += valueQuarter;
        nodes[nx, ny] += value;
	}
	int WrapNX(int nx) {
		int resultX = nx;
		if (nx < 0)
        {
			resultX = numNodesX - 1;
		}
		if (nx >= numNodesX)
        {
			resultX = 0;
        }
		return resultX;
	}
	int WrapNY(int ny)
    {
        int resultY = ny;
        if (ny < 0)
        {
            resultY = numNodesY - 1;
        }
        if (ny >= numNodesY)
        {
            resultY = 0;
        }
        return resultY;
    }
	void ShowGos() {
		if (ynGos == false) return;
		if (parent != null) {
			DestroyImmediate(parent);
		}
		parent = new GameObject();
		for (int nx = 0; nx < numNodesX; nx++)
        {
			for (int ny = 0; ny < numNodesY; ny++)
			{
				GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
				go.name = nx + ", " + ny;
				go.transform.parent = parent.transform;
				float x = nx;
				float value = nodes[nx, ny];
				float y = value;
				float z = ny;
				go.transform.position = new Vector3(x, y, z);
				go.transform.localScale = new Vector3(.5f, .5f, .5f);
				float c = Mathf.Abs(value) / valueAdd * 1.5f;
				float r = c;
				float g = c;
				float b = c;
				if (value < 0)
				{
					r *= 1.5f;
					g *= 1 / 1.25f;
					b *= 1 / 1.25f;
				}
				else
				{
					g *= 1.5f;
					r *= 1 / 1.25f;
					b *= 1 / 1.25f;
				}
				go.GetComponent<Renderer>().material.color = new Color(r, g, b);
			}
        }
		parent.name = "parent(nodes:" + (numNodesX * numNodesY);
	}
}
