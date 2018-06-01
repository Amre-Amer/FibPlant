using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NN2Node
{
	public float weight;
    public int layer;
    public int index;
    public float output;
	public NN2Node outputNode;
	public List<NN2Node> inputNodes;
    public List<float> inputNodeWeights;
    public NN2Global global;
    public NN2Node(int layer0, NN2Global global0)
    {
		layer = layer0;
        global = global0;
		inputNodes = new List<NN2Node>();
        inputNodeWeights = new List<float>();
		global.nodes[layer].Add(this);
		index = global.nodes[layer].Count - 1;
    }
}
