using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiffGlobal : MonoBehaviour {
	public int cntNodes;
	public float xStride = 3; 
	public float yStride = 2; 
	public List<List<DiffNode>> nodeLayers;
	public List<int> layerNodeCounts;
	public int numInputs;
	Texture2D texture;
	float[] data;
	List<string>filenames;
	string filename;
	public GameObject[,] textureGos;
	public GameObject parentTmpGo;
	public GameObject parentGo;
	public Texture2D imageValues;
	public GameObject imageValuesGo;
	public Texture2D inputImage;
    public GameObject inputImageGo;
	public bool ynUseGos = false;
	public DiffGlobal(List<string> filenames0) {
		filenames = filenames0;
	}
	public void FeedForwardImage(int n) {
		cntNodes = 0;
		float t1 = Time.realtimeSinceStartup;
		nodeLayers = new List<List<DiffNode>>();
        filename = filenames[n];
        LoadImage();
        ShowTexture();
        GenerateNodeCounts();
        InitImageValues();
        CreateNodes();
        LoadInputs();
        FeedForward();
        float t2 = Time.realtimeSinceStartup - t1;
		Debug.Log(n + " FeedForwardImage:" + filename + " cntNodes:" + cntNodes + " time:" + t2.ToString("F2") + "\n");
	}
	public void InitImageValues() {
		if (imageValuesGo == null)
        {
            imageValuesGo = GameObject.CreatePrimitive(PrimitiveType.Quad);
            imageValuesGo.name = "imageValuesGo";
            float w = layerNodeCounts.Count * xStride;
            float h = layerNodeCounts[0] * yStride;
            imageValues = new Texture2D((int)w, (int)h);
            imageValuesGo.transform.localScale = new Vector3(w, h, 0);
            float x = w / 2;
            float y = h / 2;
            imageValuesGo.transform.position = new Vector3(x, y, 5);
			imageValuesGo.GetComponent<Renderer>().material.mainTexture = imageValues;
		}
	}
	public void FeedForward() {
		for (int layer = 1; layer < layerNodeCounts.Count; layer++) {
			List<DiffNode> nodeLayer = nodeLayers[layer];
			int numNodes = nodeLayer.Count;
            for (int n = 0; n < numNodes; n++)
            {
				DiffNode node = nodeLayer[n];
                DiffNode nodeA = nodeLayers[layer - 1][n];
				float valueA = nodeA.value;
				DiffNode nodeB = nodeLayers[layer - 1][n + 1];
                float valueB = nodeB.value;
				float value = 0;
				if (IsDifferent(valueA, valueB) == true) {
					value = 1;
				}
				node.UpdateValue(value);
            }
		}		
		imageValues.Apply();
	}
	public bool IsDifferent(float a, float b) {
		if (Mathf.Abs(a - b) < .001f) {
			return false;
		} else {
			return true;
		}
	}
	public void ShowTexture() {
		float xOffset = -texture.width * 1.1f;
		if (ynUseGos == true)
		{
			if (textureGos == null)
			{
				parentGo = new GameObject("parentGo");
				textureGos = new GameObject[texture.width, texture.height];
			}
			for (int i = 0; i < texture.width; i++)
			{
				for (int j = 0; j < texture.height; j++)
				{
					float x = xOffset + i;
					float y = j;
					float z = 0;
					if (textureGos[i, j] == null)
					{
						GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
						go.transform.parent = parentGo.transform;
						textureGos[i, j] = go;
					}
					textureGos[i, j].transform.position = new Vector3(x, y, z);
					textureGos[i, j].GetComponent<Renderer>().material.color = texture.GetPixel(i, j);
				}
			}
		}
		float s = 4;
		if (inputImageGo == null) {
			inputImageGo = GameObject.CreatePrimitive(PrimitiveType.Quad);
			inputImageGo.name = "inputImageGo";
			xOffset = texture.width * s * -1;
			float yOffset = texture.height * s;
			inputImageGo.transform.position = new Vector3(xOffset, yOffset, -5);
		}
		inputImageGo.transform.localScale = new Vector3(texture.width * s, texture.height * s, 0);
		inputImageGo.GetComponent<Renderer>().material.mainTexture = texture;        
	}
	public void LoadImage() {
		texture = Resources.Load<Texture2D>(filename);
		numInputs = texture.width * texture.height;
//		Debug.Log("numInputs:" + numInputs + "\n");
	}
	void GenerateNodeCounts()
    {
        layerNodeCounts = new List<int>();
        for (int n = numInputs; n > 0; n--)
        {
            layerNodeCounts.Add(n);
        }
    }
	public void CreateNodes() {
		if (ynUseGos == true)
		{
			if (parentTmpGo != null)
			{
				DestroyImmediate(parentTmpGo);
			}
			parentTmpGo = new GameObject("parentTmpGos");
		}
		for (int layer = 0; layer < layerNodeCounts.Count; layer++) {
			List<DiffNode> nodeLayer = new List<DiffNode>();
            nodeLayers.Add(nodeLayer);
			int numNodes = layerNodeCounts[layer];
			for (int n = 0; n < numNodes; n++)
			{
				DiffNode node = new DiffNode(layer, this);
				if (layer > 0) {
					DiffNode nodeChild = nodeLayers[layer - 1][n];
					nodeChild.nodeFrom = node;
					nodeChild.CreateLink();
                    //
					DiffNode nodeChild2 = nodeLayers[layer - 1][n + 1];
                    nodeChild2.nodeFrom = node;
                    nodeChild2.CreateLink();
				}
			}
		}
	}
	public void LoadInputs() {
		LoadDataWithImageGrayScales();
		LoadInputLayerWithData();
	}
	public void LoadDataWithImageGrayScales() {
		Color[] colors = texture.GetPixels();
		data = new float[colors.Length];
		for (int n = 0; n < colors.Length; n++) {
			data[n] = colors[n].grayscale;
		}
	}
	public void LoadInputLayerWithData() {
		List<DiffNode> nodeLayer = nodeLayers[0];
		int numNodes = nodeLayer.Count;
		if (numNodes == data.Length)
		{
			for (int n = 0; n < numNodes; n++)
			{
				DiffNode node = nodeLayer[n];
				float value = data[n];
				node.UpdateValue(value);
			}
			imageValues.Apply();
		} else {
			Debug.Log("LoadInput: data:" + data.Length + " / input:" + numNodes + " count mismatch!\n");
		}
	}
}
