using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour {
	int numX = 25;
	int numLevel = 2; //0;
	int numZ = 35;
	float spacingX = 3;
	float spacingLevel = 3;
	float spacingZ = 3;
	string filename = "letter_b";
	Texture2D texture;
	GameObject[,,] nodes;
	// Use this for initialization
	void Start () {
		Vector3 v1 = new Vector3(1, 1, 0);
		Vector3 v2 = new Vector3(2, 1, 1);
		Vector3 vector = (v1 + v2) / 2;
		Debug.Log(v1 + " + " + v2 + " = " + vector + "\n");
		CreateGrid();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void CreateGrid() {
		nodes = new GameObject[numLevel, numX, numZ];
		texture = Resources.Load<Texture2D>(filename);
		for (int lev = 0; lev < numLevel; lev++)
        {
			for (int nx = 0; nx < numX - lev; nx++)
            {
        		for (int nz = 0; nz < numZ - lev; nz++)
        		{
					GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
					Vector3 pos = posForLevXZ(lev, nx, nz);
					go.transform.position = pos;
					nodes[lev, nx, nz] = go;
					Color col = Color.white;
					if (lev == 0) {
						col = texture.GetPixel(nx, nz);						
					} else {
						col = CompareFourColors(lev, nx, nz);
						CreateLinksForNode(lev, nx, nz);
					}
					go.GetComponent<Renderer>().material.color = col;
				}
			}
		}
	}
	bool IsColorOn(Color color) {
		if (color.r > .5f || color.g > .5f || color.b > .5f) {
			return true;
		} else {
			return false;
		}
	}
	Color CompareFourColors(int lev, int nx, int nz) {
		int cnt = 0;
		for (int dx = 0; dx <= 1; dx++)
        {
            for (int dz = 0; dz <= 1; dz++)
            {
                int ndx = nx + dx;
                int ndz = nz + dz;
				Color col = nodes[lev - 1, nx, nz].GetComponent<Renderer>().material.color;
				if (IsColorOn(col) == true) {
					cnt++;
				}
            }
        }
		if (cnt >= 2) {
			return Color.green;
		} else {
			return Color.black;
		}
	}
	void CreateLinksForNode(int lev, int nx, int nz) {
		if (lev > 0)
        {
			Vector3 pos = posForLevXZ(lev, nx, nz);
            for (int dx = 0; dx <= 1; dx++)
            {
                for (int dz = 0; dz <= 1; dz++)
                {
                    int ndx = nx + dx;
                    int ndz = nz + dz;
                    Vector3 posFrom = posForLevXZ(lev - 1, ndx, ndz);
                    Vector3 posTo = pos;
                    CreateLink(posFrom, posTo);
                }
            }
        }
	}
	void CreateLink(Vector3 posFrom, Vector3 posTo) {
		GameObject goLink = GameObject.CreatePrimitive(PrimitiveType.Cube);
		goLink.transform.position = (posFrom + posTo) / 2;
		goLink.transform.LookAt(posTo);
		float dist = Vector3.Distance(posFrom, posTo);
		goLink.transform.localScale = new Vector3(.1f, .1f, dist);
	}
	Vector3 posForLevXZ(int lev, int nx, int nz) {
		float x = (nx + .5f * lev) * spacingX;
        float y = lev * spacingLevel;
        float z = (nz + .5f * lev) * spacingZ;
		return new Vector3(x, y, z);
	}
}
