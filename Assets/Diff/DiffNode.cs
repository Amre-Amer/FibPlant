using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiffNode {
	public DiffGlobal global;
    public int layer;
	public int index;
	public GameObject go;
	public GameObject goLink;
	public DiffNode nodeFrom;
	public float value;
	public DiffNode(int layer0, DiffGlobal global0) {
		layer = layer0;
		global = global0;
		List<DiffNode> nodeLayer = global.nodeLayers[layer];
		nodeLayer.Add(this);
		index = nodeLayer.Count - 1;
		CreateGameObject();
		global.cntNodes++;
	}
	public Vector3 GetPos() {
		float yOffset = (global.layerNodeCounts[0] - global.layerNodeCounts[layer]) * global.yStride / 2f;
		float x = global.xStride * layer;
        float y = yOffset + global.yStride * index;
        float z = 0;
		return new Vector3(x, y, z);
	}
	public void CreateGameObject() {
		if (global.ynUseGos == true)
		{
			go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			go.transform.parent = global.parentTmpGo.transform;
			go.transform.position = GetPos();
			go.transform.eulerAngles = new Vector3(90, 0, 0);
			go.name = "layer:" + layer + " index:" + index;
		}
	}
	public void CreateLink() {
		if (global.ynUseGos == true)
		{
			if (nodeFrom != null)
			{
				Vector3 posFrom = GetPos();
				Vector3 posTo = nodeFrom.GetPos();
				goLink = GameObject.CreatePrimitive(PrimitiveType.Cube);
				goLink.transform.parent = global.parentTmpGo.transform;
				goLink.name = "link " + go.name + " from " + nodeFrom.go.name;
				goLink.transform.position = (posFrom + posTo) / 2;
				float dist = Vector3.Distance(posFrom, posTo);
				goLink.transform.LookAt(posTo);
				goLink.transform.localScale = new Vector3(.1f, .1f, dist);
			}
		}
	}
	public void UpdateValue(float value0) {
		value = value0;
		Color color = new Color(value, value, value);
		if (global.ynUseGos == true)
		{
			go.GetComponent<Renderer>().material.color = color;
		}
		//
		Vector3 pos = GetPos();
		global.imageValues.SetPixel((int) pos.x, (int) pos.y, color);
		//global.imageValues.Apply();
	}
}
