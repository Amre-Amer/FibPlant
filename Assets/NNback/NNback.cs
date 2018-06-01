using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNback : MonoBehaviour {
	int numInputs = 3;
	int numLayers = 3;
	int[] layerNodeCounts;
	float startTime;
	float delay;
	bool ynStep;
	int numWeightLayers = 2;
	List <float>[] weightLayers;
	List<float>[] nodeLayers;
	int numNodeLayers = 3;
	List<float>[] outputLayers;
	int numOutputLayers = 3;
	// Use this for initialization
	void Start () {
		weightLayers = new List<float>[numWeightLayers];
		nodeLayers = new List<float>[numNodeLayers];
		outputLayers = new List<float>[numOutputLayers];
		layerNodeCounts = new int[numLayers];
		layerNodeCounts[0] = 3;
		layerNodeCounts[1] = 2;
		layerNodeCounts[2] = 1;
		for (int layer = 0; layer < numLayers; layer++)
        {
			List<float> nodes = new List<float>();
			nodeLayers[layer] = nodes;
			List<float> outputs = null;
			outputs = new List<float>();
			outputLayers[layer] = outputs;
			for (int n = 0; n < layerNodeCounts[layer]; n++)
	    	{
				if (layer == 0)
				{
					float value = n + 1;
					nodes.Add(value);
					outputs.Add(value);
				} else {
					outputs.Add(-1);
				}
				if (layer == 1)
                {
					nodes.Add(3f);
                }
				if (layer == 2)
                {
                    nodes.Add(3f);
                }
				if (layer > 0)
				{
					List<float> weights = new List<float>();
					weightLayers[layer - 1] = weights;
					for (int w = 0; w < layerNodeCounts[layer - 1]; w++)
					{
//						if (layer == 1 && w == 0) weights.Add(.5f);
						weights.Add(.5f);
//						weights.Add(Random.Range(0f, 1f));
					}
				}
			}
		}
		//ForwardFeed();
		Show();
	}
	void LoadValues() {
		
	}
	void loadWeights() {
		
	}
	void ForwardFeed() {
		for (int layer = 1; layer < numLayers; layer++) {
			List<float> nodes = nodeLayers[layer];
			List<float> weights = weightLayers[layer - 1];
			List<float> outputs = outputLayers[layer - 1];
			for (int n = 0; n < nodes.Count; n++) {
				float output = 0;
				int numWeights = weights.Count / nodes.Count;
				int wStart = n * numWeights;
				for (int w = wStart; w < numWeights; w++) {
					float weight = weights[w];
					float value = outputs[w];
					output += weight * value;
				}
				outputs[n] = output;
			}            			
		}	
	}
	void Show() {
		for (int layer = 0; layer < numLayers; layer++)
        {
			Debug.Log("............layer:" + layer + "\n");
			List<float> nodes = nodeLayers[layer];
			List<float> weights = null;
			List<float> outputs = outputLayers[layer];
			for (int n = 0; n < nodes.Count; n++)
            {
				Debug.Log("node:" + nodes[n] + " output:" + outputs[n] + "\n");
				if (layer > 0)
				{
					weights = weightLayers[layer - 1];
					int numWeights = weights.Count / nodes.Count;
					Debug.Log(".." + numWeights + "\n");
                    int wStart = n * numWeights;
					for (int w = wStart; w < numWeights; w++)
					{
						Debug.Log("layer:" + layer + " n:" + n + " w:" + w + " weight:" + weights[w] + "\n");
					}
				}
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (ynStep == true && Time.realtimeSinceStartup - startTime < delay) {
			return;
		}
		//Show();
	}
}
