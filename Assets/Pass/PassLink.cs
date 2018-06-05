using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassLink {
	public PassNode nodeFrom;
	public PassNode nodeTo;
	public GameObject go;
	public PassGlobal global;
	public PassLink(PassNode nodeFrom0, PassNode nodeTo0, PassGlobal global0) {
		global = global0;
		nodeFrom = nodeFrom0;
		nodeTo = nodeTo0;
		CreateGo();
	}
	public void CreateGo() {
		go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.parent = global.parentLinks.transform;
		global.UpdateCntLinks(1);
        Vector3 posFrom = nodeFrom.go.transform.position;
        Vector3 posTo = nodeTo.go.transform.position;
        float dist = Vector3.Distance(posFrom, posTo);
        go.transform.position = (posFrom + posTo) / 2;
        go.transform.LookAt(posTo);
		float s = .05f;
		float h = 3;
		float sx = s;
		float sy = s;
		int nFrom = int.Parse(nodeFrom.go.name);
		int nTo = int.Parse(nodeTo.go.name);
		Color color = Color.black;
		if (nFrom > nTo) {
			sx = s * h;
			sy = s;
			color = Color.red;
		} else {
			sx = s;
            sy = s * h;
			color = Color.blue;
		}
        go.transform.localScale = new Vector3(sx, sy, dist);
		go.name = nodeFrom.go.name + " links " + nodeTo.go.name;
		go.GetComponent<Renderer>().material.color = color;
	}
}
