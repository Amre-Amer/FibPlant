using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Num : MonoBehaviour {
	int numNodesX = 128 * 2;
	int numNodesY = 128 * 2;
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
	MeshManager meshManager;
	Mesh[,] meshes;
	GameObject[,] meshGos;
	MeshFilter[,] meshFilters;
	Material materialMesh;
	bool ynMeshLast;
	int numMeshesX = 2;
	int numMeshesY = 2;
	int cntVertices;
	GameObject meshesGo;
	// Use this for initialization
	void Start () {
		valueAdd = numNodesX / 4;
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
		if (meshManager == null) {
			meshesGo = new GameObject();
			meshManager = new MeshManager();

			meshes = new Mesh[numMeshesX, numMeshesY];
			meshGos = new GameObject[numMeshesX, numMeshesY];
			meshFilters = new MeshFilter[numMeshesX, numMeshesY];
			Shader shader = Shader.Find("Custom/DoubleSided");
            materialMesh = new Material(shader);
            //          material.mainTexture = CreateGridTexture();
            materialMesh.mainTexture = Resources.Load<Texture2D>("grid");
    	}
		meshManager.numRadDivs = numNodesX - 1;
		meshManager.numLengthDivs = numNodesY - 1;
		//meshManager.points = CreatePointsFromNodes(numNodesX, numNodesY);
 		  //meshManager.points = CreateDemoMeshPoints(numNodesX, numNodesY);
		cntVertices = 0;
		for (int nx = 0; nx < numMeshesX; nx++)
		{
			for (int ny = 0; ny < numMeshesY; ny++)
			{
//				meshManager.points = CreatePointsFromNodes(numNodesX, numNodesY);
				meshManager.points = CreatePointsFromNodesForMesh(nx, ny);
				meshes[nx, ny] = meshManager.CreateMeshFromPoints();
				if (meshGos[nx, ny] == null)
				{
					meshGos[nx, ny] = new GameObject();
					meshGos[nx, ny].transform.parent = meshesGo.transform;
					float x = (numNodesX - 1) / numMeshesX * nx;
					float y = (numNodesY - 1) / numMeshesY * ny;
					meshGos[nx, ny].transform.position += new Vector3(x, 0, y);
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

	Vector3[] CreatePointsFromNodesForMesh(int nx, int ny)
	{
		int numX = numNodesX / numMeshesX;
		int numY = numNodesY / numMeshesY;
		int startX = nx * numX;
		int startY = ny * numY;
		meshManager.numRadDivs = numX - 1;
        meshManager.numLengthDivs = numY - 1;
		return CreatePointsFromNodesRect(startX, startY, numX, numY);
	}
	Vector3[] CreatePointsFromNodesRect(int startX, int startY, int numX, int numY)
    {
//		Debug.Log("startX,Y:" + startX + "," + startY + " numX,Y:" + numX + "," + numY + " numNodesX:" + numNodesX + " numNodesY:" + numNodesY + "\n");
        Vector3[] points = new Vector3[numX * numY];
		for (int nx = startX; nx < (startX + numX); nx++)
        {
			for (int ny = startY; ny < (startY + numY); ny++)
            {
				float dist = nodes[nx, ny];
				int nPoints = (nx - startX) * numY + (ny - startY);
				//Debug.Log("nPoints:" + nPoints + " nx:" + nx + " ny:" + ny + " (" + points.Length + ")\n");
                points[nPoints] = new Vector3(nx, dist, ny);
            }
        }
//		Debug.Log("points:" + points.Length + "\n");
        return points;
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
		//numX = numNodesX;
		//numY = numNodesY;
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
		int cycle = 200;
		//if (cntFrames >= cycle) cntFrames = 0;
		//if (cntFrames < cycle / 2)
		//{
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
		//} else {
			//if (cntFrames % (cycle / 10) == 0)
			//{
			//	int nx = Random.Range(0, numNodesX);
			//	int ny = Random.Range(0, numNodesY);
			//	float value = valueAdd;
			//	nodes[nx, ny] = value;
			//}
		//}
	}

	// Update is called once per frame
	void Update () {
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
		//if (ynMesh == true) {
		//	UPdateMesh();
		//}
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
