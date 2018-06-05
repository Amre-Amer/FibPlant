using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassNode {
	public PassGlobal global;
	public GameObject go;
	public Vector3 posLast;
	public List<PassLink> links;
	public int index;
	public float distNearLast;
	public PassNode(Vector3 pos0, PassGlobal global0) {
		global = global0;
		CreateGo(pos0);
	}
	public void CreateGo(Vector3 pos0) {
		go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.parent = global.parentNodes.transform;
		global.UpdateCntNodes(1);
        go.transform.position = pos0;
        go.transform.localScale = new Vector3(.25f, .25f, .25f);
		global.nodes.Add(this);
		index = global.nodes.Count - 1;
		go.name = index.ToString();
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
	public void Update() {
		if (go.transform.position != posLast || global.distNear != distNearLast) {
			CreateLinks();
		}
		posLast = go.transform.position;
		distNearLast = global.distNear;
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
