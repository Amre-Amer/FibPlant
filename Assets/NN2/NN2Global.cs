using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NN2Global
{
	//int numInputs = 10 * 15;
	int numLayers = 3;
	float learningRate = .1f;
	float correctAnswer = 0;
	float weightCorrectionScaleFactorAve = 1;
	float errorSum;
	float errorAve;
	List<float> error;
	List<float> answer;
	List<float> answerAve;
	List<float> answerSum;
    public List<List<NN2Node>> nodes;
	string filename = "?";
	List <string> filenames;
	public int indexFilename = 0;
	public int numEpochs = 15;
    public NN2Global(int[] layerNodeCounts0)
    {
        nodes = new List<List<NN2Node>>();
		LoadDataSets();
		CreateNodes(layerNodeCounts0);
		RunTest();
		for (int epoch = 0; epoch < numEpochs; epoch++)
		{
			Debug.Log("Epoch:" + epoch + "---------------------------------\n");
			errorSum = 0;
			//answerSum = 0;
			for (int n = 0; n < filenames.Count; n++)
			{
				indexFilename = n;
				LoadInputs();
				ForwardFeed();
				CalcError();
				//errorSum += error;
				//answerSum += answer;
			}
			errorAve = errorSum / filenames.Count;
//			answerAve = answerSum / filenames.Count;
			CalcCorrectionAve();
			BackPropagate();
		}
		RunTest();
    }
	public void RunTest() {
		Debug.Log("Test...............\n");
		for (int n = 0; n < filenames.Count; n++)
        {
            indexFilename = n;
            LoadInputs();
            ForwardFeed();
			GetAnswer();
			Debug.Log("RunTest filename:" + filename + " correctAnswer:" + correctAnswer + " answer:" + answer + "\n");
        }
		Debug.Log("...............\n");
	}
	public void CreateNodes(int[] layerNodeCounts) {
		for (int layer = 0; layer < numLayers; layer++) {
			List<NN2Node> layerNodes = new List<NN2Node>();
			nodes.Add(layerNodes);
			for (int n = 0; n < layerNodeCounts[layer]; n++)
			{
				NN2Node node = new NN2Node(layer, this);
			}
		}
		LoadWeights();
		CreateLinks();
	}
	public void LoadDataSets() {
		filenames = new List<string>();
		for (int n = 0; n < 7; n++) {
			filenames.Add("line_" + n);
		}
	}
	public void LoadInputs()
    {
		filename = filenames[indexFilename];
        Texture2D texture = Resources.Load<Texture2D>(filename);
        Color[] colors = texture.GetPixels();
        for (int c = 0; c < colors.Length; c++)
        {
            float value = colors[c].grayscale;
            nodes[0][c].output = value;
        }
		LoadCorrectAnswer();
//		Debug.Log("LoadInputs filename:" + filename + " correctAnswer:" + correctAnswer + "\n");
    }
	public void LoadCorrectAnswer() {
		correctAnswer = float.Parse(filename.Substring(filename.Length - 1, 1));
	}
	public void GetAnswer() {
		List<NN2Node> outputLayerNodes = nodes[nodes.Count - 1];
		foreach(NN2Node nodeOutput in outputLayerNodes) {
			//answer[]
		}
        NN2Node nodeAnswer = outputLayerNodes[outputLayerNodes.Count - 1];
        //answer = nodeAnswer.output;
	}
	public void CalcError() {
		GetAnswer();
		//error = correctAnswer - answer;
	}
	public void CalcCorrectionAve() {
//		weightCorrectionScaleFactorAve = (errorAve * learningRate + answerAve) / answerAve;
		Debug.Log("CalculateCorrection correctAnswer:" + correctAnswer + " -  answerAve:" + answerAve +  " = errorAve:" + errorAve + " weightCorrectionScaleFactor:" + weightCorrectionScaleFactorAve + "\n");
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
					nodeBelow.weight *= weightCorrectionScaleFactorAve;
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
