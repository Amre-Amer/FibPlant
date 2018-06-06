using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Num : MonoBehaviour {
	int numNodesX = 10;
	int numNodesY = 10;
	float[,] nodes;
	float[,] nodesLast;
	GameObject parent;
	int epoch = 0;
	bool ynStep = false;
	float startTime;
	float delay = 1;
	int fps;
	int cntFps;
	int cntFrames;
	float valueAdd = 10;
	// Use this for initialization
	void Start () {
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

	void ShowFps() {
		fps = cntFps;
		name = "Num (fps:" + fps + ")";
		cntFps = 0;
	}
    
	void SineCurve() {
		int cycle = 200;
		if (cntFrames >= cycle) cntFrames = 0;
		if (cntFrames < cycle / 2)
		{
			for (int t = 0; t < 2; t++)
			{
				int nx = (int)(numNodesX * (t + 1) * .33f);
				int ny = (int)(numNodesY * (t + 1) * .33f);
				float value = valueAdd * Mathf.Sin((cntFrames * 5 + (t * 90)) * Mathf.Deg2Rad);
				nodes[nx, ny] = value;
			}
		} else {
			if (cntFrames % (cycle / 10) == 0)
			{
				int nx = Random.Range(0, numNodesX);
				int ny = Random.Range(0, numNodesY);
				float value = valueAdd;
				nodes[nx, ny] = value;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (ynStep == false || Time.realtimeSinceStartup - startTime > delay)
		{
			startTime = Time.realtimeSinceStartup;
			SineCurve();
			Compute();
			Show();
			epoch++;
		}
		cntFps++;
		cntFrames++;
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
		float valueQuarter = value / 4;
        float valueLeft = valueQuarter;
        float valueRight = valueQuarter;
		float valueUp = valueQuarter;
        float valueDown = valueQuarter;
        value = 0;
        nodes[nLeft, ny] += valueLeft;
		nodes[nRight, ny] += valueRight;
		nodes[nx, nUp] += valueLeft;
        nodes[nx, nDown] += valueRight;
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
	void Show() {
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
		parent.name = "parent(nodes:" + (numNodesX * numNodesY) + ")";
	}
}
