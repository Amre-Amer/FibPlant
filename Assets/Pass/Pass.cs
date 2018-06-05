using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pass : MonoBehaviour {
	[Range(0, 120)]
	public int fps;
	PassGlobal global;
	[Range(0f, 10f)]
	public float distNear = 2.1f;
	int cntFps;
	// Use this for initialization
	void Start () {
		global = new PassGlobal();
		InvokeRepeating("ShowFps", 1, 1);
	}
	
	// Update is called once per frame
	void Update () {
		global.distNear = distNear;
		global.Update();
		cntFps++;
	}
	void ShowFps() {
		fps = cntFps;
		name = "Pass fps(" + fps + ")";
		cntFps = 0;
	}
}
