using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Num : MonoBehaviour {
	public int numNodesX = 128;
	public int numNodesY = 128;
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
	//GameObject meshGo;
	//MeshFilter meshFilter;
	//Mesh mesh;
	//GameObject meshGo2;
	//MeshFilter meshFilter2;
    //Mesh mesh2;
	Mesh[] meshes;
	GameObject[] meshGos;
	MeshFilter[] meshFilters;
	bool ynMeshLast;
	int numMeshes = 2;
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
			meshManager = new MeshManager();

    	}
		meshManager.numRadDivs = numNodesX - 1;
		meshManager.numLengthDivs = numNodesY - 1;
		meshes = new Mesh[numMeshes];
		meshGos = new GameObject[numMeshes];
		meshFilters = new MeshFilter[numMeshes];
//		meshManager.points = CreateDemoMeshPoints(numNodesX, numNodesY);
		meshManager.points = CreatePointsFromNodes(numNodesX, numNodesY);
		for (int n = 0; n < numMeshes; n++)
		{
			//mesh = meshManager.CreateMeshFromPoints();
			//mesh2 = meshManager.CreateMeshFromPoints();
			meshes[n] = meshManager.CreateMeshFromPoints();
			if (meshGos[n] == null)
			{
				meshGos[n] = new GameObject();
				meshGos[n].transform.position += new Vector3(numNodesX * 1.1f * n, 0, 0);

				Shader shader = Shader.Find("Custom/DoubleSided");
				Material material = new Material(shader);
				//			material.mainTexture = CreateGridTexture();
				material.mainTexture = Resources.Load<Texture2D>("grid");
				MeshRenderer meshRenderer = meshGos[n].AddComponent<MeshRenderer>();
				meshRenderer.sharedMaterial = material;
				meshFilters[n] = meshGos[n].AddComponent<MeshFilter>();
				//
				//meshGo2 = new GameObject();
				//meshGo2.transform.position += new Vector3(numNodesX * 1.1f, 0, 0);
				//Shader shader2 = Shader.Find("Custom/DoubleSided");
				//Material material2 = new Material(shader2);
				////          material.mainTexture = CreateGridTexture();
				//material2.mainTexture = Resources.Load<Texture2D>("grid");
				//MeshRenderer meshRenderer2 = meshGo2.AddComponent<MeshRenderer>();
				//meshRenderer2.sharedMaterial = material2;
				//meshFilter2 = meshGo2.AddComponent<MeshFilter>();
			}
			meshFilters[n].sharedMesh = meshes[n];
			//meshFilter.sharedMesh = mesh;
			//meshFilter2.sharedMesh = mesh2;
			meshGos[n].name = "MeshGo (vertices:" + meshes[n].vertices.Length + ")";
		}
		//meshGos[n].name = "MeshGo (vertices:" + meshes[n].vertices.Length + ")";
		//Debug.Log("mesh vertices:" + mesh.vertices.Length + "\n");
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
		name = "Num (fps:" + fps + ")";
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
			for (int n = 0; n < numMeshes; n++)
			{
				DestroyImmediate(meshGos[n]);
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
