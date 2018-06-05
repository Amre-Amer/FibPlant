using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextNNNode {
	public float value;
	public int layer;
	public int indexX;
	public int indexY;
	public NextNNGlobal global;
	public List<NextNNNode> nearNodes;
	public List<NextNNLink> links;
	public int index;
	public GameObject go;
	public Vector3 posLast;
	public float distNearLast;
	public NextNNNode(int layer0, int indexX0, int indexY0, NextNNGlobal global0) {
		layer = layer0;
		indexX = indexX0;
		indexY = indexY0;
		global = global0;
		global.nodes.Add(this);
		index = global.nodes.Count - 1;
		UpdateGo();
		UpdateLinks();
	}	
	public void UpdateGo() {
		if (go == null)
		{
			go = GameObject.CreatePrimitive(PrimitiveType.Cube);
			go.transform.parent = global.parentNodes.transform;
			go.name = "layer:" + layer + " x:" + indexX + " y:" + indexY;
		}
		float xOffset = 0;
        float yOffset = 0;
		bool ynPyramid = true;
		if (ynPyramid == true) {
			xOffset = layer * .5f;
            yOffset = layer * .5f;
		}
		float x = xOffset + indexX * global.xStride;
		float y = yOffset + indexY * global.yStride;
		float z = layer * global.zStride;
		go.transform.position = new Vector3(x, z, y);
		go.transform.localScale = new Vector3(global.scaleNode, global.scaleNode, global.scaleNode);
		float c = layer / (float) global.numLayers;
		go.GetComponent<Renderer>().material.color = new Color(c, c, c);
	}
	public void Update() {
		UpdateLinks();
		FeedForward();
	}
	public void FeedForward()
    {
//		Debug.Log("FeedForward...\n");
		if (value > 0) {
			float sumLinkScales = GetSumLinkScales();
//			Debug.Log("FeedForward value > 0 links:" + links.Count + "\n");
			for (int n = 0; n < links.Count; n++)
            {
                NextNNLink link = links[n];
				float fraction = link.go.transform.localScale.x / sumLinkScales;
				fraction = 1f / links.Count;
				NextNNNode node = links[n].nodeTo;
				float portion = fraction * value;
				Debug.Log("portion:" + portion + "\n");
				node.value = portion;
				node.UpdateValueColor();
            }
			value = 0; //value * .85f;
			UpdateValueColor();
		}
    }
	public float GetSumLinkScales() {
		float sumLinkScales = 0;
        for (int n = 0; n < links.Count; n++)
        {
            NextNNLink link = links[n];
            sumLinkScales += link.go.transform.localScale.x;
        }
		return sumLinkScales;
	}
	public void UpdateLinks() {
		if (go.transform.position != posLast || global.distNear != distNearLast)
		{
			ClearLinks();
			ClearNearNodes();
			nearNodes = new List<NextNNNode>();
			links = new List<NextNNLink>();
			for (int n = 0; n < global.nodes.Count; n++)
			{
				if (n != index)
				{
					NextNNNode node = global.nodes[n];
					float dist = Vector3.Distance(go.transform.position, node.go.transform.position);
					if (dist < global.distNear)
					{
						nearNodes.Add(node);
						NextNNLink linkExisting = DoesLinkAlreadyExist(node);
						if (linkExisting != null)
						{
							links.Add(linkExisting);
						} else {
                            NextNNLink link = new NextNNLink(this, node, global);
                            links.Add(link);
                            global.linksAll.Add(link);
						}
					}
				}
			}
		}
		//posLast = go.transform.position;
		//distNearLast = global.distNear;
//		FeedForward();
	}
	public NextNNLink DoesLinkAlreadyExist(NextNNNode node) {
		for (int n = 0; n < global.linksAll.Count; n++) {
			NextNNLink link = global.linksAll[n];
			if (link.nodeFrom == node && link.nodeTo == this) {
				return link;
			}
			if (link.nodeTo == node && link.nodeFrom == this)
            {
                return link;
            }
		}
		return null;
	}
	public void ClearNearNodes() {
		if (nearNodes != null)
		{
			nearNodes.Clear();
		}
	}
	public void ClearLinks() {
		if (links != null)
		{
			for (int n = 0; n < links.Count; n++) {
				global.linksAll.Remove(links[n]);
				global.DestroyGameObject(links[n].go);
			}
			links.Clear();
		}
	}
	public void UpdateValueColor() {
		Color color = new Color(value, 0, 0);
		go.GetComponent<Renderer>().material.color = color;
	}
}
