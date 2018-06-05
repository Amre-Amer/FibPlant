using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextNNGlobal : MonoBehaviour{
	public List<NextNNNode> nodes;
	public int numLayers;
	public int numX;
	public int numY;
	public float xStride = 1;
	public float yStride = 1;
	public float zStride = 1;
	public float xScaleLink = .025f;
	public float yScaleLink = .025f;
	public float scaleNode = .5f;
	public GameObject parentLinks;
	public GameObject parentNodes;
	public float distNear;
	public List<NextNNLink> linksAll;
	public int cntNodes;
	public int cntLinks;
	public NextNNGlobal(int numLayers0, int numX0, int numY0) {
		numLayers = numLayers0;
		numX = numX0;
		numY = numY0;
	}
	public void CreateNodes() {
		parentLinks = new GameObject("parentLinks");
		parentNodes = new GameObject("parentNodes");
		nodes = new List<NextNNNode>();
		linksAll = new List<NextNNLink>();
		for (int layer = 0; layer < numLayers; layer++)
		{
			int less = layer;
			for (int x = 0; x < numX - less; x++)
			{
				for (int y = 0; y < numY - less; y++)
				{
					NextNNNode node = new NextNNNode(layer, x, y, this);
				}
			}
		}
	}
	public void UpdateNodes() {
		foreach(NextNNNode node in nodes) {
			node.Update();
		}	
	}
	public void DestroyGameObject(GameObject go) {
		DestroyImmediate(go);
	}
}
