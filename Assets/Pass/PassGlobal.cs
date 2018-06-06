using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassGlobal : MonoBehaviour {
	public List<PassNode> nodes;
	public List<PassNode> nodesActivated;
	public List<PassNode> nodesFeedForward;
	public float distNear;
	public int numNodes = 100;
	public float radius = 3;
	public int sizeGrid = 28;
	public int sizeBox = 10;
	public float randomizeBy = .25f;
	public GameObject parentNodes;
	public GameObject parentLinks;
	public int cntNodes;
	public int cntLinks;
	public GameObject tmpGo;
	public PassGlobal() {
		nodes = new List<PassNode>();
		nodesActivated = new List<PassNode>();
		nodesFeedForward = new List<PassNode>();
		parentNodes = new GameObject("parentNodes");
		parentLinks = new GameObject("parentLinks");
		tmpGo = new GameObject("tmpGo");
        //
		string style = "line";
        //
		if (style == "line") {
            for (float x = 0; x < sizeGrid; x ++) 
            {
				Vector3 pos = RandomizeWithRange(new Vector3(x, x, 0), randomizeBy);
                PassNode node = new PassNode(pos, this);
            }
        } 
		if (style == "image") {
			string filename = "size28_a"; //_test";
			Texture2D texture = Resources.Load<Texture2D>(filename);
            for (float x = 0; x < sizeGrid; x ++) 
            {
                for (float y = 0; y < sizeGrid; y++)
                {
					Color color = texture.GetPixel((int)x, (int)y);
					float z = 3 * color.grayscale;
                    Vector3 pos = new Vector3(x, z, y);
                    PassNode node = new PassNode(pos, this);
                }
            }
        } 
		if (style == "ball") {
			for (int n = 0; n < numNodes; n++) {
				float yaw = Random.Range(0, 360f);				
				float pitch = Random.Range(0, 360f);
				float roll = 0;
				tmpGo.transform.position = Vector3.zero;
				tmpGo.transform.eulerAngles = new Vector3(pitch, yaw, roll);
				tmpGo.transform.position += tmpGo.transform.forward * radius;
				Vector3 pos = tmpGo.transform.position;
                PassNode node = new PassNode(pos, this);
			}
        } 
		if (style == "box") {
            for (float x = 0; x < sizeBox; x ++) 
            {
				for (float y = 0; y < sizeBox; y++)
                {
					for (float z = 0; z < sizeBox; z++)
                    {
						if ((x == 0 || x == sizeBox - 1) || (y == 0 || y == sizeBox - 1) || (z == 0 || z == sizeBox - 1)) {
							Vector3 pos = new Vector3(x, y, z);
                            PassNode node = new PassNode(pos, this);
						}
                    }
                }
            }
        } 
		if (style == "grid") {
			for (float x = 0; x < sizeGrid; x ++) 
			{
				for (float y = 0; y < sizeGrid; y++)
                {
					for (float z = 0; z < sizeGrid; z++)
                    {
						PassNode node = new PassNode(new Vector3(x, y, z), this);
                    }
                }
			}
		} 
		if (style == "panel") {
            for (float x = 0; x < sizeGrid; x ++) 
			{
                for (float y = 0; y < sizeGrid; y++)
                {
					Vector3 pos = RandomizeWithRange(new Vector3(x, y, 0), randomizeBy);
                    PassNode node = new PassNode(pos, this);
                }
            }
        } 
		if (style == "round") {
			for (int n = 0; n < numNodes; n++)
            {
                PassNode node = new PassNode(Random.insideUnitSphere * radius, this);
            }
		}
	}
	public Vector3 RandomizeWithRange(Vector3 pos, float r) {
		return pos + (Random.insideUnitSphere * r);		
	}
	public void Update() {
		nodesActivated.Clear();
		nodesFeedForward.Clear();
		foreach(PassNode node in nodes) {
			node.Update();
		}
		UpdateNodesActivated();
		UpdateNodesFeedForward();
	}
	public void AddNodeFeedForward(PassNode node)
    {
		if (nodesFeedForward.Contains(node) == false)
        {
			nodesFeedForward.Add(node);
        }
    }
	public void UpdateNodesFeedForward()
    {
		for (int n = 0; n < nodesFeedForward.Count; n++)
        {
			PassNode node = nodesFeedForward[n];
            node.FeedForward();
        }
    }
	public void AddNodeActivated(PassNode node) {
		if (nodesActivated.Contains(node) == false) {
			nodesActivated.Add(node);
		}		
	}
	public void UpdateNodesActivated() {
		for (int n = 0; n < nodesActivated.Count; n++) {
			PassNode node = nodesActivated[n];
			node.CreateLinks();
		}
	}
	public void DestroyLinkGo(GameObject go) {
		DestroyImmediate(go);
		UpdateCntLinks(-1);
	}
	public void UpdateCntLinks(int n) {
		cntLinks += n;
		parentLinks.name = "parentLinks (" + cntLinks + ")";
	}
	public void UpdateCntNodes(int n) {
		cntNodes += n;
		parentNodes.name = "parentNodes (" + cntNodes + ")";
	}
}
