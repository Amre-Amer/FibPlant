using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pass : MonoBehaviour {
	[Range(0, 120)]
	public int fps;
	PassGlobal global;
	[Range(0f, 10f)]
	public float distNear = 1.1f;
	int cntFps;
	bool ynStep = true;
	float delay = .5f;
	float startTime;
	int epoch;

	// Use this for initialization
	void Start () {
		global = new PassGlobal();
		InvokeRepeating("ShowFps", 1, 1);
		InvokeRepeating("Pump", 2, 3f);
	}
	
	// Update is called once per frame
	void Update () {
		if (ynStep == true && Time.realtimeSinceStartup - startTime > delay) {
			//Debug.Log("epoch:" + epoch + "\n");
			startTime = Time.realtimeSinceStartup;
			global.distNear = distNear;
            global.Update();
			epoch++;
		}
		cntFps++;
	}
	void UpdateX()
    {
        global.distNear = distNear;
        global.Update();
        cntFps++;
    }
	void Pump() {
		int n = Random.Range(0, global.nodes.Count);
		global.nodes[n].UpdateValue(1);
	}
	void ShowFps() {
		fps = cntFps;
		name = "Pass fps(" + fps + ")";
		cntFps = 0;
	}
}
