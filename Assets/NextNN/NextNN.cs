using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextNN : MonoBehaviour {
	[Range(.8f, 3f)]
	public float distNear = 2;
	public int cntNodes;
	public int cntLinks;
	NextNNGlobal global;
	// Use this for initialization
	void Start () {
		int num = 5;
		int numLayers = num;
		int numX = num;
		int numY = num;
		global = new NextNNGlobal(numLayers, numX, numY);
		global.distNear = distNear;
		global.CreateNodes();
		InvokeRepeating("CreateValue", 1, 1);
	}
	
	// Update is called once per frame
	void Update () {
		global.distNear = distNear;
		global.UpdateNodes();
		cntNodes = global.nodes.Count;
		cntLinks = global.linksAll.Count;
	}

	void CreateValue() {
		global.nodes[0].value = 10;
		global.nodes[0].Update();
	}
}
