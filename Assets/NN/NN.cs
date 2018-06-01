using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NNglobal {
	public int epoch;
	public bool ynStep = true;
	public float startTime;
	public float delay = 1;
	public int numInputs;
	public int numOutputs;
	public List<NNnode>[] nodeLayers;
	public GameObject tmpGo;
	public List<string> inputNames;
	public List<int> layerCounts;
	public GameObject parentNodes;
	public GameObject parentLinks;
	public float learningRate = .01f;
	public int cntNodes;
	public int cntWeights;
	public NNglobal(int numInputs0, List<int> hiddenLayerCounts, int numOutputs0) {
		tmpGo = new GameObject("tmpGo");
		parentNodes = new GameObject("parentNodes");
		parentLinks = new GameObject("parentLinks");
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
        //
		CreateNodesLayers();
		//UpdateNodes();
		CreateLinks();
		//
		ShowStats();
        //
		InitInputImages();
        //
        LoadInputImage(2);
		ProcessImage();
		//LearnImage();
//		ProcessImage();
	}
	public void ShowStats() {
		Debug.Log("NN layers\n");
        for (int n = 0; n < layerCounts.Count; n++)
        {
            string txt = "hidden";
            if (n == 0) txt = "input";
            if (n == layerCounts.Count - 1) txt = "output";
			Debug.Log(txt + " layer:" + n + " = " + layerCounts[n] + "\n");
        }
		Debug.Log("cntNodes:" + cntNodes + "\n");
		Debug.Log("cntWeights:" + cntWeights + "\n");
	}
	public void LearnImage() {
		learningRate = .1f;
		List<NNnode> outputLayer = nodeLayers[nodeLayers.Length - 1];
		foreach(NNnode node in outputLayer) {
			float error = node.correctAnswer - node.output;
			float scaleFactor = 1 + Mathf.Sign(error) * learningRate;
			Debug.Log("LearnRate scaleFactor:" + scaleFactor + "\n");
			ScaleWeights(node, scaleFactor);
			//ProcessImage();
			//break;
		}
		ProcessImage();
	}
	public void ScaleWeights(NNnode nodeOutput, float scaleFactor) {
		Color color = Random.ColorHSV();
		for (int layer = 1; layer < nodeLayers.Length; layer++)
		{
			for (int n = 0; n < nodeLayers[layer].Count; n++)
			{
				NNnode node = nodeLayers[layer][n];
				if (node != nodeOutput)
				{
					for (int w = 0; w < node.weights.Count; w++)
					{
						node.weights[w] *= scaleFactor;
						node.links[w].GetComponent<Renderer>().material.color = color;
					}
				}
			}
		}
	}
	public void ProcessImage() {
		for (int layer = 1; layer < nodeLayers.Length; layer++) {
			ActivateLayer(layer);
		}
		CalculateError();
	}
	public void CalculateError() {
		float errorSum = 0;
		List<NNnode> outputLayer = nodeLayers[nodeLayers.Length - 1];
		foreach(NNnode node in outputLayer) {
			errorSum += node.correctAnswer - node.output;
			Debug.Log("index:" + node.index + " correctAnswer:" + node.correctAnswer + " output:" + node.output + " = " + (node.correctAnswer - node.output).ToString("F2") + "\n");
		}
		float errorAve = errorSum / outputLayer.Count;
		Debug.Log("errorSum:" + errorSum + " errorAve:" + errorAve + "\n");
	}
	public void ActivateLayer(int layer) {
		float[] outputs = new float[nodeLayers[layer].Count];
		float min = 0;
		float max = 0;
		foreach (NNnode node in nodeLayers[layer])
        {
            node.Activate();
            float value = node.output;
            int n = node.index;
			if (n == 0 || value < min) min = value;
			if (n == 0 || value > max) max = value;
        }
		float span = max - min;
//		Debug.Log("layer:" + layer + " min:" + min + " max:" + max + " span:" + span + "\n");
		foreach (NNnode node in nodeLayers[layer])
        {
			float value = node.output;
			float score = (value - min) / (max - min);
			node.output = score;
			node.UpdateColor();
//			Debug.Log("layer:" + layer + " n:" + node.index + " output:" + value + " score:" + score + "\n");
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
		LabelOutputLayer();
	}
	public void LabelOutputLayer() {
		List<NNnode> outputLayer = nodeLayers[nodeLayers.Length - 1];
		foreach(NNnode node in outputLayer) {
			Vector3 pos = node.go.transform.position;
			pos += Vector3.right * 2;
			CreateText(pos, node.index.ToString(), Color.white);
			pos += Vector3.forward * .05f;
			CreateText(pos, node.index.ToString(), Color.black);
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
		go.transform.parent = parentLinks.transform;
		go.transform.position = (posFrom + posTo) / 2;
		float dist = Vector3.Distance(posFrom, posTo);
		go.transform.LookAt(posTo);
		go.transform.localScale = new Vector3(.1f, .1f, dist);
		go.GetComponent<Renderer>().material.color = new Color(0, 0, .125f);
		cntWeights++;
		CreateLinkInputAndWeight(nodeFrom, nodeTo, go);
	} 
	public void CreateLinkInputAndWeight(NNnode nodeFrom, NNnode nodeTo, GameObject goLink) {
		nodeFrom.inputNodes.Add(nodeTo);
		nodeFrom.weights.Add(Random.Range(0f, 1f));
		nodeFrom.links.Add(goLink);
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
		for (int n = 0; n <= 9; n++) {
			inputNames.Add("size28_" + n.ToString());		
		}
		//for (int n = 65; n <= 69; n++)
   //     {
			//inputNames.Add("size28_" + System.Convert.ToChar(n));
        //}
		//foreach (string name in inputNames) {
		//	Debug.Log(name + "\n");
		//}
//		inputNames = new List<string> {"size28_1", "size28_a", "size28_b"};
	}
	public void LoadInputImage(int n)
	{
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
		string lastChar = filename.Substring(filename.Length - 1, 1);
		string possibilites = "0123456789";
		if (possibilites.Contains(lastChar)) {
			float correctAnswer = float.Parse(lastChar);
			UpdateOutputLayerWithCorrectAnswer(correctAnswer);
		}
	}
	public void UpdateOutputLayerWithCorrectAnswer(float correctAnswer) {
		List<NNnode> outputLayer = nodeLayers[nodeLayers.Length - 1];
		foreach (NNnode node in outputLayer) {
			if (node.index == (int)correctAnswer) {
				node.correctAnswer = 1;
			} else {
				node.correctAnswer = 0;
			}
			//Debug.Log("layer:" + node.layer + " index:" + node.index + " correctAnswer:" + node.correctAnswer + "\n");
		}
	}
	Text CreateText(Vector3 pos, string txt, Color color)
    {
        GameObject go = new GameObject("text");
        go.name = txt;
        go.transform.SetParent(GameObject.Find("Canvas").transform);
		go.transform.eulerAngles = Vector3.zero;
        go.transform.position = pos;
        go.transform.localScale = new Vector3(.02f, .02f, .02f);
		RectTransform rect = go.GetComponent<RectTransform>();
        Font font = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        Text text = go.AddComponent<Text>();
        text.font = font;
        text.fontSize = 40;
        text.name = "." + txt + ".";
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
		text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.text = txt;
        return text;
    }
}
public class NNnode {
	public float correctAnswer;
	//public float guess;
	//public float error;
	public float output;
	public List<NNnode> inputNodes;
	public List<GameObject> links;
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
		go.transform.parent = global.parentNodes.transform;
		global.nodeLayers[layer].Add(this);
		index = global.nodeLayers[layer].Count - 1;
		go.name = layer + " " + index;
		inputNodes = new List<NNnode>();
		weights = new List<float>();
		links = new List<GameObject>();
		Update();
		global.cntNodes++;
	}
	public void Update() {
		UpdatePos();
		UpdateColor();
	}
	public void UpdatePos() {
		go.transform.position = GetPos();
	}
	Vector3 GetPos() {
		int numY = (int) Mathf.Sqrt(global.numInputs);
		float xStride = 10;
        float yStride = 2;
		float zStride = 0;
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
		ActivateTraditional();
//		ActivateMatrix();
	}
	public void ActivateTraditional()
    {
        for (int n = 0; n < inputNodes.Count; n++)
        {
            float value = inputNodes[n].output * weights[n];
            output += value;
        }
        //      output += bias;
    }
	public void ActivateMatrix()
    {
        for (int n = 0; n < inputNodes.Count; n++)
        {
            float value = inputNodes[n].output * weights[n];
            output += value;
        }
        //      output += bias;
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
		Debug.Log("epoch:" + global.epoch + " inputName:" + global.inputNames[n] + "\n");
//		global.LoadInputImage(n);
		global.LearnImage();
		global.ProcessImage();
		global.startTime = Time.realtimeSinceStartup;
        global.epoch++;
		n++;
		if (n >= global.inputNames.Count)
        {
            n = 0;
        }
    }
}
