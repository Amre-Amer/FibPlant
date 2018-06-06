using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassNode {
	public float value = 0;
	public PassGlobal global;
	public GameObject go;
	public Vector3 posLast;
	public List<PassLink> links;
	public int index;
	public float distNearLast;
	public float valueLast;
	public float scale = .25f;
	public PassNode(Vector3 pos0, PassGlobal global0) {
		global = global0;
		CreateGo(pos0);
	}
	public void CreateGo(Vector3 pos0) {
		go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.parent = global.parentNodes.transform;
		global.UpdateCntNodes(1);
        go.transform.position = pos0;
		go.transform.localScale = new Vector3(scale, scale, scale);
		global.nodes.Add(this);
		index = global.nodes.Count - 1;
		go.name = index.ToString();
		UpdateValue(value);
	}
	public void CreateLinks() {
		ClearLinks();
		foreach(PassNode node in global.nodes) {
			if (node != this) {
				float dist = Vector3.Distance(go.transform.position, node.go.transform.position);	
				if (dist <= global.distNear) {
					PassLink link = new PassLink(this, node, global);
					links.Add(link);
					global.AddNodeActivated(node);
				}
			}
		}
	}
	public void UpdateX() {
		if (go.transform.position != posLast || global.distNear != distNearLast) {
			CreateLinks();
			posLast = go.transform.position;
            distNearLast = global.distNear;
		}
		if (value != valueLast)
		{
			FeedForward();
			valueLast = value;
		}
	}
	public void Update()
    {
            CreateLinks();
            FeedForward();
    }
	public void FeedForward() {
		if (value > 0)
		{
			for (int n = 0; n < links.Count; n++)
			{
				PassNode node = links[n].nodeTo;
				float portion = value / links.Count;
				portion = value * .85f;
				node.value = portion;
			}
		}
//		value = 0;
		UpdateValue(value);
	}
	public void UpdateValue(float v) {
		value = v;
		Color color = new Color(v, v, v);
		go.GetComponent<Renderer>().material.color = color;
		float s = scale + v;
		go.transform.localScale = new Vector3(s, s, s);
	}
	public void ClearLinks() {
		if (links != null)
		{
			foreach (PassLink link in links)
			{
				global.DestroyLinkGo(link.go);
			}
		}
		links = new List<PassLink>();
	}
}
