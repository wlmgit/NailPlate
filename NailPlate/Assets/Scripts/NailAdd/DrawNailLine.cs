using UnityEngine;
using System.Collections;

public class DrawNailLine : MonoBehaviour {

	public LineRenderer mLine;
	
	// Use this for initialization
	void Start () {
		mLine =gameObject.GetComponent<LineRenderer>(); 
		mLine.SetWidth(0.2f, 0.2f); 
		mLine.SetVertexCount(2); 
		mLine.SetColors (Color.yellow,Color.yellow);
		//aMaterial.color = Color.red;
		// mLine.material = aMaterial;
		mLine.material.color = Color.black;  
		mLine.GetComponent<Renderer>().enabled = true; 
		//mLine.SetPosition(0,new Vector3(0,0,0));
		//mLine.SetPosition(1,new Vector3(0,0,0));
		gameObject.GetComponent<LineRenderer>().enabled=false;
	}
	// Update is called once per frame
	void Update () {
	
	}
}
