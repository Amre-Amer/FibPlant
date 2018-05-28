using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balls : MonoBehaviour {
	string filename = "letter_a";
	Texture2D texture;
	Vector3[,] gridData;
	Vector3[,] gridWeights;
	GameObject[,] gridGos;
	GameObject[,] gridWeightGos;
	int correctAnswer = 65;
	int i;

	// Use this for initialization
	void Start () {
		//return;
//		texture = Resources.Load<Texture2D>(filename);
		//InitCorrectAnswer();
		//InitGridWeights();        
//		LoadData();
		Test("letter_a");
		Test("letter_d");
		Test("letter_c");
		Test("letter_e");
		Test("letter_b");
		Test("test_a");
		Test("test_b");
		//Advance();
		////
		//i = 1;
		//filename = "letter_b";
		//texture = Resources.Load<Texture2D>(filename);
        //InitCorrectAnswer();
        //InitGridWeights();
        //LoadData();
        //Advance();
	}
	void Test(string filename0) {
		filename = filename0;
		texture = Resources.Load<Texture2D>(filename);
		LoadData();
//		Debug.Log("Test\n");
		int numData = texture.width * texture.height;
		int[] testData = new int[numData];
		int n = 0;
		for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Color col = texture.GetPixel(x, y);
                if (col.r != 1f)
                {
					testData[n] = 1;
                }
                else
                {
					testData[n] = 0;
                }
				n++;
            }
        }
		int[] data = Analyze(testData);
		int s1 = SumSimilar(data);
		data = Analyze(data);
        int s2 = SumSimilar(data);
		Debug.Log(filename + " score:" + s1 + " " + s2 + "\n");
		//data = Analyze(data);
		//      s = SumSimilar(data);
		//      Debug.Log("3 sum:" + s + "\n");
		//data = Analyze(data);
		//s = SumSimilar(data);
		//Debug.Log("4 sum:" + s + "\n");
		i++;
	}
	int[] Analyze(int[] data) {
		int[] result = new int[data.Length - 1];
		for (int n = 0; n < data.Length - 1; n++)
        {
            if (data[n] == data[n + 1])
            {
				result[n] = 1;
            }
            else
            {
				result[n] = 0;
            }
        }
		return result;
	}
	int SumSimilar(int[] data) {
		int sum = 0;
		for (int n = 0; n < data.Length; n++)
        {
            if (data[n] == 1)
            {
                sum++;
                if (n > 0 && data[n - 1] == 1)
                {
                    sum++;
                }
            }
        }
		return sum;
	}
	void InitCorrectAnswer() {
		string lastChar = filename.Substring(filename.Length - 1, 1);
        correctAnswer = getASCIIforString(lastChar);
        //
		Debug.Log("Balls:" + texture.width + " x " + texture.height + "\n");
        Debug.Log("file:" + filename + "\n");
        Debug.Log("correctAnswer:" + correctAnswer + "\n");
        Debug.Log("char:" + getCharForASCII(correctAnswer) + "\n");
	}
	void InitGridWeights() {
		gridWeights = new Vector3[texture.width, texture.height];
		gridWeightGos = new GameObject[texture.width, texture.height];
		for (int x = 0; x < texture.width; x++) {
			for (int y = 0; y < texture.height; y++) {
				float wx = Random.Range(0f, 1f);
				float wy = Random.Range(0f, 1f);
				float wz = Random.Range(0f, 1f);
				gridWeights[x, y] = new Vector3(wx, wy, wz);        
				gridWeightGos[x, y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
				gridWeightGos[x, y].transform.position = new Vector3(texture.width * 1.2f + x * 1.1f, y * 1.1f, texture.width * i);
				gridWeightGos[x, y].GetComponent<Renderer>().material.color = new Color(wx, wy, wz);
            }       
		}		
	}
	void LoadData() {
		Vector3 sum = Vector3.zero;
		gridData = new Vector3[texture.width, texture.height];      
		gridGos = new GameObject[texture.width, texture.height];
		for (int x = 0; x < texture.width; x++) {
			for (int y = 0; y < texture.height; y++) {
				Color col = texture.GetPixel(x, y);
				//if (col.r != 1f) {
				//	col = Color.white;
				//} else {
				//	col = Color.black;
				//}
				gridData[x, y] = new Vector3(col.r, col.g, col.b);
				gridGos[x, y] = GameObject.CreatePrimitive(PrimitiveType.Cube);
				gridGos[x, y].transform.position = new Vector3(x *1.1f, y * 1.1f - texture.height * 1.2f * i, 0);
				gridGos[x, y].GetComponent<Renderer>().material.color = col;
				Vector3 aveV3 = gridData[x, y];
				sum += aveV3;
			}
		}
		Vector3 result = sum / (texture.width * texture.height);
//        Debug.Log("data sum:" + sum.ToString("F3") + " ave:" + result.ToString("F3") + "\n");
	}
	void Advance() {
		Vector3 sum = Vector3.zero;
		for (int x = 0; x < texture.width; x++) {
            for (int y = 0; y < texture.height; y++) {
				Color colWeight = gridWeightGos[x, y].GetComponent<Renderer>().material.color;
				Vector3 weightV3 = new Vector3(colWeight.r, colWeight.g, colWeight.b);
				Vector3 dataV3 = gridData[x, y];
				float wx = (dataV3.x + weightV3.x) / 2;
				float wy = (dataV3.y + weightV3.y) / 2;
				float wz = (dataV3.z + weightV3.z) / 2;
				Vector3 aveV3 = new Vector3(wx, wy, wz);
				gridWeights[x, y] = aveV3;        
                gridWeightGos[x, y].GetComponent<Renderer>().material.color = new Color(wx, wy, wz);
				sum += aveV3;
            }
        }       
		Vector3 result = sum / (texture.width * texture.height);
		Debug.Log("advance sum:" + sum.ToString("F3") + " ave:" + result.ToString("F3") + "\n");
	}
	int getASCIIforString(string txt) {
		char ch = char.Parse(txt);
        int result = System.Convert.ToInt32(ch);
		return result;
	}
	string getCharForASCII(int n) {
		int unicode = n;
        char character = (char)unicode;
        string text = character.ToString();
		return text;
	}
	// Update is called once per frame
	void Update () {
		
	}
}
