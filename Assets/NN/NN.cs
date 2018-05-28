using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NNglobal {
	public int epoch;
	public bool ynStep = true;
	public float startTime;
	public float delay = 1;
	public int numInputs = 28 * 28;
	public int numOutputs = 10;
	public List<NNnode>[] nodeLayers;
	public bool ynRadial = false;
	public GameObject tmpGo;
	List<int> layerCounts;
	public NNglobal() {
		tmpGo = new GameObject("tmpGo");
	}
	public void SetupNodes(List<int> hiddenLayerCounts) {
		layerCounts = new List<int>();
		layerCounts.Add(numInputs);
		for (int hLayer = 0; hLayer < hiddenLayerCounts.Count; hLayer++)
        {
			layerCounts.Add(hiddenLayerCounts[hLayer]);
        }
		layerCounts.Add(numOutputs);
		CreateNodesLayers();
		UpdateNodes();
		CreateLinks();
	}
	public void CreateNodesLayers() {
		nodeLayers = new List<NNnode>[layerCounts.Count];
        for (int layer = 0; layer < layerCounts.Count; layer++)
        {
            CreateNodesLayer(layer, layerCounts[layer]);
        }
	}
	public void CreateNodesLayer(int layer, int numNodes) {
		nodeLayers[layer] = new List<NNnode>();
        for (int n = 0; n < numNodes; n++)
        {
			NNnode node = new NNnode(layer, this);
        }
	}
	public void CreateLinks() {
		for (int layer = 0; layer < layerCounts.Count; layer++)
        {
			for (int n = 0; n < nodeLayers[layer].Count; n++)
            {
				NNnode node = nodeLayers[layer][n];
				CreateLinksForNode(node);
            }
        }
	}
	public void CreateLinksForNode(NNnode nodeFrom) {
		if (nodeFrom.layer > 0) {
			for (int n = 0; n < nodeLayers[nodeFrom.layer - 1].Count; n++) {
				NNnode nodeTo = nodeLayers[nodeFrom.layer - 1][n];
				CreateLink(nodeFrom, nodeTo);                				
			}
		}
	}
	public void CreateLink(NNnode nodeFrom, NNnode nodeTo) {
		Vector3 posFrom = nodeFrom.go.transform.position;
		Vector3 posTo = nodeTo.go.transform.position;
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.position = (posFrom + posTo) / 2;
		float dist = Vector3.Distance(posFrom, posTo);
		go.transform.LookAt(posTo);
		go.transform.localScale = new Vector3(.1f, .1f, dist);
	} 
	public void UpdateNodes() {
		for (int layer = 0; layer < nodeLayers.Length; layer++) {
			for (int n = 0; n < nodeLayers[layer].Count; n++) {
				NNnode node = nodeLayers[layer][n];
				node.UpdatePos();
			}
		}
	}
	public void LoadInputs(int[]data) {
		
	}
}
public class NNnode {
	public float output;
	public float[] inputs;
	public float[] weights;
	public float bias;
	public NNglobal global;
	public int layer;
	public int index;
	public GameObject go;
	public NNnode(int layer0, NNglobal global0) {
		layer = layer0;
		global = global0;
		go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		global.nodeLayers[layer].Add(this);
		index = global.nodeLayers[layer].Count - 1;
		go.name = layer + " " + index;
		go.GetComponent<Renderer>().material.color = GetColor(layer);
	}
	public void UpdatePos() {
		go.transform.position = GetPos();
	}
	Vector3 GetPos()
	{
		if (global.ynRadial == true) {
			return GetPosRadial();           
		} else {
			return GetPosOrtho();			
		}
	}
	Vector3 GetPosRadial()
    {
		float yStride = 2;
        float xStride = 2;
		int numNodes = global.nodeLayers[layer].Count;
		float circum = xStride * numNodes;
		float rad = circum / (2 * Mathf.PI);
		float ang = index / (float)numNodes * 360;
		global.tmpGo.transform.position = Vector3.zero + Vector3.up * layer * yStride;
		global.tmpGo.transform.eulerAngles = new Vector3(0, ang, 0);
		global.tmpGo.transform.position += global.tmpGo.transform.forward * rad;
		return global.tmpGo.transform.position;
    }
	Vector3 GetPosOrtho() {
		int numY = (int) Mathf.Sqrt(global.numInputs);
		float xStride = 10;
        float yStride = 2;
        float x = layer * xStride;
		float y = (index % numY) * yStride;
        float z = 0;
		float xOffset = 0;
		float yOffset = 0;
		if (layer == 0)
        {
            xStride = 1.25f;
            yStride = 1.25f;
			xOffset = -1f * numY * xStride;
			yOffset = 0; 
            x = index / numY;
            y = index % numY;
            x = xOffset + x * xStride;
			Debug.Log(y + "\n");
            y = yOffset + y * yStride;
        }
		return new Vector3(x, y, z);
	}
	public Color GetColor(int layer) {
		Color color = Color.black;
		if (layer == 0) color = Color.red;
        if (layer == 1) color = Color.yellow;
        if (layer == 2) color = Color.yellow;
        if (layer == 3) color = Color.green;
		return color;
	}
	public void Activate() {
		output = 0;
		for (int n = 0; n < inputs.Length; n++) {
			output += inputs[n] * weights[n];			
		}
		output += bias;
		output = Sigmoid(output);
	}
	public float Sigmoid(float f) {
		return 1 / (1 + Mathf.Exp( - f));
	}
}
public class NN : MonoBehaviour
{
    NNglobal global;
    void Start()
    {
        global = new NNglobal();
        global.SetupNodes(new List<int>{16, 16});
    }
    void Update()
    {
        if (global.ynStep == true && Time.realtimeSinceStartup - global.startTime < global.delay)
        {
            return;
        }
        global.startTime = Time.realtimeSinceStartup;
        Debug.Log("epoch:" + global.epoch + "\n");
        global.epoch++;
    }

}
