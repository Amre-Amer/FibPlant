using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NNglobal {
	public int epoch;
	public bool ynStep = true;
	public float startTime;
	public float delay = 1;
	public int numInputs;
	public int numOutputs;
	public List<NNnode>[] nodeLayers;
	public bool ynRadial = false;
	public GameObject tmpGo;
	public List<string> inputNames;
	public List<int> layerCounts;
	public NNglobal(int numInputs0, List<int> hiddenLayerCounts, int numOutputs0) {
		tmpGo = new GameObject("tmpGo");
		numInputs = numInputs0;
		numOutputs = numOutputs0;
		SetupNodes(hiddenLayerCounts);
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
		InitInputImages();
		LoadInputImage(0);
		UpdateNodes();
		CreateLinks();
		ProcessImage();
	}
	public void ProcessImage() {
		for (int layer = 1; layer < nodeLayers.Length; layer++) {
			foreach(NNnode node in nodeLayers[layer]) {
				node.Activate();
			}
		}
	}
	public void CreateNodesLayers() {
		nodeLayers = new List<NNnode>[layerCounts.Count];
        for (int layer = 0; layer < layerCounts.Count; layer++)
        {
			int numNodes = layerCounts[layer];
			//Debug.Log("layer:" + layer + " count:" + numNodes + "\n");
			CreateNodesLayer(layer, numNodes);
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
			List<NNnode> nodeLayerBelow = nodeLayers[nodeFrom.layer - 1];
			for (int n = 0; n < nodeLayerBelow.Count; n++) {
				NNnode nodeTo = nodeLayerBelow[n];
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
		go.GetComponent<Renderer>().material.color = Color.black;
		CreateLinkInputAndWeight(nodeFrom, nodeTo);
	} 
	public void CreateLinkInputAndWeight(NNnode nodeFrom, NNnode nodeTo) {
		nodeFrom.inputNodes.Add(nodeTo);
		nodeFrom.weights.Add(Random.Range(0f, 1f));
	}
	public void UpdateNodes() {
		for (int layer = 0; layer < nodeLayers.Length; layer++) {
			for (int n = 0; n < nodeLayers[layer].Count; n++) {
				NNnode node = nodeLayers[layer][n];
				node.Update();
			}
		}
	}
	public void InitInputImages() {
		inputNames = new List<string>();
		for (int n = 1; n <= 6; n++) {
			inputNames.Add("size28_" + n.ToString());		
		}
		for (int n = 65; n <= 69; n++)
        {
			inputNames.Add("size28_" + System.Convert.ToChar(n));
        }
		//foreach (string name in inputNames) {
		//	Debug.Log(name + "\n");
		//}
//		inputNames = new List<string> {"size28_1", "size28_a", "size28_b"};
	}
	public void LoadInputImage(int n) {
		string filename = inputNames[n];
        int numY = (int)Mathf.Sqrt(numInputs);
        Texture2D texture = Resources.Load<Texture2D>(filename);
        int inputLayer = 0;
		for (int nx = 0; nx < texture.width; nx++)
        {
            for (int ny = 0; ny < texture.height; ny++)
            {
                Color color = texture.GetPixel(nx, ny);
                int index = nx * numY + ny;
				NNnode node = nodeLayers[inputLayer][index];
                node.output = 1f - color.grayscale;
				node.Update();
            }
        }
	}
}
public class NNnode {
	public float output;
	public List<NNnode> inputNodes;
	public List<float> weights;
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
		inputNodes = new List<NNnode>();
		weights = new List<float>();
	}
	public void Update() {
		UpdatePos();
		UpdateColor();
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
		float zStride = 15;
        float x = layer * xStride;
		float y = (index % numY) * yStride;
		float z = layer * zStride;
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
            y = yOffset + y * yStride;
        }
		return new Vector3(x, y, z);
	}
	public void UpdateColor() {
		Color color = new Color(output, output, output);
		go.GetComponent<Renderer>().material.color = color;
	}
	public void Activate() {
		for (int n = 0; n < inputNodes.Count; n++) {
			float value = inputNodes[n].output * weights[n];
			//inputNodes[n].output = value;
			//inputNodes[n].Update();
			output += value;			
		}
		//		output += bias;
		//output = Random.Range(0f, 1f);
//		output = Sigmoid(output);
		Debug.Log("layer:" + layer + " index:" + index + " output:" + output + " sigmoid:" + Sigmoid(output) + "\n");
		//if (output) {
			
		//}
		UpdateColor();
	}
	public float Sigmoid(float f) {
		return 1 / (1 + Mathf.Exp(-f));
	}
}
/// <summary>
/// 
/// </summary>
public class NN : MonoBehaviour
{
    NNglobal global;
	int n;
    void Start()
    {
		int numInputs = 28 * 28;
		List<int> hiddenLayerCounts = new List<int> {16, 16};
		int numOutputs = 10;
		global = new NNglobal(numInputs, hiddenLayerCounts, numOutputs);
    }
    void Update()
    {
        if (global.ynStep == true && Time.realtimeSinceStartup - global.startTime < global.delay)
        {
            return;
        }
		Debug.Log("epoch:" + global.epoch + "\n");
		if (n < global.inputNames.Count)
		{
			Debug.Log("filename:" + global.inputNames[n] + "\n");
			global.LoadInputImage(n);
			//global.ProcessImage();
			n++;
		}
        global.startTime = Time.realtimeSinceStartup;
        global.epoch++;
    }

}
