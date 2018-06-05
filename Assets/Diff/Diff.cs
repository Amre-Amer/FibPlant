using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diff : MonoBehaviour {
	DiffGlobal global;
	float startTime;
	bool ynDelay = true;
	float delay = .5f;
	int index;
	List<string> filenames;
	// Use this for initialization
	void Start () {
		//filenames = new List<string> {"face_happy", "face_normal", "face_sad"};
//		CreateIncrementingFilenames("line_", 0, 7);
		CreateIncrementingFilenames("size28_", 0, 9);
		global = new DiffGlobal(filenames);
		startTime = Time.realtimeSinceStartup;
		//global.FeedForwardImage(0);
	}
	
	// Update is called once per frame
	void Update () {
		if (ynDelay == true && (Time.realtimeSinceStartup - startTime) < delay) {
			return;
		}
		startTime = Time.realtimeSinceStartup;
		if (index >= filenames.Count)
		{
			index = 0;
		}
		global.FeedForwardImage(index);
		index++;
	}
	public void CreateIncrementingFilenames(string root, int startN, int endN) {
		filenames = new List<string>();
		for (int n = startN; n <= endN; n++) {
			filenames.Add(root + n);   
        }
	}
}
