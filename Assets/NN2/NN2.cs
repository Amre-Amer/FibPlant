using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NN2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		int numInputs = 10 * 15;
		int numHiddenLayerNodes = 16;
		int numOutputLayerNodes = 3;
		int[] layerNodeCounts = new int[] {numInputs, numHiddenLayerNodes, numOutputLayerNodes};
		NN2Global global = new NN2Global(layerNodeCounts);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
