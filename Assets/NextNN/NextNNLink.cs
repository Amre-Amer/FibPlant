using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextNNLink {
	public NextNNNode nodeFrom;
	public NextNNNode nodeTo;
	public GameObject go;
	public NextNNGlobal global;
	public NextNNLink(NextNNNode nodeFrom0, NextNNNode nodeTo0, NextNNGlobal global0) {
		nodeFrom = nodeFrom0;
		nodeTo = nodeTo0;
		global = global0;
		CreateLink();
	}
	public void CreateLink()
    {
        Vector3 posFrom = nodeFrom.go.transform.position;
        Vector3 posTo = nodeTo.go.transform.position;
        go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.parent = global.parentLinks.transform;
        go.name = "link " + nodeFrom.go.name + " to " + nodeTo.go.name;
        go.transform.position = (posFrom + posTo) / 2;
        go.transform.LookAt(posTo);
        float dist = Vector3.Distance(posFrom, posTo);
        float maxS = global.scaleNode * .5f;
        float minS = global.xScaleLink;
        float maxDist = global.distNear;
        float minDist = global.scaleNode * .5f;
        float fraction = (dist - minDist) / (maxDist - minDist);
        float s = minS + (1f - fraction) * (maxS - minS);
        go.transform.localScale = new Vector3(s, s, dist);
//		nodeFrom.links.Add(this);
    }
}
