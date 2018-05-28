using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FibPlant : MonoBehaviour {
	FibPlantGlobal global;
	void Start () {
		global = new FibPlantGlobal();
		FibPlantNode node = new FibPlantNode(null, Vector3.zero, global);
	}
	void Update () {
		if (global.ynStep == true && Time.realtimeSinceStartup - global.startTime < global.delay) {
			return;
		}
		if (global.level < global.numLevels)
		{
			global.txtLevel = "";
			global.startTime = Time.realtimeSinceStartup;
			for (int n = 0; n < global.nodes.Count; n++)
			{
				if (global.nodes[n].active == true)
				{
					global.nodes[n].Advance();
				}
			}
			Debug.Log("level:" + global.level + " cnt:" + global.nodes.Count + " = " + global.txtLevel + "\n");
			global.level++;
		}
	}
}
public class FibPlantNode {
	FibPlantGlobal global;
    FibPlantNode fromNode;
	FibPlantNode advanceNode;
	FibPlantNode childNode;
	public bool active = true;
	public int age;
	public int index;
	public GameObject go;
	public GameObject goLink;
	public FibPlantNode(FibPlantNode fromNode0, Vector3 position, FibPlantGlobal global0) {
		fromNode = fromNode0;
        global = global0;
		global.nodes.Add(this);
		index = global.nodes.Count - 1;
		go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.position = position;
		UpdateName();
	}
	public void Advance() {
		//LeaveCopy();
		go.transform.position += Vector3.up * global.levelHeight;
		UpdateLink();
		if (age >= 2) {
			childNode = new FibPlantNode(this, new Vector3(1, 0, 0), global);
		}
		age++;
		UpdateName();
	}
	public void LeaveCopy() {
		advanceNode = new FibPlantNode(this, go.transform.position, global);
		advanceNode.active = false;
        advanceNode.age = age;
        advanceNode.UpdateName();
	}
	public void UpdateLink() {
		if (fromNode == null) {
			return;
		}
		if (goLink == null) {
			goLink = GameObject.CreatePrimitive(PrimitiveType.Cube);
			goLink.name = "link";
		}
		Debug.Log("update\n");
		Vector3 posFrom = fromNode.go.transform.position;
		Vector3 posTo = go.transform.position;
		float dist = Vector3.Distance(posFrom, posTo);
		goLink.transform.position = (posFrom + posTo) / 2;
		goLink.transform.LookAt(posTo);
		goLink.transform.localScale = new Vector3(.1f, .1f, dist);
	}
	public void UpdateName() {
		go.name = age.ToString();
        global.txtLevel += go.name + ", ";
	}
}
public class FibPlantGlobal {
	public List<FibPlantNode> nodes;
	public int level;
	public float startTime;
	public bool ynStep = true;
	public float delay = 1;
	public int numLevels = 2;
	public string txtLevel;
	public float levelHeight = 5;
	public FibPlantGlobal() {
		nodes = new List<FibPlantNode>();
		startTime = Time.realtimeSinceStartup;
		Debug.Log("FibPlant\n");
	}
}
