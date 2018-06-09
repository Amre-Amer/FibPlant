using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interval : MonoBehaviour {
	int numNodes = 16;
	GameObject[] nodeGos;
	int[] hitCount;
	int[] matchCount;
	int[] intervalCountSinceLastHit;
	int cntIntervals;
	[Range(.1f, .9f)]
	public float smooth = .95f;
	int cntFrames;
	int input;
	int cntEpochs;
	GameObject inputGo;
	int matchMin;
    int matchMax;
	int[] inputHistory;
	public bool yn1;
	public bool yn2;
	public bool yn3;
	public bool yn4;
	public bool yn5;
	int cntYnLast;
	GameObject[] inputHistoryGos;

	// Use this for initialization
	void Start () {
		inputGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
		hitCount = new int[numNodes];
		matchCount = new int[numNodes];
		intervalCountSinceLastHit = new int[numNodes];
		inputHistory = new int[numNodes];
		inputHistoryGos = new GameObject[numNodes];
		nodeGos = new GameObject[numNodes];
		CreateNodes();
		CreateInputHistoryGos();
	}

	void CreateInputHistoryGos() {
		for (int n = 0; n < numNodes; n++) {
			inputHistoryGos[n] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			inputHistoryGos[n].transform.position = new Vector3(-(n + 2), 0, 0);
			inputHistoryGos[n].transform.localScale = new Vector3(.1f, .1f, .1f);
		}		
	}
	void UpdateIputHistoryGos() {
		float smooth2 = smooth * .5f;
		for (int n = 0; n < numNodes; n++) {
			float vNew = inputHistory[n];
			if (vNew == 0) vNew = .1f;
			float vExisting = inputHistoryGos[n].transform.localScale.y;
            float v = vExisting * smooth2 + vNew * (1f - smooth2);
			inputHistoryGos[n].transform.localScale = new Vector3(v, v, v);
        }       
	}

	void AddHistory() {
//		if (cntEpochs < numNodes) {
			AdvanceHistory();			
//		}
		inputHistory[0] = input;
//		Debug.Log("input:" + input + "\n");
	}
	void AdvanceHistory() {
		int last = numNodes;
		if (cntEpochs < numNodes) {
			last = cntEpochs;
		}
		for (int n = last - 1; n > 0; n--) {
			inputHistory[n] = inputHistory[n - 1];
			inputHistoryGos[n].transform.localScale = new Vector3(.5f, .5f, .5f);
		}		
	}
	// Update is called once per frame
	void Update () {
		if (cntFrames % 10 == 0)
        {
			if (cntEpochs > 0) {
				AddHistory();
			}
			InputValue();
    		UpdateValues();
			Show();
			cntEpochs++;
		}
		cntFrames++;
		UpdateNodes();
		UpdateIputHistoryGos();
	}

	void Show() {
		string txtInput = "0";
		if (input == 1) {
			txtInput = "1";
		}
		string txt = "input:" + txtInput + " = ";
		for (int n = 0; n < hitCount.Length; n++)
        {
			string s1 = "";
			string s2 = "";
			if (hitCount[n] > 1 && intervalCountSinceLastHit[n] == 0) {
				s1 = "[";
				s2 = "]";
			}
			txt += "         |         " + s1 + hitCount[n] + "," + matchCount[n] + "," + intervalCountSinceLastHit[n] + s2;
        }
//        Debug.Log(txt + "\n");
	}

	void InputValue()
    {
		input = 0;
		if (cntEpochs % 1 == 0 && yn1 == true)
        {
            input = 1;
        }
		if (cntEpochs % 2 == 0 && yn2 == true) {
			input = 1;
		}
		if (cntEpochs % 3 == 0 && yn3 == true)
        {
            input = 1;
        }
		if (cntEpochs % 4 == 0 && yn4 == true)
        {
            input = 1;
        }
		if (cntEpochs % 5 == 0 &&  yn5 == true)
        {
            input = 1;
        }
		float y = 0;
		Color color = Color.red;
		if (input == 1) {
			y = 1;
			color = Color.green;
		}
		inputGo.transform.position = new Vector3(-1, y / 2, 0);
		inputGo.transform.localScale = new Vector3(.5f, y, .5f);
		inputGo.GetComponent<Renderer>().material.color = color;
		int cntYn = 0;
		if (yn1 == true) cntYn++; 
		if (yn2 == true) cntYn++;
		if (yn3 == true) cntYn++;
		if (yn4 == true) cntYn++;
		if (yn5 == true) cntYn++;
		if (cntYn != cntYnLast) {
			Debug.Log("clear:" + cntYn + "\n");
			ClearMatchCounts();
		}
		cntYnLast = cntYn;
    }

	void Activate(int n)
    {
        int interval = n;
        if (input == 1)
        {
            if (hitCount[n] > 0)
            {
                if (intervalCountSinceLastHit[n] == interval)
                {
                    hitCount[n]++;
                    matchCount[n]++;
                    intervalCountSinceLastHit[n] = 0;
                }
                else
                {
                    //hitCount[n] = 0;
                    //matchCount[n] = 0;
                    intervalCountSinceLastHit[n]++;
                }
            }
            else
            {
                hitCount[n] = 1;
                matchCount[n] = 0;
                intervalCountSinceLastHit[n] = 0;
            }
        }
        else
        {
            if (hitCount[n] > 0)
            {
                if (intervalCountSinceLastHit[n] == interval)
                {
                    hitCount[n] = 0;
                    matchCount[n] = 0;
                    intervalCountSinceLastHit[n] = 0;
                }
                else
                {
                    intervalCountSinceLastHit[n]++;
                }
            }
        }
    }

	void UpdateValues() {
		for (int n = 0; n < numNodes; n++)
		{
			Activate(n);
		}
		cntIntervals++;
	}

	bool IsSignalAcquired(int n) {
		return hitCount[n] > 0;
	}

	bool IsIntervalInPhase(int n) {
		return intervalCountSinceLastHit[n] == n;
	}

	float Sigmoid(float f) {
		return 1 / (1 + Mathf.Exp(-f));
	}

	void CreateNodes() {
        for (int n = 0; n < numNodes; n++) {
			nodeGos[n] = GameObject.CreatePrimitive(PrimitiveType.Cube);
            nodeGos[n].transform.position = new Vector3(n, 0, 0);
        }       
	}

	void UpdateNodes() {
		CalcMatchCountMinMax();
		int range = matchMax - matchMin;
		for (int n = 0; n < numNodes; n++)
        {
			float vNew = 0;
			if (range > 0) {
				vNew = (matchCount[n] - matchMin) / (float)range;
			}
			vNew *= 5;
            float vExisting = nodeGos[n].transform.localScale.y;
            float v = vExisting * smooth + vNew * (1f - smooth);
            nodeGos[n].transform.localScale = new Vector3(.5f, v, .5f);
			nodeGos[n].transform.position = new Vector3(n, v / 2, 0);
			nodeGos[n].GetComponent<Renderer>().material.color = new Color(v, v, v);
        }
	}
	void CalcMatchCountMinMax()
    {
		matchMin = 0;
		matchMax = 0;
        for (int n = 0; n < numNodes; n++)
        {
            int cnt = hitCount[n];
            if (n == 0 || cnt < matchMin)
            {
                matchMin = cnt;
            }
            if (n == 0 || cnt > matchMax)
            {
                matchMax = cnt;
            }
        }
    }
	void ClearMatchCounts() {
		for (int n = 0; n < numNodes; n++)
		{
			matchCount[n] = 0;
		}
	}
}
