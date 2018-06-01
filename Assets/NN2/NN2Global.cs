using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NN2Global
{
	int numInputs = 10 * 15;
	int numLayers = 3;
	float learningRate = .1f;
	float correctAnswer = 6;
	float weightCorrectionScaleFactor = 1;
	float error;
	float answer;
    public List<List<NN2Node>> nodes;
    public NN2Global()
    {
        nodes = new List<List<NN2Node>>();
		CreateNodes();
//		Show();
		ForwardFeed();
//		Show();
		//
		for (int epoch = 0; epoch < 20; epoch++)
		{
			CalculateCorrection();
			BackPropagate();
			ForwardFeed();
//			Show();
		}
//		Show();
    }
	public void CreateNodes() {
		int[] layerNodeCounts = new int[] { numInputs, 2, 1};
		for (int layer = 0; layer < numLayers; layer++) {
			List<NN2Node> layerNodes = new List<NN2Node>();
			nodes.Add(layerNodes);
			for (int n = 0; n < layerNodeCounts[layer]; n++)
			{
				NN2Node node = new NN2Node(layer, this);
			}
		}
		LoadInputs();
		LoadWeights();
		CreateLinks();
	}
	public void CalculateCorrection() {
		List<NN2Node> outputLayerNodes = nodes[nodes.Count - 1];
        NN2Node nodeAnswer = outputLayerNodes[outputLayerNodes.Count - 1];
		answer = nodeAnswer.output;
		error = correctAnswer - answer;
		weightCorrectionScaleFactor = (error * learningRate + answer) / answer;
		Debug.Log("CalculateCorrection correctAnswer:" + correctAnswer + " -  answer:" + answer +  " = error:" + error + " weightCorrectionScaleFactor:" + weightCorrectionScaleFactor + "\n");
	}
	public void BackPropagate() {
		for (int layer = 1; layer < numLayers; layer++)
        {
            List<NN2Node> layerNodes = nodes[layer];
            for (int n = 0; n < layerNodes.Count; n++)
            {
                NN2Node node = layerNodes[n];
                List<NN2Node> layerNodeBelow = nodes[layer - 1];
                foreach (NN2Node nodeBelow in layerNodeBelow)
                {
					nodeBelow.weight *= weightCorrectionScaleFactor;
                }
            }
        }
	}
	public void ForwardFeed() {
		for (int layer = 1; layer < numLayers; layer++)
        {
            List<NN2Node> layerNodes = nodes[layer];
            for (int n = 0; n < layerNodes.Count; n++)
            {
                NN2Node node = layerNodes[n];
				node.output = 0;
                List<NN2Node> layerNodeBelow = nodes[layer - 1];
                foreach (NN2Node nodeBelow in layerNodeBelow)
                {
					node.output += nodeBelow.output * nodeBelow.weight;
                }
            }
        }
	}
	public void LoadWeights() {
		for (int layer = 1; layer < numLayers; layer++)
        {
            List<NN2Node> layerNodes = nodes[layer];
            for (int n = 0; n < layerNodes.Count; n++)
            {
                NN2Node node = layerNodes[n];
                List<NN2Node> layerNodeBelow = nodes[layer - 1];
                foreach (NN2Node nodeBelow in layerNodeBelow)
                {
					nodeBelow.weight = .5f;
                }
            }
        }
	}
	public void LoadInputs() {
		string filename = "line_0";
		Texture2D texture = Resources.Load<Texture2D>(filename);
		Color[] colors = texture.GetPixels();
		for (int c = 0; c < colors.Length; c++) {
			float value = colors[c].grayscale;
			nodes[0][c].output = value;
		}
		//nodes[0][0].output = 1;
		//nodes[0][1].output = 2;
		//nodes[0][2].output = 3;
	}
	public void CreateLinks() {
		for (int layer = 1; layer < numLayers; layer++)
        {
			List<NN2Node> layerNodes = nodes[layer];
			for (int n = 0; n < layerNodes.Count; n++)
            {
				NN2Node node = layerNodes[n];
				List<NN2Node> layerNodeBelow = nodes[layer - 1];
				foreach(NN2Node nodeBelow in layerNodeBelow) {
					node.inputNodes.Add(nodeBelow);
					nodeBelow.outputNode = node;
				}
            }
        }
	}
	public void Show() {
		Debug.Log("nodes.....\n");
		for (int layer = 0; layer < nodes.Count; layer++)
		{
			foreach (NN2Node node in nodes[layer])
			{
				Debug.Log("layer:" + node.layer + " index:" + node.index + " output:" + node.output + " weight:" + node.weight + "\n");
			}
		}
	}
}
